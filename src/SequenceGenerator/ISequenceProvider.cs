namespace SequenceGenerator
{
    public interface ISequenceProvider
    {
        Task<SequenceTemplate> SaveTemplateAsync(
            SequenceTemplate sequenceTemplate,
            CancellationToken cancellationToken = default);

        Task<SequenceTemplate> GetTemplateAsync(
            string key,
            CancellationToken cancellationToken = default);

        Task<SequenceSeed> GenerateSeedAsync(
            SequenceTemplate template,
            Payload? payload = default,
            CancellationToken cancellationToken = default);
    }
}