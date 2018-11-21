using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Framework;
using NUnit.Framework;

namespace Mimick.Tests.Framework
{
    [TestFixture]
    public class ComponentTest
    {
        private IComponentContext container;

        [SetUp]
        public void BeforeClass() => container = FrameworkContext.Current.ComponentContext;

        [Test]
        public void ShouldResolveByConcreteType() => Assert.IsNotNull(container.Resolve<ImplementedComponent>());

        [Test]
        public void ShouldResolveByInterfaceType() => Assert.IsNotNull(container.Resolve<IImplementedComponent>());

        [Test]
        public void ShouldResolveByName() => Assert.IsNotNull(container.Resolve("AlternativeNamedComponent"));

        [Test]
        public void ShouldInstantiateForAdhoc()
        {
            var component1 = container.Resolve<AdhocComponent>();
            var component2 = container.Resolve<AdhocComponent>();

            Assert.IsNotNull(component1);
            Assert.IsNotNull(component2);
            Assert.AreNotEqual(component1.Guid, component2.Guid);
        }

        [Test]
        public void ShouldRetrieveForSingleton()
        {
            var component1 = container.Resolve<SingletonComponent>();
            var component2 = container.Resolve<SingletonComponent>();

            Assert.IsNotNull(component1);
            Assert.IsNotNull(component2);
            Assert.AreEqual(component1.Guid, component2.Guid);
        }

        [Test]
        public void ShouldRetrieveForConfigurationComponent() => Assert.IsNotNull(container.Resolve<ConfiguredComponent>());
    }
}
