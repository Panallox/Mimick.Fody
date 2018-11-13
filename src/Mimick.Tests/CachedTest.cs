using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests
{
    [TestClass]
    public class CachedTest
    {
        private CachedMethods target;

        [TestInitialize]
        public void Initialize() => target = new CachedMethods();

        [TestMethod]
        public void ShouldReturnDifferentValueWhenNotCached() => Assert.AreNotEqual(target.NonCached(), target.NonCached());

        [TestMethod]
        public void ShouldReturnSameValueWhenCached() => Assert.AreEqual(target.Cached(), target.Cached());

        [TestMethod]
        public void ShouldReturnDifferentValueWhenCachedWithDifferentArguments() => Assert.AreNotEqual(target.CachedArgument(1), target.CachedArgument(2));

        [TestMethod]
        public void ShouldReturnSameValueWhenCachedWithSameArguments() => Assert.AreEqual(target.CachedArgument(123), target.CachedArgument(123));
    }
}
