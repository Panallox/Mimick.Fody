using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using NUnit.Framework;

namespace Mimick.Tests.Attributes
{
    [TestFixture]
    public class AutowireTest
    {
        private AutowireAttributes target;

        [SetUp]
        public void SetUp() => target = new AutowireAttributes();

        [Test]
        public void ShouldReturnValueWhenFieldIsAutowired() => Assert.NotNull(target.autoField);

        [Test]
        public void ShouldReturnValueWhenPropertyIsAutowired() => Assert.NotNull(target.AutoProperty);

        [Test]
        public void ShouldReturnValueWhenMethodParameterIsAutowired() => Assert.NotNull(target.AutoMethod());

        [Test]
        public void ShouldReturnValueWhenMethodAnyParameterIsAutowired() => Assert.NotNull(target.AutoAnyMethod());

        [Test]
        public void ShouldReturnValueWhenPrivateFieldIsAutowired() => Assert.NotNull(target.GetAutoPrivateField());
    }
}
