using Microsoft.Extensions.DependencyInjection;
using System;

namespace SequenceGenerator.Tests
{
    public abstract class BaseUnitTest
    {
        protected readonly IServiceProvider _serviceProvider;

        public BaseUnitTest()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSequenceGenerator();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}