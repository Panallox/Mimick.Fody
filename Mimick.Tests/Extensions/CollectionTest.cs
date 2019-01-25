using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests.Extensions
{
    [TestFixture]
    public class CollectionTest
    {
        private static readonly int[] Numbers = { 1, 2, 3, 4 };
        private static readonly List<int> ListFilled = new List<int>(Numbers);
        private static readonly List<int> ListEmpty = new List<int>();

        [Test]
        public void AddIfMissingListShouldNotAddIfExists()
        {
            var list = new List<int>(Numbers);
            list.AddIfMissing(1);

            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void AddIfMissingListShouldAddIfNotFound()
        {
            var list = new List<int>(Numbers);
            list.AddIfMissing(5);

            Assert.AreEqual(5, list.Count);
            Assert.IsTrue(list.Contains(5));
        }

        [Test]
        public void AddIfMissingDictionaryShouldNotAddIfExists()
        {
            var dictionary = new Dictionary<string, int>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            dictionary.AddIfMissing("1", 3);

            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual(1, dictionary["1"]);
        }

        [Test]
        public void AddIfMissingDictionaryShouldAddIfNotFound()
        {
            var dictionary = new Dictionary<string, int>();
            dictionary.Add("1", 1);
            dictionary.Add("2", 2);
            dictionary.AddIfMissing("3", 3);

            Assert.AreEqual(3, dictionary.Count);
            Assert.AreEqual(3, dictionary["3"]);
        }

        [Test]
        public void ContainsAllShouldReturnFalseIfOneMissing() => Assert.IsFalse(ListFilled.ContainsAll(1, 3, 5));

        [Test]
        public void ContainsAllShouldReturnTrueIfAllExist() => Assert.IsTrue(ListFilled.ContainsAll(1, 3));

        [Test]
        public void ContainsAnyShouldReturnFalseIsNoneExist() => Assert.IsFalse(ListFilled.ContainsAny(5, 6, 7));

        [Test]
        public void ContainsAnyShouldReturnTrueIfOneExists() => Assert.IsTrue(ListFilled.ContainsAny(4));

        [Test]
        public void CountShouldReturnZeroIfNull() => Assert.AreEqual(0, ((IEnumerable)null).Count());

        [Test]
        public void CountShouldReturnValidIfPassedValid() => Assert.AreEqual(4, ((IEnumerable)ListFilled).Count());
        
        [Test]
        public void IsEmptyShouldReturnTrueIfNoItems() => Assert.IsTrue(ListEmpty.IsEmpty());

        [Test]
        public void IsEmptyShouldReturnFalseIfHasItems() => Assert.IsFalse(ListFilled.IsEmpty());

        [Test]
        public void IsNotEmptyShouldReturnFalseIfNoItems() => Assert.IsFalse(ListEmpty.IsNotEmpty());

        [Test]
        public void IsNotEmptyShouldReturnTrueIfHasItems() => Assert.IsTrue(ListFilled.IsNotEmpty());

        [Test]
        public void RemoveRangeShouldRemoveItems()
        {
            var list = new List<int>(Numbers);
            var remove = new List<int>(new[] { 2, 3 });

            list.RemoveRange(remove);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(4, list[1]);
        }

        [Test]
        public void ToReadOnlyShouldReturnReadOnlyList()
        {
            var list = ListFilled.ToReadOnly();
            
            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
        }

    }
}
