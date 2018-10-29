using Mimick.Aspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute, IMethodInterceptor
    {
        public void OnEnter(MethodInterceptionArgs e)
        {
            Console.WriteLine("> Entered");
        }

        public void OnException(MethodInterceptionArgs e, Exception ex)
        {
            
        }

        public void OnExit(MethodInterceptionArgs e)
        {
            Console.WriteLine("< Exited");
            e.Return = "Goodbye World";
        }
    }
}
