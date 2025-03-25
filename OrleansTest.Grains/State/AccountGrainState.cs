
namespace OrleansTest.Grains.State
{
    [GenerateSerializer]
    public record AccountGrainState
    {
        [Id(0)]
        public required string Name { get; set; }

        [Id(1)]
        public DateTime CreatedAtUtc { get; set; }

        [Id(2)]
        public List<Guid> CameraIds { get; set; } = new List<Guid>();
    }
}
