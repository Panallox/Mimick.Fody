using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class implementation of the component descriptor which manages information on a component of the framework.
    /// </summary>
    sealed class ComponentDescriptor : IComponentDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptor"/> class.
        /// </summary>
        /// <param name="type">The component type.</param>
        /// <param name="constructor">The constructor method used to instantiate the type.</param>
        /// <param name="interfaces">The interfaces implemented by the component type.</param>
        /// <param name="names">The names associated with the component.</param>
        public ComponentDescriptor(Type type, Func<object> constructor, Type[] interfaces, string[] names)
        {
            Constructor = constructor;
            Designer = null;
            Interfaces = interfaces;
            Names = names;
            Type = type;
        }

        #region Properties

        /// <summary>
        /// Gets the constructor method used to instantiate the component type.
        /// </summary>
        public Func<object> Constructor
        {
            get;
        }

        /// <summary>
        /// Gets the component designer which can be used to activate the component.
        /// </summary>
        public IComponentDesigner Designer
        {
            get; set;
        }

        /// <summary>
        /// Gets the optional collection of interfaces which have been implemented by the component type.
        /// </summary>
        public Type[] Interfaces
        {
            get;
        }

        /// <summary>
        /// Gets the optional collection of names associated with the component.
        /// </summary>
        public string[] Names
        {
            get;
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        public Type Type
        {
            get;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Designer.IsDisposable)
                Designer.Dispose();
        }
    }
}
