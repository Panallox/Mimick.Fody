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
    public class UpperCaseTest
    {
        private UpperCaseAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new UpperCaseAttributes();

        [Test]
        public void ShouldReturnNullWhenPassedNull() => Assert.IsNull(target.Upper(null));

        [Test]
        public void ShouldReturnUpperWhenPassedValid() => Assert.AreEqual("ABC", target.Upper("abc"));

        [Test]
        public void ShouldReturnUpperWhenReturnIsPassedValid() => Assert.AreEqual("ABC", target.UpperReturn("abc"));

        [Test]
        public void ShouldReturnSameValueWhenConditionalIsPassedUnsupported() => Assert.AreEqual(123, target.UpperConditional(123));

        [Test]
        public void ShouldReturnUpperWhenPropertyIsPassedValid()
        {
            target.Value = "abc";
            Assert.AreEqual("ABC", target.Value);
        }
    }
}
