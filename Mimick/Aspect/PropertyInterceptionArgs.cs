using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An event arguments class containing information relevant to the interception of a property.
    /// </summary>
    public class PropertyInterceptionArgs : EventArgs
    {
        private object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInterceptionArgs" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="property">The property.</param>
        /// <param name="val">The value.</param>
        public PropertyInterceptionArgs(object instance, PropertyInfo property, object val)
        {
            Instance = instance;
            Property = property;
            value = val;
        }

        #region Properties
        
        /// <summary>
        /// Gets the object instance for which the property interception is occurring. If the parent class is <c>static</c>
        /// then this value will be <c>null</c>.
        /// </summary>
        public object Instance
        {
            get;
        }

        /// <summary>
        /// Gets whether the property value has become dirty after having been assigned during interception.
        /// </summary>
        public bool IsDirty
        {
            get; private set;
        }

        /// <summary>
        /// Gets the property which has been intercepted.
        /// </summary>
        public PropertyInfo Property
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value
        {
            get => value;
            set
            {
                this.value = value;
                IsDirty = true;
            }
        }

        #endregion
    }
}
