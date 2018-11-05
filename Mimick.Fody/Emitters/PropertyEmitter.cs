using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody.Weavers
{
    /// <summary>
    /// An emitter class containing methods for emitting against a property.
    /// </summary>
    public class PropertyEmitter
    {
        private MethodEmitter getter;
        private MethodEmitter setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEmitter"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="property">The property.</param>
        public PropertyEmitter(TypeEmitter parent, PropertyReference property)
        {
            BackingField = property.GetBackingField();
            Parent = parent;
            Target = property as PropertyDefinition ?? property.Resolve();

            if (Target.GetMethod != null)
                getter = new MethodEmitter(parent, Target.GetMethod);

            if (Target.SetMethod != null)
                setter = new MethodEmitter(parent, Target.SetMethod);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the backing field.
        /// </summary>
        public FieldReference BackingField
        {
            get; set;
        }

        /// <summary>
        /// Gets whether the property has an existing getter method.
        /// </summary>
        public bool HasGetter => Target.GetMethod != null;

        /// <summary>
        /// Gets whether the property has an existing setter method.
        /// </summary>
        public bool HasSetter => Target.SetMethod != null;

        /// <summary>
        /// Gets whether the property is static.
        /// </summary>
        public bool IsStatic => (Target.GetMethod ?? Target.SetMethod).IsStatic;

        /// <summary>
        /// Gets the parent type weaver.
        /// </summary>
        public TypeEmitter Parent
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
        public MethodEmitter GetGetter()
        {
            if (getter != null)
                return getter;

            var attributes = MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
            var method = new MethodDefinition($"get_{Target.Name}", attributes, Target.PropertyType);
            Parent.Target.Methods.Add(method);
            Target.GetMethod = method;

            Parent.Context.AddCompilerGenerated(method);

            return getter = new MethodEmitter(Parent, method);
        }

        /// <summary>
        /// Gets or creates a method weaver for the property setter.
        /// </summary>
        /// <returns></returns>
        public MethodEmitter GetSetter()
        {
            if (setter != null)
                return setter;

            var attributes = MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
            var method = new MethodDefinition($"set_{Target.Name}", attributes, Target.Module.TypeSystem.Void);
            var parameter = new ParameterDefinition("value", ParameterAttributes.None, Target.PropertyType);

            method.Parameters.Add(parameter);
            Parent.Target.Methods.Add(method);
            Target.SetMethod = method;

            Parent.Context.AddCompilerGenerated(method);

            return setter = new MethodEmitter(Parent, method);
        }
    }
}
