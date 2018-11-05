using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
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

            var testing = new Testing<int>();

            Console.WriteLine("-- PropertyChanged");
            var prop = testing as INotifyPropertyChanged;
            prop.PropertyChanged += (s, e) => Console.WriteLine($"The property '{e.PropertyName}' has been changed!");

            testing.MyProperty = "One";
            testing.MyProperty = "Two";

            var inner = new Testing<int>.InnerConverter<long>();
            inner.Testing();
            
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
        public string MyProperty { get; set; }

        [Autowire]
        public Service MyService { get; set; }

        [Autowire("svc")]
        public Service MyNamedService { get; set; }

        public Testing() : base()
        {

        }

        [PreConstruct]
        private void InitializeBefore() => Console.WriteLine($"I initialize before");

        [Autowire]
        [PostConstruct]
        private void Initialize(Service svc = null) => Console.WriteLine($"I initialize after (myField = {myField}) where service = {svc}");

        public class InnerConverter<U> : ITesting<T, U>
        {
            public T convert(U inbound) => default(T);

            public void Testing([Value("Testing")] string name = null) => Console.WriteLine($"Inner testing = {name}");
        }
    }

    public interface ITesting<T, U>
    {
        T convert(U inbound);
    }
    
}
