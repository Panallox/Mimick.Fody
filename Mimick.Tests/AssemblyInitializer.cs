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
        public static void BeforeAssembly()
        {
            var framework = FrameworkContext.Current;

            framework
                .ComponentContext
                .RegisterAssembly<AdhocComponent>();

            framework
                .ConfigurationContext
                .Register<AppConfigurationSource>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mimick.Tests.Configuration.xml");

            framework
                .ConfigurationContext
                .Register(new XmlConfigurationSource(stream));

            framework
                .Initialize();
        }
    }
}
