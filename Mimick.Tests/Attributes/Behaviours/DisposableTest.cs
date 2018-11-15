using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Behaviours;
using NUnit.Framework;

namespace Mimick.Tests.Attributes.Behaviours
{
    [TestFixture]
    public class DisposableTest
    {
        [Test]
        public void ShouldImplementIDisposable()
        {
            var target = new DisposableAttributes();

            Assert.IsNotNull(target as IDisposable);
        }

        [Test]
        public void ShouldInvokeAllDisposeAssociatedMethods()
        {
            var target = new DisposableAttributes();
            ((IDisposable)target).Dispose();

            Assert.AreEqual(2, target.DisposeCount);
        }

        [Test]
        public void ShouldInvokeAllDisposeAssociatedMethodsOnlyOnce()
        {
            var target = new DisposableAttributes();

            ((IDisposable)target).Dispose();
            ((IDisposable)target).Dispose();
            Assert.AreEqual(2, target.DisposeCount);
        }

        [Test]
        public void ShouldSetIsDisposeToTrue()
        {
            var target = new DisposableAttributes();

            ((IDisposable)target).Dispose();
            Assert.IsTrue(((IDisposableAndTracked)target).IsDisposed);
        }
    }
}
