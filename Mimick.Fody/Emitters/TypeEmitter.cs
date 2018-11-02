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
    /// An emitter class containing methods and properties for emitting against a type.
    /// </summary>
    public class TypeEmitter
    {
        private MethodEmitter[] constructors;
        private MethodEmitter staticConstructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeEmitter" /> class.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="type">The type.</param>
        /// <param name="context">The context.</param>
        public TypeEmitter(ModuleDefinition module, TypeReference type, WeaveContext context)
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
        /// Performs an implicit conversion from <see cref="TypeEmitter"/> to <see cref="TypeDefinition"/>.
        /// </summary>
        /// <param name="weaver">The weaver.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator TypeDefinition(TypeEmitter weaver) => weaver.Target;

        /// <summary>
        /// Create a new event within the type. If an event already exists with the provided name, type
        /// and static modifier, then the existing field will be returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="toStatic"></param>
        /// <returns></returns>
        public EventEmitter EmitEvent(string name, TypeReference type, bool toStatic = false)
        {
            var existing = Target.Events.FirstOrDefault(a => a.Name == name && a.EventType.FullName == type.FullName);

            if (existing != null)
                return new EventEmitter(this, existing);

            var evt = new EventDefinition(name, EventAttributes.None, type);
            Target.Events.Add(evt);

            Context.AddCompilerGenerated(evt);

            return new EventEmitter(this, evt);
        }

        /// <summary>
        /// Create a new field within the type. If a field already exists with the provided name, type
        /// and static modifier, then the existing field will be returned.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="toStatic">Whether the field should be static.</param>
        /// <returns>A <see cref="Variable"/> instance.</returns>
        public Variable EmitField(string name, TypeReference type, bool toStatic = false)
        {
            var existing = Target.Fields.FirstOrDefault(a => a.Name == name && a.FieldType.FullName == type.FullName && a.IsStatic == toStatic);

            if (existing != null)
                return new Variable(existing);

            var attributes = FieldAttributes.Private;

            if (toStatic)
                attributes |= FieldAttributes.Static;

            var field = new FieldDefinition(name, attributes, type);
            Target.Fields.Add(field);

            Context.AddCompilerGenerated(field);
            Context.AddNonSerialized(field);

            return new Variable(field);
        }

        /// <summary>
        /// Create a new property within the type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="toStatic">Whether the field should be static.</param>
        /// <param name="toBackingField">Whether the property should have a corresponding backing field.</param>
        /// <returns>A <see cref="PropertyEmitter"/> instance.</returns>
        public PropertyEmitter EmitProperty(string name, TypeReference type, bool toStatic = false, bool toBackingField = false)
        {
            var existing = Target.Properties.FirstOrDefault(a => a.Name == name && a.PropertyType.FullName == type.FullName);

            if (existing != null)
                throw new NotSupportedException($"A property exists in '{Target.FullName}' named '{name}'");

            if (toBackingField)
            {
                var field = $"<{name}>k__BackingField";
                EmitField(field, type, toStatic: toStatic);
            }

            var attributes = PropertyAttributes.SpecialName;
            var property = new PropertyDefinition(name, attributes, type);
            Target.Properties.Add(property);

            Context.AddCompilerGenerated(property);
            Context.AddNonSerialized(property);

            return new PropertyEmitter(this, property);
        }

        /// <summary>
        /// Gets a collection of method emitters for the constructors of the type, where each constructor
        /// does not call a reference to another self-constructor.
        /// </summary>
        /// <returns>A collection of <see cref="MethodEmitter"/> values.</returns>
        public MethodEmitter[] GetConstructors()
        {
            if (constructors != null)
                return constructors;

            var candidates = Target.Methods.Where(m => m.Name == ".ctor" && m.IsSpecialName);
            var match = new List<MethodEmitter>();

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
                    match.Add(new MethodEmitter(this, candidate));
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
        /// Gets a method emitter for the provided method name and parameter types. If no parameter types are provided then the
        /// first matching method is used.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameter types.</param>
        /// <returns>A <see cref="MethodEmitter"/> value.</returns>
        public MethodEmitter GetMethod(string name, params Type[] parameters)
        {
            var candidates = Target.Methods.Where(m => m.Name == name);

            if (parameters.Length > 0)
                candidates = candidates.Where(m => m.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(parameters.Select(p => p.FullName)));

            var method = candidates.FirstOrDefault();

            if (method == null)
                throw new MissingMethodException($"Cannot find method '{name}' in '{Target.FullName}'");

            return new MethodEmitter(this, method);
        }

        /// <summary>
        /// Gets a method emitter for the provided method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A <see cref="MethodEmitter"/> value.</returns>
        public MethodEmitter GetMethod(MethodReference method)
        {
            if (method.DeclaringType.FullName != Target.FullName)
                throw new NotSupportedException($"Cannot reference method '{method.FullName}' from '{Target.FullName}'");

            return new MethodEmitter(this, method);
        }

        /// <summary>
        /// Gets a property emitter for the provided property name and return type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="returnType">The return type.</param>
        /// <returns></returns>
        public PropertyEmitter GetProperty(string name, TypeReference returnType)
        {
            var match = Target.Properties.FirstOrDefault(p => p.Name == name && p.PropertyType.FullName == returnType.FullName);

            if (match == null)
                throw new MissingMemberException($"Cannot find a property '{name}' with type '{returnType.FullName}' in '{Target.FullName}'");

            return new PropertyEmitter(this, match);
        }

        /// <summary>
        /// Gets a method emitter for the static constructor of the type.
        /// </summary>
        /// <returns></returns>
        public MethodEmitter GetStaticConstructor()
        {
            if (staticConstructor == null)
            {
                var method = Target.Methods.Where(m => m.IsConstructor && m.IsStatic).FirstOrDefault();

                if (method != null)
                    staticConstructor = new MethodEmitter(this, method);
                else
                {
                    method = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Private, Module.TypeSystem.Void);

                    if (Target.IsGenericInstance)
                    {
                        foreach (var generic in Target.GenericParameters)
                            method.GenericParameters.Add(generic);
                    }

                    Target.Methods.Add(method);

                    staticConstructor = new MethodEmitter(this, method);
                    staticConstructor.GetIL().Emit(Codes.Return);
                }

                var code = staticConstructor.GetIL();
                code.Insert = CodeInsertion.Before;
                code.Position = code.GetLast();
            }

            return staticConstructor;
        }
    }
}
