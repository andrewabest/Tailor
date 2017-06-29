using System.Linq;
using System.Threading.Tasks;
using Conventional;
using NUnit.Framework;
using Tailor.Test;

namespace Tailor.Tests.Sample
{
    public class SampleTests : DbTest
    {
        [Test]
        public async Task AllQueries_MustSatisfyTheTailor()
        {
            var results = await Task.WhenAll(
                TheTailor
                    .Create(TestDbConnectionString, 
                        typeof(IAmTheQueryAssembly).Assembly.GetExportedTypes().Where(x => x.Namespace == "Tailor.Tests.Sample").ToArray())
                    .Measure(typeof(NotFoundException)));

            foreach (var result in results)
            {
                result.WithFailureAssertion(Assert.Fail);
            }
        }
    }
}