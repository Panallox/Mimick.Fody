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
    public class CachedTest
    {
        [TestMethod]
        public void ShouldReturnDifferentValueWhenNotCached()
        {
            var target = new CachedAttributes();

            Assert.AreNotEqual(target.NonCached(), target.NonCached());
        }

        [TestMethod]
        public void ShouldReturnSameValueWhenCached()
        {
            var target = new CachedAttributes();

            Assert.AreEqual(target.Cached(), target.Cached());
        }

        [TestMethod]
        public void ShouldReturnDifferentValueWhenCachedWithDifferentArguments()
        {
            var target = new CachedAttributes();

            Assert.AreNotEqual(target.CachedArgument(1), target.CachedArgument(2));
        }

        [TestMethod]
        public void ShouldReturnSameValueWhenCachedWithSameArguments()
        {
            var target = new CachedAttributes();

            Assert.AreEqual(target.CachedArgument(123), target.CachedArgument(123));
        }
    }
}
