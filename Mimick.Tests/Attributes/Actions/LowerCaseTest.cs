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
    public class LowerCaseTest
    {
        private LowerCaseAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new LowerCaseAttributes();

        [Test]
        public void ShouldReturnNullWhenPassedNull() => Assert.IsNull(target.Lower(null));

        [Test]
        public void ShouldReturnLowerWhenPassedValid() => Assert.AreEqual("abc", target.Lower("ABC"));

        [Test]
        public void ShouldReturnLowerWhenReturnIsPassedValid() => Assert.AreEqual("abc", target.LowerReturn("ABC"));

        [Test]
        public void ShouldReturnSameValueWhenConditionalIsPassedUnsupported() => Assert.AreEqual(123, target.LowerConditional(123));

        [Test]
        public void ShouldReturnLowerWhenPropertyIsPassedValid()
        {
            target.Value = "ABC";
            Assert.AreEqual("abc", target.Value);
        }
    }
}
