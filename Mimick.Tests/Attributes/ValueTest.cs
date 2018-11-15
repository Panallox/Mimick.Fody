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
    public class ValueTest
    {
        private ValueAttributes target;

        [OneTimeSetUp]
        public void BeforeClass() => target = new ValueAttributes();

        [Test]
        public void ShouldReturnValidNumberWhenSimple() => Assert.AreEqual(10, target.SimpleNumber);

        [Test]
        public void ShouldReturnValidStringWhenSimple() => Assert.AreEqual("Test", target.SimpleString);
        
        [Test]
        public void ShouldReturnValidNumberWhenComputed() => Assert.AreEqual(50, target.ComputedNumber);

        [Test]
        public void ShouldReturnValidStringWhenComputed() => Assert.AreEqual("Test 1 Value", target.ComputedString);

        [Test]
        public void ShouldReturnValidNumberWhenComplex() => Assert.AreEqual(90, target.ComplexNumber);

        [Test]
        public void ShouldReturnValidStringWhenComplex() => Assert.AreEqual("Number 90", target.ComplexString);

        [Test]
        public void ShouldReturnValidMethodNumberWhenComputed() => Assert.AreEqual(60, target.GetComputedNumber());

        [Test]
        public void ShouldReturnValidMethodStringWhenComputed() => Assert.AreEqual("Testing 30 Result", target.GetComputedString());

        [Test]
        public void ShouldReturnValidXmlAttributeNumber() => Assert.AreEqual(123, target.XmlAttributeNumber);

        [Test]
        public void ShouldReturnValidXmlElementString() => Assert.AreEqual("Hello", target.XmlElementString);
    }
}
