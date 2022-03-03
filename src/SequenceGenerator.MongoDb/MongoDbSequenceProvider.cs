using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SequenceGenerator.Exceptions;
using SmartFormat;

namespace SequenceGenerator.MongoDb
{
    public class MongoDbSequenceProvider : ISequenceProvider
    {
        private readonly MongoClientContext _mongoClientContext;

        public MongoDbSequenceProvider(MongoClientContext mongoClientContext)
        {
            _mongoClientContext = mongoClientContext;
        }

        public async Task<SequenceTemplate> GetTemplateAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var sequenceTemplate = await _mongoClientContext.SequenceTemplates.Find(x => x.Key == key)
                .FirstOrDefaultAsync();

            if (sequenceTemplate == null)
            {
                throw new SequenceTemplateNotFoundException();
            }

            return sequenceTemplate;
        }

        public async Task<SequenceTemplate> SaveTemplateAsync(
            SequenceTemplate sequenceTemplate,
            CancellationToken cancellationToken = default)
        {
            if (sequenceTemplate.Id == Guid.Empty)
            {
                sequenceTemplate.Id = Guid.NewGuid();
            }

            await _mongoClientContext.SequenceTemplates.ReplaceOneAsync(x => x.Id == sequenceTemplate.Id, sequenceTemplate, new ReplaceOptions { IsUpsert = true });

            return sequenceTemplate;
        }

        public async Task<SequenceSeed> GenerateSeedAsync(
            SequenceTemplate template,
            Payload? payload = null,
            CancellationToken cancellationToken = default)
        {
            string? partitionStrategy = template.PartitionStrategy != null
                ? Smart.Format(template.PartitionStrategy, payload)
                : null;

            var seed = _mongoClientContext.SequenceSeeds.Find(x => x.SequenceTemplate == template && x.PartitionStrategy == partitionStrategy).FirstOrDefault()
                ?? new SequenceSeed { Id = Guid.NewGuid(), RunningNumber = template.StartNumber - template.Increment ?? 0, SequenceTemplate = template };

            if (template.MaxNumber.HasValue && seed.RunningNumber + template.Increment > template?.MaxNumber)
            {
                throw new MaximumValueReachedException();
            }

            seed.RunningNumber += template!.Increment;

            await _mongoClientContext.SequenceSeeds.ReplaceOneAsync(x => x.Id == seed.Id, seed, new ReplaceOptions { IsUpsert = true });

            return seed;
        }
    }
}