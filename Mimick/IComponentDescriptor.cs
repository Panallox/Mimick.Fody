using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a descriptor of a component which has been registered in the framework. The descriptor contains information
    /// on the registration parameters of the component, and exposes the component designer which manages the activation of the component.
    /// </summary>
    /// <seealso cref="IComponentDesigner"/>
    /// <seealso cref="IDisposable"/>
    public interface IComponentDescriptor : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the component designer which can be used to activate the component.
        /// </summary>
        IComponentDesigner Designer
        {
            get;
        }

        /// <summary>
        /// Gets the optional collection of interfaces which have been implemented by the component type.
        /// </summary>
        Type[] Interfaces
        {
            get;
        }

        /// <summary>
        /// Gets the optional collection of names associated with the component.
        /// </summary>
        string[] Names
        {
            get;
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        Type Type
        {
            get;
        }

        #endregion
    }
}
