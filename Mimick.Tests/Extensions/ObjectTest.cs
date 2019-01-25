using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests.Extensions
{
    [TestFixture]
    public class ObjectTest
    {
        private static readonly object NotNull = new object();
        private static readonly object Null = null;

        [Test]
        public void IfNotNullShouldNotInvokeIfNull() => Null.IfNotNull(() => Assert.Fail());

        [Test]
        public void IfNotNullShouldInvokeIfNotNull()
        {
            NotNull.IfNotNull(() => Assert.Pass());
            Assert.Fail();
        }

        [Test]
        public void IfNullShouldInvokeIfNull()
        {
            Null.IfNull(() => Assert.Pass());
            Assert.Fail();
        }

        [Test]
        public void IfNullShouldNotInvokeIfNotNull() => NotNull.IfNull(() => Assert.Fail());

        [Test]
        public void IsNotNullShouldReturnFalseIfNull() => Assert.IsFalse(Null.IsNotNull());

        [Test]
        public void IsNotNullShouldReturnTrueIfNotNull() => Assert.IsTrue(NotNull.IsNotNull());

        [Test]
        public void IsNullShouldReturnTrueIfNull() => Assert.IsTrue(Null.IsNull());

        [Test]
        public void IsNullShouldReturnFalseIfNotNull() => Assert.IsFalse(NotNull.IsNull());
    }
}
