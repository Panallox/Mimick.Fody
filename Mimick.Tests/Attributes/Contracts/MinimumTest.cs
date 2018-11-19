using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Contracts;
using NUnit.Framework;

namespace Mimick.Tests.Attributes.Contracts
{
    [TestFixture]
    public class MinimumTest
    {
        private MinimumAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new MinimumAttributes();

        [Test]
        public void ShouldPassWhenMinimumIsNotPresent() => target.PassIfBelow(int.MinValue);

        [Test]
        public void ShouldThrowWhenMinimumIntIsPassedValueLessThan() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfBelow(0));

        [Test]
        public void ShouldPassWhenMinimumIntIsPassedValueGreaterThan() => target.ThrowIfBelow(20);

        [Test]
        public void ShouldThrowWhenMinimumMethodIsPassedValueLessThan() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAnyBelow(0, 10));

        [Test]
        public void ShouldPassWhenMinimumMethodIsPassedValueGreaterThan() => target.ThrowIfAnyBelow(20, 15);

        [Test]
        public void ShouldThrowWhenMinimumReturnIsReturningValueLessThan() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfReturnsBelow(0));

        [Test]
        public void ShouldPassWhenMinimumReturnIsReturningValueGreaterThan() => target.ThrowIfReturnsBelow(10);
    }
}
