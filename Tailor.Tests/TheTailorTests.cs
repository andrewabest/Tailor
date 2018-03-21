using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using Tailor.Test;
using Tailor.Tests.Sample;

namespace Tailor.Tests
{
    public class TheTailorTests
    {
        [Test]
        public void WhenCreatingTheTailorWithAnEmptyConnectionString_ThrowsAnException()
        {
            Should.Throw<ArgumentException>(() => TheTailor.Create(string.Empty, typeof(IAmTheQueryAssembly).Assembly.GetExportedTypes()
                .Where(x => x.Namespace == "Tailor.Tests.Sample").ToArray()));
        }
    }
}
