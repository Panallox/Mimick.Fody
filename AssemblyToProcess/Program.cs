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

            var working = new Example<int>();
            working.Testing();

            Console.WriteLine("Creating testing<T>");
            var testing = new Testing<int>();

            Console.WriteLine("Calling print(T)");
            testing.Print(1234);

            Console.WriteLine("Calling print<U>(U)");
            testing.PrintFor("Hello world", 456);

            Console.WriteLine("Calling testing");
            Console.WriteLine($"Value is {testing.testing}");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    public class Testing<T>
    {
        [Prop]
        public string testing;

        [Test]
        public void Print(T value) => Console.WriteLine($"Value is {value}");

        [Test]
        public void PrintFor<U>(U value, T other) => Console.WriteLine($"Value<U> is {value} and {other}");
    }

    public class Example<T>
    {
        static TestAttribute att;
        static MethodInfo mi;
        
        static Example()
        {
            att = new TestAttribute();
            mi = typeof(Example<T>).GetMethod("Testing");
        }

        public void Testing()
        {
            var args = new MethodInterceptionArgs(this, new object[0], null, mi);
        }
    }
}
