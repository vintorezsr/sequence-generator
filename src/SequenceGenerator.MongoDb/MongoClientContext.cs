using MongoDB.Driver;

namespace SequenceGenerator.MongoDb
{
    public class MongoClientContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoClientContext(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            Configure();
        }

        public virtual IMongoCollection<SequenceTemplate> SequenceTemplates => _mongoDatabase.GetCollection<SequenceTemplate>(nameof(SequenceTemplate));
        public virtual IMongoCollection<SequenceSeed> SequenceSeeds => _mongoDatabase.GetCollection<SequenceSeed>(nameof(SequenceSeed));

        private void Configure()
        {
            var indexBuilder = Builders<SequenceTemplate>.IndexKeys;
            var indexKeys = indexBuilder.Ascending(e => e.Key);
            var index = new CreateIndexModel<SequenceTemplate>(indexKeys, new CreateIndexOptions()
            {
                Unique = true,
                Name = "ix_key"
            });

            SequenceTemplates.Indexes.CreateOne(index);
        }
    }
}