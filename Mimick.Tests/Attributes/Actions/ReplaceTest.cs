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
    public class ReplaceTest
    {
        private ReplaceAttributes target;

        [OneTimeSetUp]
        public void SetUp() => target = new ReplaceAttributes();

        [Test]
        public void ShouldReturnNullWhenPassedNull() => Assert.IsNull(target.Replace(null));

        [Test]
        public void ShouldReturnReplacedWhenPassedValid() => Assert.AreEqual("abc****", target.Replace("abc1234"));

        [Test]
        public void ShouldReturnReplacedWhenReturnIsPassedValid() => Assert.AreEqual("abc****", target.ReplaceReturn("abc1234"));

        [Test]
        public void ShouldReturnSameValueWhenConditionalIsPassedUnsupported() => Assert.AreEqual(12345, target.ReplaceConditional(12345));

        [Test]
        public void ShouldReturnReplacedWhenPropertyIsPassedValid()
        {
            target.Value = "abc1234";
            Assert.AreEqual("abc****", target.Value);
        }
    }
}
