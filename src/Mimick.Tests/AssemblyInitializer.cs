using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mimick.Tests
{
    [TestClass]
    public class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void BeforeAssembly(TestContext context)
        {
            var configuration = FrameworkConfiguration
                .Begin()
                .Assemblies(x =>
                    x.Add(Assemblies.Of<ConstructAttributes>()))
                .Configurations(x =>
                    x.Add(Configurations.AppConfig));

            FrameworkContext.Configure(configuration);
        }
    }
}
