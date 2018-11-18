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
    public class PatternTest
    {
        private PatternAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new PatternAttributes();

        [Test]
        public void ShouldPassWhenPatternIsNotPresent()
        {
            target.PassIfAnything(null);
            target.PassIfAnything("Ab 12 @#");
        }

        [Test]
        public void ShouldThrowIfPatternIntegerIsPassedInvalid() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfNotInteger("abcdef"));

        [Test]
        public void ShouldPassIfPatternIntegerIsPassedValid() => target.ThrowIfNotInteger("1234567");

        [Test]
        public void ShouldThrowIfPatternUpperCaseIsPassedInvalid() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfNotUpperCase("ABCDEf"));

        [Test]
        public void ShouldPassIfPatternUpperCaseIsPassedValid() => target.ThrowIfNotUpperCase("ABCDEF");

        [Test]
        public void ShouldThrowIfPatternNotInBracketsIsPassedInvalid() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfNotInBrackets("Value"));

        [Test]
        public void ShouldPassIfPatternNotInBracketsIsPassedValid() => target.ThrowIfNotInBrackets("(Value)");
    }
}
