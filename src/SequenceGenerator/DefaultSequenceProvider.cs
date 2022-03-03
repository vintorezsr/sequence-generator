using SequenceGenerator.Exceptions;
using SmartFormat;

namespace SequenceGenerator
{
    public class DefaultSequenceProvider : ISequenceProvider
    {
        private static readonly List<SequenceTemplate> _sequenceTemplateStores = new();
        private static readonly List<SequenceSeed> _sequenceSeeds = new();
        private static object _locker = new();

        public Task<SequenceTemplate> GetTemplateAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var sequenceTemplate = _sequenceTemplateStores.FirstOrDefault(x => x.Key == key);

            if (sequenceTemplate == null)
            {
                throw new SequenceTemplateNotFoundException();
            }

            return Task.FromResult(sequenceTemplate);
        }

        public Task<SequenceTemplate> SaveTemplateAsync(
            SequenceTemplate sequenceTemplateInput,
            CancellationToken cancellationToken = default)
        {
            var sequenceTemplate = _sequenceTemplateStores.FirstOrDefault(x => x.Key == sequenceTemplateInput.Key);

            if (sequenceTemplate == null)
            {
                sequenceTemplate = new SequenceTemplate
                {
                    Id = Guid.NewGuid(),
                    Key = sequenceTemplateInput.Key
                };

                _sequenceTemplateStores.Add(sequenceTemplate);
            }

            sequenceTemplate.Template = sequenceTemplateInput.Template;
            sequenceTemplate.PartitionStrategy = sequenceTemplateInput.PartitionStrategy;
            sequenceTemplate.Increment = sequenceTemplateInput.Increment;
            sequenceTemplate.StartNumber = sequenceTemplateInput.StartNumber;
            sequenceTemplate.MaxNumber = sequenceTemplateInput.MaxNumber;

            return Task.FromResult(sequenceTemplate);
        }

        public Task<SequenceSeed> GenerateSeedAsync(
            SequenceTemplate template,
            Payload? payload = null,
            CancellationToken cancellationToken = default)
        {
            string? partitionStrategy = template.PartitionStrategy != null
                ? Smart.Format(template.PartitionStrategy, payload)
                : null;

            lock (_locker)
            {
                var seed = _sequenceSeeds.FirstOrDefault(x => x.SequenceTemplate == template && x.PartitionStrategy == partitionStrategy)
                    ?? new SequenceSeed
                    {
                        Id = Guid.NewGuid(),
                        PartitionStrategy = partitionStrategy,
                        RunningNumber = template.StartNumber - template.Increment ?? 0,
                        SequenceTemplate = template
                    };

                if (template.MaxNumber.HasValue && seed.RunningNumber + template.Increment > template?.MaxNumber)
                {
                    throw new MaximumValueReachedException();
                }

                seed.RunningNumber += template!.Increment;
                _sequenceSeeds.Add(seed);

                return Task.FromResult(seed);
            }
        }
    }
}