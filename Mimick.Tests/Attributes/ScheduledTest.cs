using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using NUnit.Framework;

namespace Mimick.Tests.Attributes
{
    [TestFixture]
    public class ScheduledTest
    {
        private ScheduledAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = FrameworkContext.Current.ComponentContext.Resolve<ScheduledAttributes>();

        [Test]
        public void ShouldIncrementCounterEachSecond()
        {
            var before = target.Counter;
            Thread.Sleep(1500);
            var after = target.Counter;

            Assert.That(after > before);
        }
    }
}
