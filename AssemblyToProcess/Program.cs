using System;
using System.Collections.Generic;
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
            var testing = new Testing();

            Console.WriteLine("Calling print(T)");
            testing.Print(1234);

            Console.WriteLine("Calling print<U>(U)");
            testing.PrintFor("Hello world");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    public class Testing
    {
        [Test]
        public void Print(int value) => Console.WriteLine($"Value is {value}");

        [Test]
        public void PrintFor<U>(U value) => Console.WriteLine($"Value<U> is {value}");
    }
}
