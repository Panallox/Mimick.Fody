using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody.Weavers
{
    /// <summary>
    /// A weaver class containing methods for weaving against an existing or new type.
    /// </summary>
    public class TypeWeaver
    {
        private MethodWeaver[] constructors;
        private MethodWeaver staticConstructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeWeaver" /> class.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="type">The type.</param>
        /// <param name="context">The context.</param>
        public TypeWeaver(ModuleDefinition module, TypeReference type, WeaveContext context)
        {
            Context = context;
            Module = module;
            Target = type as TypeDefinition ?? type.Resolve();
        }

        #region Properties

        /// <summary>
        /// Gets the weaving context.
        /// </summary>
        public WeaveContext Context
        {
            get;
        }

        /// <summary>
        /// Gets the module definition.
        /// </summary>
        public ModuleDefinition Module
        {
            get;
        }

        /// <summary>
        /// Gets the resolved type definition.
        /// </summary>
        public TypeDefinition Target
        {
            get;
        }

        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="TypeWeaver"/> to <see cref="TypeDefinition"/>.
        /// </summary>
        /// <param name="weaver">The weaver.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator TypeDefinition(TypeWeaver weaver) => weaver.Target;

        /// <summary>
        /// Create a new field within the type. If a field already exists with the provided name, type
        /// and static modifier, then the existing field will be returned.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="toStatic">Whether the field should be static.</param>
        /// <returns>A <see cref="Variable"/> instance.</returns>
        public Variable CreateField(string name, TypeReference type, bool toStatic = false)
        {
            var existing = Target.Fields.FirstOrDefault(a => a.Name == name && a.FieldType.FullName == type.FullName && a.IsStatic == toStatic);

            if (existing != null)
                return new Variable(existing);

            var attributes = FieldAttributes.Private;

            if (toStatic)
                attributes |= FieldAttributes.Static;

            var field = new FieldDefinition(name, attributes, type);
            Target.Fields.Add(field);

            return new Variable(field);
        }

        /// <summary>
        /// Create a new property within the type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="toStatic">Whether the field should be static.</param>
        /// <returns>A <see cref="PropertyWeaver"/> instance.</returns>
        public PropertyWeaver CreateProperty(string name, TypeReference type, bool toStatic = false)
        {
            var existing = Target.Properties.FirstOrDefault(a => a.Name == name && a.PropertyType.FullName == type.FullName);

            if (existing != null)
                throw new NotSupportedException($"A property exists in '{type.FullName}' named '{name}'");

            var attributes = PropertyAttributes.SpecialName;
            var property = new PropertyDefinition(name, attributes, type);
            Target.Properties.Add(property);

            return new PropertyWeaver(this, property);
        }

        /// <summary>
        /// Gets a collection of method weavers for the constructors of the type, where each constructor
        /// does not call a reference to another self-constructor.
        /// </summary>
        /// <returns>A collection of <see cref="MethodWeaver"/> values.</returns>
        public MethodWeaver[] GetConstructors()
        {
            if (constructors != null)
                return constructors;

            var candidates = Target.Methods.Where(m => m.Name == ".ctor" && m.IsSpecialName);
            var match = new List<MethodWeaver>();

            foreach (var candidate in candidates)
            {
                var il = candidate.Body.Instructions;
                var ignore = false;

                foreach (var i in il)
                {
                    if (i.OpCode == OpCodes.Call)
                    {
                        var target = i.Operand;

                        if (target is MethodDefinition method && method.Name == ".ctor" && method.DeclaringType.FullName == Target.FullName)
                        {
                            ignore = true;
                            break;
                        }
                    }
                }

                if (!ignore)
                    match.Add(new MethodWeaver(this, candidate));
            }

            return constructors = match.ToArray();
        }

        /// <summary>
        /// Gets a variable for the provided field name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="Variable"/> value.</returns>
        public Variable GetField(string name)
        {
            var field = Target.Fields.FirstOrDefault(a => a.Name == name);

            if (field == null)
                throw new MissingFieldException($"Cannot find field '{name}' in '{Target.FullName}'");

            return new Variable(field);
        }

        /// <summary>
        /// Gets a method weaver for the provided method name and parameter types. If no parameter types are provided then the
        /// first matching method is used.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameter types.</param>
        /// <returns>A <see cref="MethodWeaver"/> value.</returns>
        public MethodWeaver GetMethod(string name, params Type[] parameters)
        {
            var candidates = Target.Methods.Where(m => m.Name == name);

            if (parameters.Length > 0)
                candidates = candidates.Where(m => m.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(parameters.Select(p => p.FullName)));

            var method = candidates.FirstOrDefault();

            if (method == null)
                throw new MissingMethodException($"Cannot find method '{name}' in '{Target.FullName}'");

            return new MethodWeaver(this, method);
        }

        /// <summary>
        /// Gets a method weaver for the provided method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A <see cref="MethodWeaver"/> value.</returns>
        public MethodWeaver GetMethod(MethodReference method)
        {
            if (method.DeclaringType.FullName != Target.FullName)
                throw new NotSupportedException($"Cannot reference method '{method.FullName}' from '{Target.FullName}'");

            return new MethodWeaver(this, method);
        }

        /// <summary>
        /// Gets a method weaver for the static constructor of the type.
        /// </summary>
        /// <returns></returns>
        public MethodWeaver GetStaticConstructor()
        {
            if (staticConstructor == null)
            {
                var method = Target.Methods.Where(m => m.Name == ".cctor" && m.IsStatic).FirstOrDefault();

                if (method != null)
                    staticConstructor = new MethodWeaver(this, method);
                else
                {
                    method = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Private, Module.TypeSystem.Void);

                    if (Target.IsGenericInstance)
                    {
                        foreach (var generic in Target.GenericParameters)
                            method.GenericParameters.Add(generic);
                    }

                    Target.Methods.Add(method);

                    staticConstructor = new MethodWeaver(this, method);
                    staticConstructor.GetWeaver().Emit(Codes.Return);
                }

                var code = staticConstructor.GetWeaver();
                code.Insert = CodeInsertion.Before;
                code.Position = code.GetLast();
            }

            return staticConstructor;
        }
    }
}
