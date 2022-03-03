using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using Xunit;

namespace SequenceGenerator.MongoDb.Tests
{
    public abstract class BaseUnitTest
    {
        protected readonly IServiceProvider _serviceProvider;

        public BaseUnitTest()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSequenceGenerator()
                .UseMongoDbSequenceProvider("mongodb://localhost:27017", "sequence_generator", true);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}