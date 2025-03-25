using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Orleans.Streams;
using OrleansTest.Grains.Events;
using OrleansTest.Grains.GrainInterfaces;
using OrleansTest.Grains.State;

namespace OrleansTest.Grains.Grains
{
    public class AccountGrain(
        [PersistentState("account", "accountStore")] IPersistentState<AccountGrainState> accountState)
        : Grain, IAccountGrain, IAsyncObserver<MotionEvent>
    {
        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // Resubscribe to streams on activation
            var streamProvider = this.GetStreamProvider("StreamProvider");
            foreach (Guid cameraId in accountState.State.CameraIds)
            {
                var streamId = StreamId.Create("MotionStream", cameraId);
                var stream = streamProvider.GetStream<MotionEvent>(streamId);
                var handles = await stream.GetAllSubscriptionHandles();
                foreach (var handle in handles)
                {
                    await handle.ResumeAsync(this);
                }
            }
        }

        public async Task Initialize(string accountName)
        {
            accountState.State.Name = accountName;
            accountState.State.CreatedAtUtc = DateTime.UtcNow;

            await accountState.WriteStateAsync();
        }

        public async Task AddCamera(Guid cameraId)
        {
            accountState.State.CameraIds.Add(cameraId);

            var streamProvider = this.GetStreamProvider("StreamProvider");
            var streamId = StreamId.Create("MotionStream", cameraId);
            var stream = streamProvider.GetStream<MotionEvent>(streamId);
            await stream.SubscribeAsync(this);

            await accountState.WriteStateAsync();
        }

        public Task<List<Guid>> GetCameraIds()
        {
            return Task.FromResult(accountState.State.CameraIds);
        }

        public Task<string> GetName()
        {
            return Task.FromResult(accountState.State.Name);
        }

        public Task<DateTime> GetCreationTime()
        {
            return Task.FromResult(accountState.State.CreatedAtUtc);
        }

        Task IAsyncObserver<MotionEvent>.OnNextAsync(MotionEvent item, StreamSequenceToken? token)
        {
            string motionEventString = JsonSerializer.Serialize(item);
            Console.WriteLine(motionEventString);
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask; //Do error handling on actual application
    }
}
