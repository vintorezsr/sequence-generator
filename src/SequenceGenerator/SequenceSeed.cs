namespace SequenceGenerator
{
    public class SequenceSeed
    {
        public Guid Id { get; set; }
        public string? PartitionStrategy { get; set; }
        public long RunningNumber { get; set; }
        public SequenceTemplate SequenceTemplate { get; set; } = default!;
    }
}