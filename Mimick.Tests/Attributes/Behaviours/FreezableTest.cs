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
    public class FreezableTest
    {
        [Test]
        public void ShouldImplementIFreezable() => Assert.IsNotNull(new FreezableAttribute() is IFreezable);

        [Test]
        public void ShouldAllowSettersWhenNotFrozen()
        {
            var target = new FreezableAttributes();

            target.Id = 1;
            target.Name = "Test";
        }

        [Test]
        public void ShouldThrowExceptionOnSettersWhenFrozen()
        {
            var target = new FreezableAttributes();
            ((IFreezable)target).Freeze();

            Assert.Throws(typeof(FrozenException), () => target.Id = 1);
        }
    }
}
