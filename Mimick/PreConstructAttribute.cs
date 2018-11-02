using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PreConstructAttribute : Attribute, IInjectBeforeInitializer
    {

    }
}
