using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SequenceGenerator.EntityFramework.Tests
{
    public abstract class BaseUnitTest
    {
        protected readonly IServiceProvider _serviceProvider;

        public BaseUnitTest()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSequenceGenerator()
                .UseEntityFrameworkSequenceProvider((serviceProvider, builder) =>
                {
                    builder.UseSqlServer(GetConnectionString());
                });

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var context = _serviceProvider.GetRequiredService<SequenceDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private string GetConnectionString()
        {
            var msSqlServerConnectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING")
                ?? "Data Source=localhost;User Id=sa;Password=Passw0rd;TrustServerCertificate=true;Encrypt=yes";

            var connectionStringBuilder =
                new SqlConnectionStringBuilder(msSqlServerConnectionString)
                {
                    InitialCatalog = "sequence_generator"
                };

            var connectionString = connectionStringBuilder.ToString();

            return connectionString;
        }
    }
}