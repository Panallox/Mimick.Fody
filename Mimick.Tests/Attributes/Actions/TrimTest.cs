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
    public class TrimTest
    {
        private TrimAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new TrimAttributes();

        [Test]
        public void ShouldReturnNullWhenPassedNull() => Assert.IsNull(target.Trim((string)null));

        [Test]
        public void ShouldReturnTrimmedWhenPassedWhitespace() => Assert.AreEqual("abc", target.Trim("  abc  "));

        [Test]
        public void ShouldReturnTrimmedBuilderWhenPassedWhitespace() => Assert.AreEqual(new StringBuilder("abc").ToString(), target.Trim(new StringBuilder("  abc  ").ToString()));

        [Test]
        public void ShouldReturnNullWhenReturnIsPassedNull() => Assert.IsNull(target.TrimReturn(null));

        [Test]
        public void ShouldReturnTrimmedWhenReturnIsPassedWhitespace() => Assert.AreEqual("abc", target.TrimReturn("  abc  "));

        [Test]
        public void ShouldReturnNullWhenPropertyIsPassedNull()
        {
            target.Value = null;
            Assert.IsNull(target.Value);
        }

        [Test]
        public void ShouldReturnTrimmedWhenPropertyIsPassedWhitespace()
        {
            target.Value = "  abc  ";
            Assert.AreEqual("abc", target.Value);
        }

        [Test]
        public void ShouldReturnSameValueWhenConditionalIsPassedUnsupported() => Assert.AreEqual(123, (int)target.TrimConditional(123));

        [Test]
        public void ShouldReturnTrimmedWhenConditionalIsPassedWhitespace() => Assert.AreEqual("abc", (string)target.TrimConditional("  abc  "));
    }
}
