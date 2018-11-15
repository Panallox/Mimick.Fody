using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Behaviours;
using NUnit.Framework;

namespace Mimick.Tests.Attributes.Behaviours
{
    [TestFixture]
    public class PropertyChangingTest
    {
        [Test]
        public void ShouldImplementINotifyPropertyChanging()
        {
            var target = new PropertyChangingAttributes();

            Assert.NotNull(target as INotifyPropertyChanging);
        }

        [Test]
        public void ShouldRaiseEventWhenPropertyChanging()
        {
            var target = new PropertyChangingAttributes();
            var raised = false;

            ((INotifyPropertyChanging)target).PropertyChanging += (sender, e) => raised = true;
            target.Text = "Hello world";

            Assert.True(raised);
        }

        [Test]
        public void ShouldNotRaiseEventWhenPropertyChangingAndIsIgnored()
        {
            var target = new PropertyChangingAttributes();
            var raised = false;

            ((INotifyPropertyChanging)target).PropertyChanging += (sender, e) => raised = true;
            target.Id = 2000;

            Assert.False(raised);
        }
    }
}
