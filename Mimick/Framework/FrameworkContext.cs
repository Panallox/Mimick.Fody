using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing the 
    /// </summary>
    public sealed class FrameworkContext : IFrameworkContext
    {
        public IDependencyFactory DependencyFactory => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
