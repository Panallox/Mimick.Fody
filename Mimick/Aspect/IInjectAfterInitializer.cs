using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing an attribute which injects an associated method into the constructor of a type after the method body.
    /// </summary>
    /// <remarks>
    /// The interface does not contain any methods, and implementing attributes will automatically introduce the behaviour
    /// of copying the associated method invocation into the constructor.
    /// </remarks>
    public interface IInjectAfterInitializer
    {

    }
}
