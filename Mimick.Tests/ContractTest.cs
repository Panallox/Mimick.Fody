using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mimick.Tests
{
    [TestFixture]
    public class ContractTest
    {
        [Test]
        public void ShouldThrowWhenEqualPassedNullAndValidExpected() => Assert.Throws<InvalidValueException>(() => Contract.Equal(null, "abc"));

        [Test]
        public void ShouldThrowWhenEqualPassedValueAndNullExpected() => Assert.Throws<InvalidValueException>(() => Contract.Equal("abc", null));

        [Test]
        public void ShouldThrowWhenEqualPassedNonEqualValues() => Assert.Throws<InvalidValueException>(() => Contract.Equal(1, 2));

        [Test]
        public void ShouldPassWhenEqualPassedNullAndNullExpected() => Contract.Equal((object)null, null);

        [Test]
        public void ShouldPassWhenEqualPassedValueAndValidExpected() => Contract.Equal(1, 1);

        [Test]
        public void ShouldThrowWhenGreaterThanPassedLessThan() => Assert.Throws<InvalidValueException>(() => Contract.GreaterThan(10, 20));

        [Test]
        public void ShouldThrowWhenGreaterThanPassedEqual() => Assert.Throws<InvalidValueException>(() => Contract.GreaterThan(10, 10));

        [Test]
        public void ShouldPassWhenGreaterThanPassedValid() => Contract.GreaterThan(10, 5);

        [Test]
        public void ShouldThrowWhenGreaterThanEqualPassedLessThan() => Assert.Throws<InvalidValueException>(() => Contract.GreaterThanEqual(10, 20));

        [Test]
        public void ShouldPassWhenGreaterThanEqualPassedEqual() => Contract.GreaterThanEqual(10, 10);

        [Test]
        public void ShouldPassWhenGreaterThanEqualPassedGreaterThan() => Contract.GreaterThanEqual(10, 5);

        [Test]
        public void ShouldThrowWhenLessThanPassedGreaterThan() => Assert.Throws<InvalidValueException>(() => Contract.LessThan(10, 5));

        [Test]
        public void ShouldThrowWhenLessThanPassedEqual() => Assert.Throws<InvalidValueException>(() => Contract.LessThan(10, 10));

        [Test]
        public void ShouldPassWhenLessThanPassedLessThan() => Contract.LessThan(10, 20);

        [Test]
        public void ShouldThrowWhenLessThanEqualPassedGreaterThan() => Assert.Throws<InvalidValueException>(() => Contract.LessThanEqual(10, 5));

        [Test]
        public void ShouldPassWhenLessThanEqualPassedEqual() => Contract.LessThanEqual(10, 10);

        [Test]
        public void ShouldPassWhenLessThanEqualPassedLessThan() => Contract.LessThanEqual(10, 20);

        [Test]
        public void ShouldThrowWhenNotBlankPassedNull() => Assert.Throws<EmptyException>(() => Contract.NotBlank((string)null));

        [Test]
        public void ShouldThrowWhenNotBlankPassedEmpty() => Assert.Throws<EmptyException>(() => Contract.NotBlank(string.Empty));

        [Test]
        public void ShouldThrowWhenNotBlankPassedWhiteSpace() => Assert.Throws<EmptyException>(() => Contract.NotBlank("  "));

        [Test]
        public void ShouldPassWhenNotBlankPassedValid() => Contract.NotBlank("abc");

        [Test]
        public void ShouldThrowWhenNotEmptyArrayPassedNull() => Assert.Throws<EmptyException>(() => Contract.NotEmpty((int[])null));

        [Test]
        public void ShouldThrowWhenNotEmptyArrayPassedEmpty() => Assert.Throws<EmptyException>(() => Contract.NotEmpty(new int[0]));

        [Test]
        public void ShouldPassWhenNotEmptyArrayPassedValid() => Contract.NotEmpty(new int[] { 1, 2 });

        [Test]
        public void ShouldThrowWhenNotEmptyCollectionPassedNull() => Assert.Throws<EmptyException>(() => Contract.NotEmpty((List<int>)null));

        [Test]
        public void ShouldThrowWhenNotEmptyCollectionPassedEmpty() => Assert.Throws<EmptyException>(() => Contract.NotEmpty(new List<int>()));

        [Test]
        public void ShouldPassWhenNotEmptyCollectionPassedValid() => Contract.NotEmpty(new List<int>(new[] { 1, 2 }));

        [Test]
        public void ShouldThrowWhenNotEmptyStringPassedNull() => Assert.Throws<EmptyException>(() => Contract.NotEmpty((string)null));

        [Test]
        public void ShouldThrowWhenNotEmptyStringPassedEmpty() => Assert.Throws<EmptyException>(() => Contract.NotEmpty(string.Empty));

        [Test]
        public void ShouldPassWhenNotEmptyStringPassedValid() => Contract.NotEmpty("abc");

        [Test]
        public void ShouldThrowWhenNotEqualPassedNullAndNullComparison() => Assert.Throws<InvalidValueException>(() => Contract.NotEqual((object)null, null));

        [Test]
        public void ShouldThrowWhenNotEqualPassedValueAndEqualComparison() => Assert.Throws<InvalidValueException>(() => Contract.NotEqual(1, 1));

        [Test]
        public void ShouldPassWhenNotEqualPassedNullAndValidComparison() => Contract.NotEqual(null, "abc");

        [Test]
        public void ShouldPassWhenNotEqualPassedValueAndNullComparison() => Contract.NotEqual("abc", null);

        [Test]
        public void ShouldPassWhenNotEqualPassedValueAndDifferentComparison() => Contract.NotEqual("abc", "def");

        [Test]
        public void ShouldThrowWhenNotNullPassedNull() => Assert.Throws<NullReferenceException>(() => Contract.NotNull((object)null));

        [Test]
        public void ShouldPassWhenNotNullPassedValid() => Contract.NotNull(new object());
    }
}
