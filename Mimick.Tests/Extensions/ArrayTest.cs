using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests.Extensions
{
    [TestFixture]
    public class ArrayTest
    {
        private static readonly int[] Values = { 1, 2, 3, 4, 5 };

        [Test]
        public void CopyShouldCopyAllValues()
        {
            var copy = Values.Copy();

            Assert.AreEqual(Values.Length, copy.Length);
            Assert.AreEqual(Values[0], copy[0]);
            Assert.AreEqual(Values[1], copy[1]);
            Assert.AreEqual(Values[2], copy[2]);
            Assert.AreEqual(Values[3], copy[3]);
            Assert.AreEqual(Values[4], copy[4]);
        }

        [Test]
        public void CopyShouldCopySubsetWithIndex()
        {
            var copy = Values.Copy(2);

            Assert.AreEqual(3, copy.Length);
            Assert.AreEqual(Values[2], copy[0]);
            Assert.AreEqual(Values[3], copy[1]);
            Assert.AreEqual(Values[4], copy[2]);
        }

        [Test]
        public void CopyShouldCopySubsetWithIndexAndCount()
        {
            var copy = Values.Copy(2, 2);

            Assert.AreEqual(2, copy.Length);
            Assert.AreEqual(Values[2], copy[0]);
            Assert.AreEqual(Values[3], copy[1]);
        }

        [Test]
        public void FillShouldFillAllValues()
        {
            var array = new int[3];
            array.Fill(1);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(1, array[2]);
        }

        [Test]
        public void FillShouldFillSubsetWithIndexAndCount()
        {
            var array = new int[5];
            array.Fill(1, 2, 2);

            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(0, array[1]);
            Assert.AreEqual(1, array[2]);
            Assert.AreEqual(1, array[3]);
            Assert.AreEqual(0, array[4]);
        }

        [Test]
        public void SliceShouldReturnSubsetWithIndex()
        {
            var slice = Values.Slice(2);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual(Values[2], slice[0]);
            Assert.AreEqual(Values[3], slice[1]);
            Assert.AreEqual(Values[4], slice[2]);
        }

        [Test]
        public void SliceShouldReturnSubsetWithIndexAndCount()
        {
            var slice = Values.Slice(1, 3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual(Values[1], slice[0]);
            Assert.AreEqual(Values[2], slice[1]);
            Assert.AreEqual(Values[3], slice[2]);
        }
    }
}
