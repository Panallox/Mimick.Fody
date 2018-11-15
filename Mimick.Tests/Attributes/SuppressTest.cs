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
    public class SuppressTest
    {
        private static SuppressAttributes target;

        [ClassInitialize]
        public static void BeforeClass(TestContext context) => target = new SuppressAttributes();

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionWhenNotSuppressed() => target.ThrowException();

        [TestMethod]
        public void ShouldNotThrowExceptionWhenSuppressed() => target.ThrowAndSuppressException();

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowExceptionWhenSuppressedAndNotInFilter() => target.ThrowAndFilterException();

        [TestMethod]
        public void ShouldNotThrowExceptionWhenSuppressedAndInFilter() => target.ThrowAndFilterAndSuppressException();

        [TestMethod]
        public void ShouldReturnDefaultWhenExceptionIsSuppressed() => Assert.AreEqual(default(int), target.ThrowAndSuppressExceptionAndReturnDefault());
    }
}
