using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SequenceGenerator.EntityFramework
{
    public static class ServiceExtensions
    {
        public static IServiceCollection UseEntityFrameworkSequenceProvider(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> configure)
        {
            serviceCollection.RemoveAll<ISequenceProvider>();
            serviceCollection.AddTransient<ISequenceProvider, EntityFrameworkSequenceProvider>();
            serviceCollection.AddDbContextFactory<SequenceDbContext>(configure);

            return serviceCollection;
        }
    }
}