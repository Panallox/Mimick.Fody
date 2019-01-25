using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests.Extensions
{
    [TestFixture]
    public class DateTest
    {
        [Test]
        public void IsFutureShouldReturnFalseIfInThePast() => Assert.IsFalse(DateTime.Now.AddDays(-1).IsFuture());

        [Test]
        public void IsFutureShouldReturnTrueIfInTheFuture() => Assert.IsTrue(DateTime.Now.AddDays(1).IsFuture());

        [Test]
        public void IsPastShouldReturnTrueIfInThePast() => Assert.IsTrue(DateTime.Now.AddDays(-1).IsPast());

        [Test]
        public void IsPastShouldReturnFalseIfInTheFuture() => Assert.IsFalse(DateTime.Now.AddDays(1).IsPast());
    }
}
