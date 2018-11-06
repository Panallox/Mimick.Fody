using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mimick;
using Mimick.Aspect;
using Mimick.Framework;

namespace AssemblyToProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = FrameworkConfiguration
                .Begin()
                .Assemblies(a => a
                    .Add(Assemblies.This)
                    .Add(Assemblies.Find("Mimick.*"))
                    .Add(Assemblies.Of<Program>()))
                .Configurations(c => c
                    .Add(Configurations.AppConfig));

            FrameworkContext.Configure(configuration);

            var context = FrameworkContext.Instance;
            var dependencies = context.Dependencies;

            dependencies.Register<Service>("svc");

            
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    public class Service
    {
        public void Print() => Console.WriteLine("Service print was called");
    }

    [PropertyChanged]
    public class Testing<T>
    {
        [Value("Testing")]
        public string myField;

        [Value("Port")]
        public int MyPort;

        [Value("Testing")]
        [NotEmpty]
        public string MyProperty { get; set; }

        [Autowire]
        public Service MyService { get; set; }

        [Autowire("svc")]
        public Service MyNamedService { get; set; }

        [IgnoreChange]
        [Minimum(10)]
        public int MyChanged { get; set; }

        [NotEmpty]
        public List<string> MyList { get; set; }

        public Testing() : base()
        {

        }

        [PreConstruct]
        private void InitializeBefore() => Console.WriteLine($"I initialize before");

        [Autowire]
        [PostConstruct]
        private void Initialize(Service svc = null) => Console.WriteLine($"I initialize after (myField = {myField}) where service = {svc}");

        [Disposable]
        private void DisposeInstance() => Console.WriteLine($"I am being disposed!");

        [Disposable]
        private void DisposeAgain() => Console.WriteLine($"I am being disposed a second time!");
    }
}
