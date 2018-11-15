using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests.Attributes.Contracts
{
    [TestClass]
    public class NotNullTest
    {
        private NotNullAttributes target;

        [TestInitialize]
        public void Initialize() => target = new NotNullAttributes();

        [TestMethod]
        public void ShouldPassWhenNotNullIsNotPresent() => target.PassIfNull(null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailWhenNotNullArgumentIsPassedNull() => target.ThrowIfNull(null);

        [TestMethod]
        public void ShouldPassWhenNotNullArgumentIsPassedValid() => target.ThrowIfNull(new object());

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldFailWhenNotNullMethodIsPassedNull() => target.ThrowIfAnyNull(new object(), null);

        [TestMethod]
        public void ShouldPassWhenNotNullMethodIsPassedValid() => target.ThrowIfAnyNull(new object(), new object());
    }
}
