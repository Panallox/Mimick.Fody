using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests.Extensions
{
    [TestFixture]
    public class StringTest
    {
        private const string Null = null;

        [Test]
        public void IsAnyShouldReturnFalseWhenNotInList() => Assert.IsFalse("a".IsAny("b", "c"));

        [Test]
        public void IsAnyShouldReturnTrueWhenInList() => Assert.IsTrue("a".IsAny("a", "b"));

        [Test]
        public void IsBlankShouldReturnTrueWhenNull() => Assert.IsTrue(Null.IsBlank());

        [Test]
        public void IsBlankShouldReturnTrueWhenWhitespace() => Assert.IsTrue("  ".IsBlank());

        [Test]
        public void IsBlankShouldReturnFalseWhenNonWhitespace() => Assert.IsFalse(" a ".IsBlank());

        [Test]
        public void IsEmptyShouldReturnTrueWhenNull() => Assert.IsTrue(Null.IsEmpty());

        [Test]
        public void IsEmptyShouldReturnTrueWhenEmpty() => Assert.IsTrue("".IsEmpty());

        [Test]
        public void IsEmptyShouldReturnFalseWhenNonEmpty() => Assert.IsFalse(" a ".IsEmpty());

        [Test]
        public void IsNotBlankShouldReturnFalseWhenNull() => Assert.IsFalse(Null.IsNotBlank());

        [Test]
        public void IsNotBlankShouldReturnFalseWhenWhitespace() => Assert.IsFalse("  ".IsNotBlank());

        [Test]
        public void IsNotBlankShouldReturnTrueWhenNonWhitespace() => Assert.IsTrue(" a ".IsNotBlank());

        [Test]
        public void IsNotEmptyShouldReturnFalseWhenNull() => Assert.IsFalse(Null.IsNotEmpty());

        [Test]
        public void IsNotEmptyShouldReturnFalseWhenEmpty() => Assert.IsFalse("".IsNotEmpty());

        [Test]
        public void IsNotEmptyShouldReturnTrueWhenNonEmpty() => Assert.IsTrue(" a ".IsNotEmpty());

        [Test]
        public void IsNumericShouldReturnFalseWhenNull() => Assert.IsFalse(Null.IsNumeric());

        [Test]
        public void IsNumericShouldReturnFalseWhenEmpty() => Assert.IsFalse("".IsNumeric());

        [Test]
        public void IsNumericShouldReturnFalseWhenNonNumeric() => Assert.IsFalse("abc".IsNumeric());

        [Test]
        public void IsNumericShouldReturnTrueWhenNumeric() => Assert.IsTrue("1234".IsNumeric());

        [Test]
        public void LeftShouldReturnValidValue() => Assert.AreEqual("abc", "abcdef".Left(3));

        [Test]
        public void RepeatShouldReturnSameValueIfCountIsOne() => Assert.AreEqual("abc", "abc".Repeat(1));

        [Test]
        public void RepeatShouldReturnValidValue() => Assert.AreEqual("abcabcabc", "abc".Repeat(3));

        [Test]
        public void RightShouldReturnValidValue() => Assert.AreEqual("def", "abcdef".Right(3));

        [Test]
        public void ToByteArrayShouldReturnValidValue()
        {
            var target = "abcdef";
            var expected = Encoding.ASCII.GetBytes(target);
            var actual = target.ToByteArray();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToEnumShouldReturnValidValue() => Assert.AreEqual(DayOfWeek.Thursday, "Thursday".ToEnum<DayOfWeek>());
        
        [Test]
        public void TrimToNullShouldReturnNullWhenNull() => Assert.IsNull(Null.TrimToNull());

        [Test]
        public void TrimToNullShouldReturnNullWhenEmpty() => Assert.IsNull("  ".TrimToNull());

        [Test]
        public void TrimToNullShouldReturnValidValueWhenNonEMpty() => Assert.AreEqual("abc", "  abc  ".TrimToNull());
    }
}
