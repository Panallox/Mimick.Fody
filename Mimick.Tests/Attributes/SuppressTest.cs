using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Mimick.Tests.Attributes
{
    [TestFixture]
    public class SuppressTest
    {
        [Test]
        public void ShouldThrowExceptionWhenNotSuppressed() => Assert.Throws<Exception>(() => new SuppressAttributes().ThrowException());

        [Test]
        public void ShouldNotThrowExceptionWhenSuppressed() => new SuppressAttributes().ThrowAndSuppressException();

        [Test]
        public void ShouldThrowExceptionWhenSuppressedAndNotInFilter() => Assert.Throws<InvalidOperationException>(() => new SuppressAttributes().ThrowAndFilterException());

        [Test]
        public void ShouldNotThrowExceptionWhenSuppressedAndInFilter() => new SuppressAttributes().ThrowAndFilterAndSuppressException();

        [Test]
        public void ShouldReturnDefaultWhenExceptionIsSuppressed() => Assert.AreEqual(default(int), new SuppressAttributes().ThrowAndSuppressExceptionAndReturnDefault());
    }
}
