using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;
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
        var interfaceType = attribute.GetAttribute<CompilationImplementsAttribute>()?.GetProperty<TypeReference>("Interface")?.Resolve();
        
        if (interfaceType == null)
            return;
        
        if (!implementType.Interfaces.Any(i => i.InterfaceType.FullName == interfaceType.FullName))
            throw new NotSupportedException($"Cannot implement attribute '{implementType.FullName}' as it does not implement '{interfaceType.FullName}'");
        
        var field = CreateAttribute(emitter, attribute);
        emitter.Target.Interfaces.Add(new InterfaceImplementation(interfaceType));

        foreach (var property in interfaceType.Properties)
            WeaveImplementedProperty(emitter, field, property);

        foreach (var evt in interfaceType.Events)
            WeaveImplementedEvent(emitter, field, evt);
    }

    public void WeaveImplementedEvent(TypeEmitter emitter, Variable field, EventReference evt)
    {
        var emitted = emitter.EmitEvent(evt.Name, evt.EventType);
        var implemented = field.Type.GetEvent(evt.Name, evt.EventType);

        if (implemented == null)
            throw new MissingMemberException($"Cannot implement '{field.Type.FullName}' as it does not implement event '{evt.Name}'");

        var add = emitted.GetAdd();
        var ail = add.GetIL();

        ail.Emit(Codes.Nop);
        ail.Emit(Codes.ThisIf(field));
        ail.Emit(Codes.Load(field));
        ail.Emit(Codes.Arg(add.IsStatic ? 0 : 1));
        ail.Emit(Codes.Invoke(implemented.Resolve().AddMethod));
        ail.Emit(Codes.Return);
    }

    /// <summary>
    /// Weaves an implementation of a property against the provided type.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="field">The attribute field.</param>
    /// <param name="property">The property.</param>
    public void WeaveImplementedProperty(TypeEmitter emitter, Variable field, PropertyDefinition property)
    {
        var emitted = emitter.EmitProperty(property.Name, property.PropertyType, toBackingField: true);
        var implemented = field.Type.GetProperty(property.Name, property.PropertyType)?.Resolve();

        if (implemented == null)
            throw new MissingMemberException($"Cannot implement '{field.Type.FullName}' as it does not implement property '{property.Name}'");

        var source = new PropertyEmitter(emitter, implemented);

        if (source.HasGetter)
        {
            var il = emitted.GetGetter().GetIL();

            il.Emit(Codes.Nop);
            il.Emit(Codes.ThisIf(field));
            il.Emit(Codes.Load(field));
            il.Emit(Codes.Invoke(implemented.GetMethod.GetGeneric()));
            il.Emit(Codes.Return);
        }

        if (source.HasSetter)
        {
            var setter = emitted.GetSetter();
            var il = setter.GetIL();

            il.Emit(Codes.Nop);
            il.Emit(Codes.ThisIf(field));
            il.Emit(Codes.Load(field));
            il.Emit(Codes.Arg(setter.Target.IsStatic ? 0 : 1));
            il.Emit(Codes.Invoke(implemented.SetMethod.GetGeneric()));
            il.Emit(Codes.Return);
        }
    }
}