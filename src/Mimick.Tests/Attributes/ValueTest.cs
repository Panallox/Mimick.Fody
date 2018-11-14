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
    public class ValueTest
    {
        private static ValueAttributes target;

        [ClassInitialize]
        public static void BeforeClass(TestContext context) => target = new ValueAttributes();

        [TestMethod]
        public void ShouldReturnValidNumberWhenSimple() => Assert.AreEqual(10, target.SimpleNumber);

        [TestMethod]
        public void ShouldReturnValidStringWhenSimple() => Assert.AreEqual("Test", target.SimpleString);
        
        [TestMethod]
        public void ShouldReturnValidNumberWhenComputed() => Assert.AreEqual(50, target.ComputedNumber);

        [TestMethod]
        public void ShouldReturnValidStringWhenComputed() => Assert.AreEqual("Test 1 Value", target.ComputedString);

        [TestMethod]
        public void ShouldReturnValidNumberWhenConfigured() => Assert.AreEqual(12345, target.ConfiguredNumber);

        [TestMethod]
        public void ShouldReturnValidStringWhenConfigured() => Assert.AreEqual("Testing", target.ConfiguredString);

        [TestMethod]
        public void ShouldReturnValidNumberWhenComplex() => Assert.AreEqual(90, target.ComplexNumber);

        [TestMethod]
        public void ShouldReturnValidStringWhenComplex() => Assert.AreEqual("Number 90", target.ComplexString);

        [TestMethod]
        public void ShouldReturnValidMethodNumberWhenComputed() => Assert.AreEqual(60, target.GetComputedNumber());

        [TestMethod]
        public void ShouldReturnValidMethodStringWhenComputed() => Assert.AreEqual("Testing 30 Result", target.GetComputedString());
    }
}
