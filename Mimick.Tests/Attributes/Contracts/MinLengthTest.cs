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
    public class MinLengthTest
    {
        private List<string> invalid;
        private List<string> valid;
        private MinLengthAttributes target;

        [OneTimeSetUp]
        public void SetUp()
        {
            invalid = new List<string>(new[] { "A" });
            valid = new List<string>(new[] { "A", "B", "C", "D", "E", "F", "G" });
            target = new MinLengthAttributes();
        }

        [Test]
        public void ShouldPassWhenMinLengthIsNotPresent()
        {
            target.PassIfBelow(null);
            target.PassIfBelow(invalid);
        }

        [Test]
        public void ShouldFailWhenMinLengthListIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfBelow(invalid));

        [Test]
        public void ShouldPassWhenMinLengthListIsPassedValid() => target.ThrowIfBelow(valid);

        [Test]
        public void ShouldFailWhenMinLengthEnumerableIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfBelow((IEnumerable<string>)invalid));

        [Test]
        public void ShouldPassWhenMinLengthEnumerableIsPassedValid() => target.ThrowIfBelow((IEnumerable<string>)valid);

        [Test]
        public void ShouldFailWhenMinLengthStringIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfBelow("A"));

        [Test]
        public void ShouldPassWhenMinLengthStringIsPassedValid() => target.ThrowIfBelow("ABCDEFG");

        [Test]
        public void ShouldFailWhenMinLengthArrayIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfBelow(invalid.ToArray()));

        [Test]
        public void ShouldPassWhenMinLengthArrayIsPassedValid() => target.ThrowIfBelow(valid.ToArray());

        [Test]
        public void ShouldFailWhenMinLengthMethodIsPassedInvalid() => Assert.Throws(typeof(ArgumentOutOfRangeException), () => target.ThrowIfAnyBelow("A", "B"));

        [Test]
        public void ShouldPassWhenMinLengthMethodIsPassedValid() => target.ThrowIfAnyBelow("ABCDEFG", "HIJKLMN");
    }
}
