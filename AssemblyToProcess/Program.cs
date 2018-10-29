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
            var factory = new DependencyFactory();

            factory.Register<Example>("testing").Singleton();

            var instance = factory.Resolve<Example>();
            instance.Read("Hello World");

            instance = factory.Resolve("testing") as Example;
            instance.Read("Hello again");

            instance = factory.Resolve<IExample>() as Example;
            instance.Read("Hello interface");
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    public interface IExample
    {
        void Write();
    }

    public class Example : IExample
    {
        [Test]
        public void Write()
        {
            Console.WriteLine("Hello world from method");
        }

        [Test]
        public string Generate()
        {
            return "Hello world";
        }

        public void Read([Param] string message)
        {
            Console.WriteLine("Read: " + message);
        }

        [Test]
        public static void WriteStatic() => throw new Exception("Nothing thrown here (static, wink wink)");
    }
}
