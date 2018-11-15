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
    public class PropertyChangedTest
    {
        [Test]
        public void ShouldImplementINotifyPropertyChanged()
        {
            var target = new PropertyChangedAttributes();

            Assert.NotNull(target as INotifyPropertyChanged);
        }

        [Test]
        public void ShouldRaiseEventWhenPropertyChanged()
        {
            var target = new PropertyChangedAttributes();
            var raised = false;

            ((INotifyPropertyChanged)target).PropertyChanged += (sender, e) => raised = true;
            target.Text = "Hello world";

            Assert.True(raised);
        }

        [Test]
        public void ShouldNotRaiseEventWhenPropertyChangedAndIsIgnored()
        {
            var target = new PropertyChangedAttributes();
            var raised = false;

            ((INotifyPropertyChanged)target).PropertyChanged += (sender, e) => raised = true;
            target.Id = 2000;

            Assert.False(raised);
        }
    }
}
