using Microsoft.Extensions.DependencyInjection;

namespace SequenceGenerator
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSequenceGenerator(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISequenceProvider, DefaultSequenceProvider>();
            serviceCollection.AddTransient<ISequenceGenerator, DefaultSequenceGenerator>();

            return serviceCollection;
        }
    }
}