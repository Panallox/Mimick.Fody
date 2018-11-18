using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Fody.Weavers;
using Mono.Cecil;

/// <summary>
/// A class containing methods for weaving an implementation into a class.
/// </summary>
public partial class ModuleWeaver
{
    /// <summary>
    /// Weaves all implementations.
    /// </summary>
    public void WeaveImplementations()
    {
        var candidates = Context.Candidates.FindTypeByImplements();

        foreach (var item in candidates)
        {
            var emitter = new TypeEmitter(ModuleDefinition, item.Type, Context);

            foreach (var attribute in item.Implements)
                WeaveImplementation(emitter, attribute);
        }
    }

    /// <summary>
    /// Weaves an implementation of a provided attribute type against a provided type.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="attribute">The attribute.</param>
    public void WeaveImplementation(TypeEmitter emitter, CustomAttribute attribute)
    {
        var implementType = attribute.AttributeType.Resolve();
        var interfaceType = attribute.GetAttribute(Context.Finder.CompilationImplementsAttribute)?.GetProperty<TypeReference>("Interface")?.Resolve();
                
        if (interfaceType == null)
            return;

        if (emitter.Target.Interfaces.Any(i => i.InterfaceType.FullName == interfaceType.FullName))
            return;

        if (interfaceType.HasGenericParameters)
            throw new NotSupportedException($"Cannot implement interface {interfaceType.FullName} due to generic parameters");

        var field = CreateAttribute(emitter, attribute);

        WeaveImplementation(emitter, implementType, interfaceType, field);
    }

    public void WeaveImplementation(TypeEmitter emitter, TypeDefinition implementType, TypeDefinition interfaceType, Variable field)
    {
        if (!implementType.Interfaces.Any(i => i.InterfaceType.FullName == interfaceType.FullName))
            throw new NotSupportedException($"Cannot implement attribute '{implementType.FullName}' as it does not implement '{interfaceType.FullName}'");

        if (emitter.Target.Interfaces.Any(i => i.InterfaceType.FullName == interfaceType.FullName))
            return;

        emitter.Target.Interfaces.Add(new InterfaceImplementation(interfaceType.Import()));

        foreach (var method in interfaceType.Methods.Concat(interfaceType.Interfaces.Select(i => i.InterfaceType.Resolve()).SelectMany(i => i.Methods)))
            WeaveImplementedMethod(emitter, field, method);

        foreach (var property in interfaceType.Properties)
            WeaveImplementedProperty(emitter, field, property);

        foreach (var evt in interfaceType.Events)
            WeaveImplementedEvent(emitter, field, evt);
    }

    /// <summary>
    /// Weave an implementation of an event against the provided type.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="field">The attribute field.</param>
    /// <param name="evt">The event.</param>
    public void WeaveImplementedEvent(TypeEmitter emitter, Variable field, EventReference evt)
    {
        var implemented = field.Type.GetEvent(evt.Name, evt.EventType);

        if (implemented == null)
            throw new MissingMemberException($"Cannot implement '{field.Type.FullName}' as it does not implement event '{evt.Name}'");

        var emitted = emitter.EmitEvent(evt.Name, implemented.EventType);
        var definition = implemented.Resolve();

        if (!emitted.HasAdd)
        {
            var add = emitted.GetAdd();
            var ail = add.GetIL();
            var interfaceAdd = definition.AddMethod.Import();

            ail.Emit(Codes.Nop);
            ail.Emit(Codes.ThisIf(field));
            ail.Emit(Codes.Load(field));
            ail.Emit(Codes.Arg(add.IsStatic ? 0 : 1));
            ail.Emit(Codes.Invoke(interfaceAdd.GetGeneric()));
            ail.Emit(Codes.Return);
        }

        if (!emitted.HasRemove)
        {
            var remove = emitted.GetRemove();
            var ril = remove.GetIL();
            var interfaceRemove = definition.RemoveMethod.Import();

            ril.Emit(Codes.Nop);
            ril.Emit(Codes.ThisIf(field));
            ril.Emit(Codes.Load(field));
            ril.Emit(Codes.Arg(remove.IsStatic ? 0 : 1));
            ril.Emit(Codes.Invoke(interfaceRemove.GetGeneric()));
            ril.Emit(Codes.Return);
        }
    }

    /// <summary>
    /// Weave an implementation of a method against the provided type.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="field">The attribute field.</param>
    /// <param name="method">The method.</param>
    public void WeaveImplementedMethod(TypeEmitter emitter, Variable field, MethodReference method)
    {
        var resolved = method.Resolve();

        var emitted = emitter.EmitMethod(
            method.Name,
            method.ReturnType.Import(),
            parameterTypes: method.HasParameters ? method.Parameters.Select(p => p.ParameterType.Import()).ToArray() : new TypeReference[0],
            genericTypes: method.HasGenericParameters ? method.GenericParameters.ToArray() : new GenericParameter[0],
            toStatic: resolved.IsStatic,
            toVisibility: resolved.GetVisiblity()
        );

        var implemented = field.Type.GetMethod(method.Name, method.ReturnType, method.Parameters.Select(p => p.ParameterType).ToArray(), method.GenericParameters.ToArray());

        if (implemented == null)
            throw new MissingMemberException($"Cannot implement '{field.Type.FullName}' as it does not implement method '{method.FullName}'");

        var imported = implemented.Import();

        var il = emitted.GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.ThisIf(field));
        il.Emit(Codes.Load(field));

        for (int i = 0, count = method.Parameters.Count; i < count; i++)
            il.Emit(Codes.Arg(i + 1));

        il.Emit(Codes.Invoke(imported.GetGeneric()));
        il.Emit(Codes.Return);
    }

    /// <summary>
    /// Weaves an implementation of a property against the provided type.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="field">The attribute field.</param>
    /// <param name="property">The property.</param>
    public void WeaveImplementedProperty(TypeEmitter emitter, Variable field, PropertyDefinition property)
    {
        var emitted = emitter.EmitProperty(property.Name, property.PropertyType.Import(), toBackingField: true);
        var implemented = field.Type.GetProperty(property.Name, property.PropertyType)?.Resolve();

        if (implemented == null)
            throw new MissingMemberException($"Cannot implement '{field.Type.FullName}' as it does not implement property '{property.Name}'");

        var source = new PropertyEmitter(emitter, implemented);

        if (source.HasGetter && !emitted.HasGetter)
        {
            var getter = emitted.GetGetter();
            var il = getter.GetIL();
            var propertyGet = implemented.GetMethod.Import();

            il.Emit(Codes.Nop);
            il.Emit(Codes.ThisIf(field));
            il.Emit(Codes.Load(field));
            il.Emit(Codes.Invoke(propertyGet.GetGeneric()));
            il.Emit(Codes.Return);
        }

        if (source.HasSetter && !emitted.HasSetter)
        {
            var setter = emitted.GetSetter();
            var il = setter.GetIL();
            var propertySet = implemented.SetMethod.Import();

            il.Emit(Codes.Nop);
            il.Emit(Codes.ThisIf(field));
            il.Emit(Codes.Load(field));
            il.Emit(Codes.Arg(setter.Target.IsStatic ? 0 : 1));
            il.Emit(Codes.Invoke(propertySet.GetGeneric()));
            il.Emit(Codes.Return);
        }
    }
}