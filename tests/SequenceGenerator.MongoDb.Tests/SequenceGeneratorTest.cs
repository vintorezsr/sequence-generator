using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace SequenceGenerator.MongoDb.Tests
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
    }
}