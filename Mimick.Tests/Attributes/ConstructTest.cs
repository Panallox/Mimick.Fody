using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using NUnit.Framework;

namespace Mimick.Tests.Attributes
{
    [TestFixture]
    public class ConstructTest
    {
        [Test]
        public void ShouldInvokeBeforeAndAfterMethodsWhenConstructed()
        {
            var target = new ConstructAttributes();

            Assert.AreEqual(1, target.BeforeConstructionCount);
            Assert.AreEqual(1, target.ConstructionCount);
            Assert.AreEqual(1, target.AfterConstructionCount);
        }

        [Test]
        public void ShouldInvokeBeforeAndAfterMethodsWhenConstructedOnlyOnce()
        {
            var target = new ConstructAttributes(1);

            Assert.AreEqual(1, target.BeforeConstructionCount);
            Assert.AreEqual(2, target.ConstructionCount);
            Assert.AreEqual(1, target.AfterConstructionCount);
        }
    }
}
