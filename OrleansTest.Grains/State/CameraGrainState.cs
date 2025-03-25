
namespace OrleansTest.Grains.State
{
    [GenerateSerializer]
    public record CameraGrainState
    {
        [Id(0)]
        public required string Name { get; set; }

        //[Id(1)]
        //public DateTime CreatedAtUtc { get; set; }
    }
}
