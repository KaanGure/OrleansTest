using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansTest.Grains.Events
{
    [GenerateSerializer]
    public record MotionEvent
    {
        [Id(0)]
        public Guid CameraId { get; set; }

        [Id(1)]
        public required string CameraName { get; set; }

        [Id(2)]
        public DateTime EventTime { get; set; }
    }
}
