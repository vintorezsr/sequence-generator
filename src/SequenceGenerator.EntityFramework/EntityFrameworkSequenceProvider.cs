using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SequenceGenerator.Exceptions;
using SmartFormat;

namespace SequenceGenerator.EntityFramework
{
    public class EntityFrameworkSequenceProvider : ISequenceProvider
    {
        private readonly SequenceDbContext _sequenceDbContext;

        public EntityFrameworkSequenceProvider(SequenceDbContext sequenceDbContext)
        {
            _sequenceDbContext = sequenceDbContext;
        }

        public async Task<SequenceTemplate> GetTemplateAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var sequenceTemplate = await _sequenceDbContext.SequenceTemplates.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

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
            _sequenceDbContext.SequenceTemplates.Update(sequenceTemplate);
            await _sequenceDbContext.SaveChangesAsync(cancellationToken);
            _sequenceDbContext.Entry(sequenceTemplate).State = EntityState.Detached;

            return sequenceTemplate;
        }

        public async Task<SequenceSeed> GenerateSeedAsync(
            SequenceTemplate template,
            Payload? payload = default,
            CancellationToken cancellationToken = default)
        {
            string? partitionStrategy = template.PartitionStrategy != null
                ? Smart.Format(template.PartitionStrategy, payload)
                : null;
            var dbConnection = _sequenceDbContext.Database.GetDbConnection();
            var dbTransaction = _sequenceDbContext.Database.CurrentTransaction?.GetDbTransaction();
            var needToBeCommit = dbTransaction == null;

            dbTransaction ??= _sequenceDbContext.Database.BeginTransaction().GetDbTransaction();

            var parameters = new
            {
                SequenceTemplateId = template.Id,
                StartNumber = template.StartNumber ?? 1,
                Increment = template.Increment,
                PartitionStrategy = partitionStrategy
            };
            var query = @$"MERGE INTO SequenceSeeds WITH(HOLDLOCK) AS TARGET
                USING (
	                SELECT @{nameof(parameters.SequenceTemplateId)} AS {nameof(parameters.SequenceTemplateId)}, ISNULL(@PartitionStrategy, '') AS {nameof(parameters.PartitionStrategy)}
                ) AS SOURCE ON SOURCE.{nameof(parameters.SequenceTemplateId)} = TARGET.{nameof(parameters.SequenceTemplateId)} AND SOURCE.{nameof(parameters.PartitionStrategy)} = ISNULL(TARGET.{nameof(SequenceTemplate.PartitionStrategy)}, '')
                WHEN MATCHED THEN
	                UPDATE SET
                        TARGET.{nameof(SequenceSeed.RunningNumber)} = TARGET.{nameof(SequenceSeed.RunningNumber)} + @{nameof(parameters.Increment)}
                WHEN NOT MATCHED THEN
	                INSERT ({nameof(parameters.SequenceTemplateId)}, {nameof(parameters.PartitionStrategy)}, {nameof(SequenceSeed.RunningNumber)})
                VALUES(@{nameof(parameters.SequenceTemplateId)}, @{nameof(parameters.PartitionStrategy)}, @{nameof(parameters.StartNumber)})
	                OUTPUT inserted.*;";

            var seed = await dbConnection.QueryFirstOrDefaultAsync<SequenceSeed>(query, parameters, dbTransaction);

            if (template.MaxNumber.HasValue && seed.RunningNumber > template?.MaxNumber)
            {
                throw new MaximumValueReachedException();
            }

            seed.SequenceTemplate = template!;

            if (needToBeCommit)
            {
                await dbTransaction.CommitAsync(cancellationToken);
            }

            return seed;
        }
    }
}