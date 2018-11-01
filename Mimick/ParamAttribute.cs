using Mimick.Aspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamAttribute : Attribute, IParameterInterceptor
    {
        public void OnEnter(ParameterInterceptionArgs e)
        {
            Console.WriteLine("[I] Intercepted argument " + (e?.Value ?? "<null>"));
        }
    }
}
