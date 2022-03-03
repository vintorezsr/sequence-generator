using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SequenceGenerator.EntityFramework.Tests
{
    public class SequenceGeneratorTest : BaseUnitTest
    {
        [Fact]
        public async Task GenerateAsyncTest()
        {
            var sequenceProvider = _serviceProvider.GetService<ISequenceProvider>()!;
            var sequenceGenerator = _serviceProvider.GetService<ISequenceGenerator>()!;
            var sequenceTemplateKey = "sequence1";

            await sequenceProvider.SaveTemplateAsync(new SequenceTemplate
            {
                Key = sequenceTemplateKey,
                Template = "SEQ_{RunningNumber:000000}"
            });

            var expected = new SequenceNumber(1, "SEQ_000001");
            var actual = await sequenceGenerator.GenerateAsync(sequenceTemplateKey);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task MultiConcurrencyTest()
        {
            var sequenceProvider = _serviceProvider.GetService<ISequenceProvider>()!;
            var sequenceTemplateKey = "sequence1";
            
            await sequenceProvider.SaveTemplateAsync(new SequenceTemplate
            {
                Key = sequenceTemplateKey,
                Template = "MULTI_SEQ_{RunningNumber:000000}"
            });

            var cancellationTokenSource = new CancellationTokenSource();
            var tasks = Enumerable.Range(0, 100).Select(x =>
            {
                using var supressFlow = ExecutionContext.SuppressFlow();

                return Task.Run(async () =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var sequenceGenerator = scope.ServiceProvider.GetService<ISequenceGenerator>()!;
                    var dbContext = scope.ServiceProvider.GetRequiredService<SequenceDbContext>();
                    var number = string.Empty;

                    try
                    {
                        using var transaction = await dbContext.Database.BeginTransactionAsync();
                        var sequenceNumber = await sequenceGenerator.GenerateAsync(sequenceTemplateKey, cancellationToken: cancellationTokenSource.Token);
                        number = sequenceNumber.Number;
                        transaction.Commit();
                    }
                    catch
                    {
                        cancellationTokenSource.Cancel();
                    }

                    return number;
                });
            });

            var docs = await Task.WhenAll(tasks);

            Assert.Equal(docs.Length, docs.Distinct().Count());
        }
    }
}