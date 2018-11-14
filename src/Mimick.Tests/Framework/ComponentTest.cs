using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests.Framework
{
    [TestClass]
    public class ComponentTest
    {
        private static IDependencyContext container;

        [ClassInitialize]
        public static void BeforeClass(TestContext context) => container = FrameworkContext.Instance.Dependencies;

        [TestMethod]
        public void ShouldResolveByConcreteType() => Assert.IsNotNull(container.Resolve<ImplementedComponent>());

        [TestMethod]
        public void ShouldResolveByInterfaceType() => Assert.IsNotNull(container.Resolve<IImplementedComponent>());

        [TestMethod]
        public void ShouldResolveByName() => Assert.IsNotNull(container.Resolve("AlternativeNamedComponent"));

        [TestMethod]
        public void ShouldInstantiateForAdhoc()
        {
            var component1 = container.Resolve<AdhocComponent>();
            var component2 = container.Resolve<AdhocComponent>();

            Assert.IsNotNull(component1);
            Assert.IsNotNull(component2);
            Assert.AreNotEqual(component1.Guid, component2.Guid);
        }

        [TestMethod]
        public void ShouldRetrieveForSingleton()
        {
            var component1 = container.Resolve<SingletonComponent>();
            var component2 = container.Resolve<SingletonComponent>();

            Assert.IsNotNull(component1);
            Assert.IsNotNull(component2);
            Assert.AreEqual(component1.Guid, component2.Guid);
        }
    }
}
