using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes.Contracts;
using NUnit.Framework;

namespace Mimick.Tests.Attributes.Contracts
{
    [TestFixture]
    public class NotEmptyTest
    {
        private NotEmptyAttributes target;

        [OneTimeSetUp]
        public void Initialize() => target = new NotEmptyAttributes();

        [Test]
        public void ShouldPassWhenNotEmptyIsNotPresent()
        {
            target.PassIfEmpty((string)null);
            target.PassIfEmpty((ICollection<int>)null);
            target.PassIfEmpty((IEnumerable<int>)null);
        }

        [Test]
        public void ShouldFailWhenNotEmptyStringIsPassedNull() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfEmpty((string)null));

        [Test]
        public void ShouldFailWhenNotEmptyStringIsPassedEmpty() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfEmpty(""));

        [Test]
        public void ShouldPassWhenNotEmptyStringIsPassedValid() => target.ThrowIfEmpty("Test");

        [Test]
        public void ShouldFailWhenNotEmptyCollectionIsPassedNull() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfEmpty((ICollection<int>)null));

        [Test]
        public void ShouldFailWhenNotEmptyCollectionIsPassedEmpty() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfEmpty(new List<int>()));

        [Test]
        public void ShouldPassWhenNotEmptyCollectionIsPassedValid() => target.ThrowIfEmpty(new List<int>(new[] { 1, 2, 3 }));

        [Test]
        public void ShouldFailWhenNotEmptyEnumerableIsPassedNull() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfEmpty((IEnumerable<int>)null));

        [Test]
        public void ShouldFailWhenNotEmptyEnumerableIsPassedEmpty() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfEmpty((IEnumerable<int>)new int[0]));

        [Test]
        public void ShouldPassWhenNotEmptyEnumerableIsPassedValid() => target.ThrowIfEmpty((IEnumerable<int>)new[] { 1, 2, 3 });

        [Test]
        public void ShouldFailWhenNotEmptyMethodIsPassedEmpty() => Assert.Throws(typeof(ArgumentException), () => target.ThrowIfAnyEmpty("Test", ""));
    }
}
