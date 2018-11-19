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
    public class MaxLengthTest
    {
        private List<string> invalid;
        private List<string> valid;
        private MaxLengthAttributes target;

        [OneTimeSetUp]
        public void SetUp()
        {
            invalid = new List<string>(new[] { "A", "B", "C", "D", "E", "F", "G" });
            valid = new List<string>(new[] { "A" });
            target = new MaxLengthAttributes();
        }

        [Test]
        public void ShouldPassWhenMaxLengthIsNotPresent()
        {
            target.PassIfAbove(null);
            target.PassIfAbove(invalid);
        }

        [Test]
        public void ShouldThrowWhenMaxLengthListIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAbove(invalid));

        [Test]
        public void ShouldPassWhenMaxLengthListIsPassedValid() => target.ThrowIfAbove(valid);

        [Test]
        public void ShouldThrowWhenMaxLengthEnumerableIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAbove((IEnumerable<string>)invalid));

        [Test]
        public void ShouldPassWhenMaxLengthEnumerableIsPassedValid() => target.ThrowIfAbove((IEnumerable<string>)valid);

        [Test]
        public void ShouldThrowWhenMaxLengthStringIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAbove("ABCDEFG"));

        [Test]
        public void ShouldPassWhenMaxLengthStringIsPassedValid() => target.ThrowIfAbove("A");

        [Test]
        public void ShouldThrowWhenMaxLengthArrayIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAbove(invalid.ToArray()));

        [Test]
        public void ShouldPassWhenMaxLengthArrayIsPassedValid() => target.ThrowIfAbove(valid.ToArray());

        [Test]
        public void ShouldThrowWhenMaxLengthMethodIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAnyAbove("A", "ABCDEFG"));

        [Test]
        public void ShouldPassWhenMaxLengthMethodIsPassedValid() => target.ThrowIfAnyAbove("A", "B");
    }
}
