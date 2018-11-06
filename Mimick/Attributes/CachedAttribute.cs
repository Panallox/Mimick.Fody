using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method or property return value should be cached dependending on the parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class CachedAttribute : Attribute, IMethodInterceptor, IPropertyGetInterceptor
    {
    }
}
