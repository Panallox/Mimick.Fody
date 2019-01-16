using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Actions;
using NUnit.Framework;

namespace Mimick.Tests.Attributes.Actions
{
    [TestFixture]
    public class RoundTest
    {
        private RoundAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new RoundAttributes();

        [Test]
        public void ShouldReturnRoundedWhenPassedFraction() => Assert.AreEqual(10.0, target.Round(10.24567));

        [Test]
        public void ShouldReturnRoundedUpWhenPassedFractionOverHalf() => Assert.AreEqual(11.0, target.Round(10.8762));

        [Test]
        public void ShouldReturnRoundedWhenToTwoIsPassedFraction() => Assert.AreEqual(10.44, target.RoundTo2(10.4389));

        [Test]
        public void ShouldReturnRoundedDownWhenToEvenIsPassedFraction() => Assert.AreEqual(10.0, target.RoundToNearestEven(10.5));

        [Test]
        public void ShouldReturnRoundedWhenReturnIsPassedFraction() => Assert.AreEqual(10.0, target.RoundReturn(10.4212));

        [Test]
        public void ShouldReturnSameValueWhenConditionalIsPassedUnsupported() => Assert.AreEqual("abc", target.RoundConditional("abc"));

        [Test]
        public void ShouldReturnRoundedWhenPropertyIsPassedFraction()
        {
            target.Value = 4.23701;
            Assert.AreEqual(4.0, target.Value);
        }
    }
}
