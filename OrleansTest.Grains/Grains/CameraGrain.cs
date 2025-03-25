using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Streams;
using OrleansTest.Grains.Events;
using OrleansTest.Grains.GrainInterfaces;
using OrleansTest.Grains.State;

namespace OrleansTest.Grains.Grains
{
    public class CameraGrain(
        [PersistentState("camera", "cameraStore")] IPersistentState<CameraGrainState> cameraState)
        : Grain, ICameraGrain, IRemindable
    {
        private IGrainTimer? _timerHandle;
        private readonly Random _random = new Random();
        public async Task Initialize(string cameraName)
        {
            cameraState.State.Name = cameraName;
            await cameraState.WriteStateAsync();

            if (_timerHandle != null)
            {
                _timerHandle.Dispose();
                _timerHandle = null;
            }
            _timerHandle = this.RegisterGrainTimer(TimerCallback, TimeSpan.Zero, TimeSpan.FromSeconds(RandomNumberBetween(5, 10)));

            await this.RegisterOrUpdateReminder($"MotionReminder:::{this.GetGrainId().GetGuidKey()}",
                TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public Task<string> GetName()
        {
            return Task.FromResult(cameraState.State.Name);
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (_timerHandle == null && reminderName.StartsWith("MotionReminder"))
            {
                _timerHandle = this.RegisterGrainTimer(TimerCallback, TimeSpan.Zero, TimeSpan.FromSeconds(RandomNumberBetween(5, 10)));
            }

            // If there are other reminders parse out which one it is from reminderName
            return Task.CompletedTask;
        }

        private async Task TimerCallback()
        {
            var streamProvider = this.GetStreamProvider("StreamProvider");
            var streamId = StreamId.Create("MotionStream", this.GetGrainId().GetGuidKey());
            var stream = streamProvider.GetStream<MotionEvent>(streamId);
            await stream.OnNextAsync(new MotionEvent()
            {
                CameraId = this.GetGrainId().GetGuidKey(),
                CameraName = cameraState.State.Name,
                EventTime = DateTime.UtcNow
            });
        }

        private double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = _random.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }
    }
}
