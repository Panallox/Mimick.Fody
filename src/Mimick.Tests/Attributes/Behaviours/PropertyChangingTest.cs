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
    public class PropertyChangingTest
    {
        [TestMethod]
        public void ShouldImplementINotifyPropertyChanging()
        {
            var target = new PropertyChangingAttributes();

            Assert.IsNotNull(target as INotifyPropertyChanging);
        }

        [TestMethod]
        public void ShouldRaiseEventWhenPropertyChanging()
        {
            var target = new PropertyChangingAttributes();
            var raised = false;

            ((INotifyPropertyChanging)target).PropertyChanging += (sender, e) => raised = true;
            target.Text = "Hello world";

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void ShouldNotRaiseEventWhenPropertyChangingAndIsIgnored()
        {
            var target = new PropertyChangingAttributes();
            var raised = false;

            ((INotifyPropertyChanging)target).PropertyChanging += (sender, e) => raised = true;
            target.Id = 2000;

            Assert.IsFalse(raised);
        }
    }
}
