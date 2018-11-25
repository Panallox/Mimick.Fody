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
        public void ShouldIncrementTimedCounterEachSecond()
        {
            var before = target.TimedCounter;
            Thread.Sleep(1500);
            var after = target.TimedCounter;

            Assert.That(after > before);
        }

        /*
        [Test]
        public void ShouldIncrementScheduledCounterEveryTwoSeconds()
        {
            var before = target.TimedCounter;
            Thread.Sleep(5000);
            var after = target.TimedCounter;

            Assert.That(after > before);
            Assert.AreEqual(before + 2, after);
        }
        */
    }
}
