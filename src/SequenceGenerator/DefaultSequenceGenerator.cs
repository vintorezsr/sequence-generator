using SmartFormat;

namespace SequenceGenerator
{
    public class DefaultSequenceGenerator : ISequenceGenerator
    {
        private readonly ISequenceProvider _sequenceProvider;

        public DefaultSequenceGenerator(ISequenceProvider sequenceProvider)
        {
            _sequenceProvider = sequenceProvider;
        }

        public async Task<SequenceNumber> GenerateAsync(
            string templateKey,
            Payload? payload = null,
            CancellationToken cancellationToken = default)
        {
            payload ??= new Payload();

            var template = await _sequenceProvider.GetTemplateAsync(templateKey, cancellationToken);
            var seed = await _sequenceProvider.GenerateSeedAsync(template, payload, cancellationToken);
            var sequenceNumber = GenerateSequenceNumber(seed, payload);

            return sequenceNumber;
        }

        private SequenceNumber GenerateSequenceNumber(SequenceSeed seed, Payload payload)
        {
            payload.Update(nameof(seed.RunningNumber), seed.RunningNumber);

            var sequenceNumber = new SequenceNumber(seed.RunningNumber, Smart.Format(seed.SequenceTemplate!.Template, payload));

            return sequenceNumber;
        }
    }
}