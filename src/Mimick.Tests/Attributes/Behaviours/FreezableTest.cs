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
    public class FreezableTest
    {
        [TestMethod]
        public void ShouldImplementIFreezable() => Assert.IsNotNull(new FreezableAttribute() is IFreezable);

        [TestMethod]
        public void ShouldAllowSettersWhenNotFrozen()
        {
            var target = new FreezableAttributes();

            target.Id = 1;
            target.Name = "Test";
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowExceptionOnSettersWhenFrozen()
        {
            var target = new FreezableAttributes();
            ((IFreezable)target).Freeze();

            target.Id = 1;
        }
    }
}
