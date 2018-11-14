using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Designers;

namespace Mimick.Framework
{
    /// <summary>
    /// A class providing options for configuring a component registration.
    /// </summary>
    sealed class ComponentRegistration : IComponentRegistration
    {
        private readonly ComponentDescriptor[] descriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentRegistration"/> class.
        /// </summary>
        /// <param name="component">The component descriptor.</param>
        public ComponentRegistration(ComponentDescriptor[] components) => descriptors = components;

        /// <summary>
        /// Sets the scope of the component, which determines how the component should persist.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public IComponentRegistration ToScope(Scope scope)
        {
            foreach (var descriptor in descriptors)
            {
                switch (scope)
                {
                    case Scope.Adhoc:
                        descriptor.Designer = new AdhocDesigner(descriptor.Constructor);
                        break;
                    case Scope.Singleton:
                        descriptor.Designer = new SingletonDesigner(descriptor.Constructor);
                        break;
                    default:
                        throw new ArgumentException($"Cannot resolve a component designer for scope {scope}");
                }
            }

            return this;
        }
    }
}
