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

            Console.WriteLine("Creating testing<T>");
            var testing = new Testing<int>();

            Console.WriteLine("Calling print(T)");
            testing.Print(1234);

            Console.WriteLine("Calling print<U>(U)");
            testing.PrintFor("Hello world", 456);

            Console.WriteLine("Calling testing");
            Console.WriteLine($"Value is {testing.testing}");

            testing.Other = "Hello!!";
            Console.WriteLine($"Other is now {testing.Other}");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    [PropertyChanged]
    public class Testing<T>
    {
        [Prop]
        public string testing;

        public string Other { get; set; }

        public event PropertyChangedEventHandler TestEvent;

        public Testing()
        {
            Console.WriteLine("I am the constructor");
        }

        [Test]
        public void Print(T value) => Console.WriteLine($"Value is {value}");

        [Test]
        public void PrintFor<U>(U value, T other) => Console.WriteLine($"Value<U> is {value} and {other} and {testing}");

        [PostConstruct]
        public void AfterInit()
        {
            Console.WriteLine("I have been invoked after the constructor");
        }

        [PreConstruct]
        public void BeforeInit()
        {
            Console.WriteLine("I have been invoked before the constructor");
        }
    }
    
}
