using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Behaviours;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests.Attributes.Behaviours
{
    [TestClass]
    public class DisposableTest
    {
        [TestMethod]
        public void ShouldImplementIDisposable()
        {
            var target = new DisposableAttributes();

            Assert.IsNotNull(target as IDisposable);
        }

        [TestMethod]
        public void ShouldInvokeAllDisposeAssociatedMethods()
        {
            var target = new DisposableAttributes();
            ((IDisposable)target).Dispose();

            Assert.AreEqual(2, target.DisposeCount);
        }

        [TestMethod]
        public void ShouldInvokeAllDisposeAssociatedMethodsOnlyOnce()
        {
            var target = new DisposableAttributes();

            ((IDisposable)target).Dispose();
            ((IDisposable)target).Dispose();
            Assert.AreEqual(2, target.DisposeCount);
        }

        [TestMethod]
        public void ShouldSetIsDisposeToTrue()
        {
            var target = new DisposableAttributes();

            ((IDisposable)target).Dispose();
            Assert.IsTrue(((IDisposableAndTracked)target).IsDisposed);
        }
    }
}
