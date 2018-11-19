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
    public class MaximumTest
    {
        private MaximumAttributes target;

        [OneTimeSetUp]
        public void Initialize() => target = new MaximumAttributes();

        [Test]
        public void ShouldPassWhenMaximumIsNotPresent() => target.PassIfAbove(int.MaxValue);

        [Test]
        public void ShouldThrowWhenMaximumIntIsPassedValueGreaterThan() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAbove(20));

        [Test]
        public void ShouldPassWhenMaximumIntIsPassedValueLessThan() => target.ThrowIfAbove(5);

        [Test]
        public void ShouldThrowWhenMaximumMethodIsPassedValueGreaterThan() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAnyAbove(5, 20));

        [Test]
        public void ShouldPassWhenMaximumMethodIsPassedValueLessThan() => target.ThrowIfAnyAbove(5, 10);

        [Test]
        public void ShouldThrowWhenMaximumReturnsIsReturningValueGreaterThan() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfReturnAbove(20));

        [Test]
        public void ShouldPassWhenMaximumReturnsIsReturningValueLessThan() => target.ThrowIfReturnAbove(10);
    }
}
