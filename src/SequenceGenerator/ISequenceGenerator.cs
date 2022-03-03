namespace SequenceGenerator
{
    public interface ISequenceGenerator
    {
        Task<SequenceNumber> GenerateAsync(
            string templateKey,
            Payload? payload = default,
            CancellationToken cancellationToken = default);
    }
}