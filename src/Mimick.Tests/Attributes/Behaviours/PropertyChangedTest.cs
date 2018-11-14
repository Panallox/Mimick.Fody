using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Behaviours;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests.Attributes.Behaviours
{
    [TestClass]
    public class PropertyChangedTest
    {
        [TestMethod]
        public void ShouldImplementINotifyPropertyChanged()
        {
            var target = new PropertyChangedAttributes();

            Assert.IsNotNull(target as INotifyPropertyChanged);
        }

        [TestMethod]
        public void ShouldRaiseEventWhenPropertyChanged()
        {
            var target = new PropertyChangedAttributes();
            var raised = false;

            ((INotifyPropertyChanged)target).PropertyChanged += (sender, e) => raised = true;
            target.Text = "Hello world";

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void ShouldNotRaiseEventWhenPropertyChangedAndIsIgnored()
        {
            var target = new PropertyChangedAttributes();
            var raised = false;

            ((INotifyPropertyChanged)target).PropertyChanged += (sender, e) => raised = true;
            target.Id = 2000;

            Assert.IsFalse(raised);
        }
    }
}
