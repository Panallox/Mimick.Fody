using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick;
using Mimick.Aspect;

namespace AssemblyToProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            var example = new Example();

            example.Write();
            example.Read("Good morning");

            string message = example.Generate();
            Console.WriteLine($"Generate: {message}");

            Example.WriteStatic();

            Console.ReadLine();
        }

        void ReflectMe()
        {
            var attribute = new TestAttribute();
            var args = new MethodInterceptionArgs(this, new object[0], null, MethodInfo.GetCurrentMethod());

            try
            {
                attribute.OnEnter(args);
            }
            catch (Exception ex)
            {
                attribute.OnException(args, ex);
            }
            finally
            {
                attribute.OnExit(args);
            }
        }
    }

    class Example
    {
        [Test]
        public void Write()
        {
            Console.WriteLine("Hello world");
            throw new Exception("Nothing to see here");
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
