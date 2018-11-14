using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using AssemblyToProcess.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mimick.Configurations;

namespace Mimick.Tests
{
    [TestClass]
    public class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void BeforeAssembly(TestContext context)
        {
            var framework = FrameworkContext.Current;

            framework
                .ComponentContext
                .RegisterAssembly<AdhocComponent>();

            framework
                .ConfigurationContext
                .Register<AppConfigurationSource>();

            framework
                .ConfigurationContext
                .Register(new XmlConfigurationSource("Configuration.xml"));

            framework
                .Initialize();
        }
    }
}
