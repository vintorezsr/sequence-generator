namespace SequenceGenerator
{
    public class SequenceTemplate
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = default!;
        public string Template { get; set; } = default!;
        public string? PartitionStrategy { get; set; }
        public int Increment { get; set; } = 1;
        public long? StartNumber { get; set; }
        public long? MaxNumber { get; set; }
    }
}