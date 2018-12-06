using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Attributes;
using AssemblyToProcess.Framework;
using Mimick.Configurations;
using NUnit.Framework;

namespace Mimick.Tests
{
    [SetUpFixture]
    public class AssemblyInitializer
    {
        [OneTimeSetUp]
        public static void SetUp()
        {
            var framework = FrameworkContext.Current;

            framework
                .ComponentContext
                .RegisterAssembly<AdhocComponent>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mimick.Tests.Configuration.xml");

            framework
                .ConfigurationContext
                .Register(new XmlConfigurationSource(stream));

            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mimick.Tests.Configuration.yaml");

            framework
                .ConfigurationContext
                .Register(new YamlConfigurationSource(stream));

            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mimick.Tests.Configuration.json");

            framework
                .ConfigurationContext
                .Register(new JsonConfigurationSource(stream));

            framework
                .Initialize();
        }

        [OneTimeTearDown]
        public static void TearDown()
        {
            var framework = FrameworkContext.Current;
            framework.Dispose();
        }

        public static void Throwing() => throw new NotImplementedException();
    }
}
