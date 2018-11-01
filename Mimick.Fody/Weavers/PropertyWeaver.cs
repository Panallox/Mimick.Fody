using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody.Weavers
{
    /// <summary>
    /// A class containing methods for weaving against a property.
    /// </summary>
    public class PropertyWeaver
    {
        private MethodWeaver getter;
        private MethodWeaver setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyWeaver"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="property">The property.</param>
        public PropertyWeaver(TypeWeaver parent, PropertyReference property)
        {
            Parent = parent;
            Target = property as PropertyDefinition ?? property.Resolve();

            if (Target.GetMethod != null)
                getter = new MethodWeaver(parent, Target.GetMethod);

            if (Target.SetMethod != null)
                setter = new MethodWeaver(parent, Target.SetMethod);
        }

        #region Properties

        /// <summary>
        /// Gets whether the property has an existing getter method.
        /// </summary>
        public bool HasGetter => Target.GetMethod != null;

        /// <summary>
        /// Gets whether the property has an existing setter method.
        /// </summary>
        public bool HasSetter => Target.SetMethod != null;

        /// <summary>
        /// Gets the parent type weaver.
        /// </summary>
        public TypeWeaver Parent
        {
            get;
        }

        /// <summary>
        /// Gets the resolved property definition.
        /// </summary>
        public PropertyDefinition Target
        {
            get;
        }

        #endregion

        /// <summary>
        /// Gets or creates a method weaver for the property getter.
        /// </summary>
        /// <returns></returns>
        public MethodWeaver GetGetter()
        {
            if (getter != null)
                return getter;

            var attributes = MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public;
            var method = new MethodDefinition($"get_{Target.Name}", attributes, Target.PropertyType);
            Parent.Target.Methods.Add(method);
            Target.GetMethod = method;

            Parent.Context.AddCompilerGenerated(method);

            return new MethodWeaver(Parent, method);
        }

        /// <summary>
        /// Gets or creates a method weaver for the property setter.
        /// </summary>
        /// <returns></returns>
        public MethodWeaver GetSetter()
        {
            if (setter != null)
                return setter;

            var attributes = MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public;
            var method = new MethodDefinition($"set_{Target.Name}", attributes, Target.Module.TypeSystem.Void);
            var parameter = new ParameterDefinition("value", ParameterAttributes.None, Target.PropertyType);

            method.Parameters.Add(parameter);
            Parent.Target.Methods.Add(method);
            Target.SetMethod = method;

            Parent.Context.AddCompilerGenerated(method);

            return new MethodWeaver(Parent, method);
        }
    }
}
