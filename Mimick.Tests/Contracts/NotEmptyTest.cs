using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests.Contracts
{
    [TestClass]
    public class NotEmptyTest
    {
        private NotEmptyContracts target;

        [TestInitialize]
        public void Initialize() => target = new NotEmptyContracts();

        [TestMethod]
        public void ShouldPassWhenNotEmptyIsNotPresent()
        {
            target.PassIfEmpty((string)null);
            target.PassIfEmpty((ICollection<int>)null);
            target.PassIfEmpty((IEnumerable<int>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyStringIsPassedNull() => target.ThrowIfEmpty((string)null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyStringIsPassedEmpty() => target.ThrowIfEmpty("");

        [TestMethod]
        public void ShouldPassWhenNotEmptyStringIsPassedValid() => target.ThrowIfEmpty("Test");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyCollectionIsPassedNull() => target.ThrowIfEmpty((ICollection<int>)null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyCollectionIsPassedEmpty() => target.ThrowIfEmpty(new List<int>());

        [TestMethod]
        public void ShouldPassWhenNotEmptyCollectionIsPassedValid() => target.ThrowIfEmpty(new List<int>(new[] { 1, 2, 3 }));
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyEnumerableIsPassedNull() => target.ThrowIfEmpty((IEnumerable<int>)null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyEnumerableIsPassedEmpty() => target.ThrowIfEmpty((IEnumerable<int>)new int[0]);

        [TestMethod]
        public void ShouldPassWhenNotEmptyEnumerableIsPassedValid() => target.ThrowIfEmpty((IEnumerable<int>)new[] { 1, 2, 3 });

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldFailWhenNotEmptyMethodIsPassedEmpty() => target.ThrowIfAnyEmpty("Test", "");
    }
}
