using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace SequenceGenerator.MongoDb
{
    public static class ServiceExtensions
    {
        public static IServiceCollection UseMongoDbSequenceProvider(
            this IServiceCollection serviceCollection,
            string connectionString,
            string databaseName,
            bool drop = false)
        {
            serviceCollection.RemoveAll<ISequenceProvider>();
            serviceCollection.AddTransient<ISequenceProvider, MongoDbSequenceProvider>();
            serviceCollection.AddSingleton<IMongoClient, MongoClient>(x => new MongoClient(connectionString));
            serviceCollection.AddSingleton(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();

                if (drop)
                {
                    mongoClient.DropDatabase(databaseName);
                }

                return mongoClient.GetDatabase(databaseName);
            });
            serviceCollection.AddTransient<MongoClientContext>();

            return serviceCollection;
        }
    }
}