using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests.Attributes
{
    [TestClass]
    public class ConstructTest
    {
        [TestMethod]
        public void ShouldInvokeBeforeAndAfterMethodsWhenConstructed()
        {
            var target = new ConstructAttributes();

            Assert.AreEqual(1, target.BeforeConstructionCount);
            Assert.AreEqual(1, target.ConstructionCount);
            Assert.AreEqual(1, target.AfterConstructionCount);
        }

        [TestMethod]
        public void ShouldInvokeBeforeAndAfterMethodsWhenConstructedOnlyOnce()
        {
            var target = new ConstructAttributes(1);

            Assert.AreEqual(1, target.BeforeConstructionCount);
            Assert.AreEqual(2, target.ConstructionCount);
            Assert.AreEqual(1, target.AfterConstructionCount);
        }
    }
}
