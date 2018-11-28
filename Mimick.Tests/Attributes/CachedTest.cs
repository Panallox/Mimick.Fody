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
    public class CachedTest
    {
        [Test]
        public void ShouldReturnDifferentValueWhenNotCached()
        {
            var target = new CachedAttributes();

            Assert.AreNotEqual(target.NonCached(), target.NonCached());
        }

        [Test]
        public void ShouldReturnSameValueWhenCached()
        {
            var target = new CachedAttributes();

            Assert.AreEqual(target.Cached(), target.Cached());
        }

        [Test]
        public void ShouldReturnDifferentValueWhenCachedWithDifferentArguments()
        {
            var target = new CachedAttributes();

            Assert.AreNotEqual(target.CachedArgument(1), target.CachedArgument(2));
        }

        [Test]
        public void ShouldReturnSameValueWhenCachedWithSameArguments()
        {
            var target = new CachedAttributes();

            Assert.AreEqual(target.CachedArgument(123), target.CachedArgument(123));
        }

        [Test]
        public void ShouldReturnSameValueWhenCachedWithDifferentByRefArgument()
        {
            int a = 1;
            int b = 2;
            var target = new CachedAttributes();

            Assert.AreEqual(target.CachedIgnoringByRef(10, ref a), target.CachedIgnoringByRef(10, ref b));
        }
    }
}
