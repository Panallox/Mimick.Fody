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

            var testing = new Testing<int>();
            Console.WriteLine($"Field = {testing.MyField}");
            Console.WriteLine($"Property = {testing.MyProperty}");

            Console.WriteLine($"Port = {testing.MyPort}");
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    public class Testing<T>
    {
        [Value("Testing")]
        public string MyField;

        [Value("Port")]
        public int MyPort;

        [Value("Testing")]
        public string MyProperty { get; set; }

        public Testing()
        { 
        }

        [PostConstruct]
        private void Initialize()
        {
            Console.WriteLine("I am now initialized");
        }
    }
    
}
