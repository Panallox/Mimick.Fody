﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;
using Mimick.Fody.Weavers;
using Mono.Cecil;

/// <summary>
/// A class containing common weaving methods shared across the member weavers.
/// </summary>
public partial class ModuleWeaver
{
    /// <summary>
    /// Create an array variable containing the arguments of the method invocation.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <returns></returns>
    public Variable CreateArgumentArray(MethodEmitter method)
    {
        var parameters = method.Target.Parameters;
        var count = parameters.Count;

        var array = method.EmitLocal(Context.Refs.ObjectArray);
        var il = method.GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.Int(count));
        il.Emit(Codes.CreateArray(TypeSystem.ObjectReference));
        il.Emit(Codes.Store(array));

        for (int i = 0; i < count; i++)
        {
            var p = parameters[i];

            il.Emit(Codes.Load(array));
            il.Emit(Codes.Int(i));
            il.Emit(Codes.Arg(p));
            il.Emit(Codes.Box(p.ParameterType));
            il.Emit(Codes.StoreArray);
        }

        return array;
    }

    /// <summary>
    /// Create an attribute variable for a method definition.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable CreateAttribute(MethodEmitter method, CustomAttribute attribute)
    {
        var options = attribute.GetAttribute<CompilationOptionsAttribute>();
        var scope = options.GetProperty("Scope", notFound: AttributeScope.Singleton);

        if (scope == AttributeScope.Instanced && method.Target.IsStatic)
            scope = AttributeScope.Singleton;

        switch (scope)
        {
            case AttributeScope.Adhoc:
                return CreateAttributeAdhoc(method, attribute);
            case AttributeScope.Instanced:
                return CreateAttributeInstanced(method.Parent, attribute);
            case AttributeScope.Singleton:
                return CreateAttributeSingleton(method.Parent, attribute);
            default:
                throw new NotSupportedException($"Cannot create attribute '{attribute.AttributeType.FullName}' with scope {scope}");
        }
    }

    /// <summary>
    /// Create an attribute variable for a type reference.
    /// </summary>
    /// <param name="emitter">The type emitter.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable CreateAttribute(TypeEmitter emitter, CustomAttribute attribute)
    {
        var options = attribute.GetAttribute<CompilationOptionsAttribute>();
        var scope = options.GetProperty("Scope", notFound: AttributeScope.Singleton);

        switch (scope)
        {
            case AttributeScope.Adhoc:
            case AttributeScope.Instanced:
                return CreateAttributeInstanced(emitter, attribute);
            case AttributeScope.Singleton:
                return CreateAttributeSingleton(emitter, attribute);
            default:
                throw new NotSupportedException($"Cannot create attribute '{attribute.AttributeType.FullName}' with scope '{scope}'");
        }
    }

    /// <summary>
    /// Create an attribute variable for a property definition.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="get">The getter method.</param>
    /// <param name="set">The setter method.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable[] CreateAttribute(PropertyEmitter property, MethodEmitter get, MethodEmitter set, CustomAttribute attribute)
    {
        var method = get ?? set;
        var sta = method.Target.IsStatic;

        var options = attribute.GetAttribute<CompilationOptionsAttribute>();
        var scope = options.GetProperty("Scope", notFound: AttributeScope.Singleton);

        if (scope == AttributeScope.Instanced && sta)
            scope = AttributeScope.Singleton;

        var hasGet = attribute.HasInterface<IPropertyGetInterceptor>();
        var hasSet = attribute.HasInterface<IPropertySetInterceptor>();

        switch (scope)
        {
            case AttributeScope.Adhoc:
                var adhocGet = hasGet && get != null ? CreateAttributeAdhoc(get, attribute) : null;
                var adhocSet = hasSet && set != null ? CreateAttributeAdhoc(set, attribute) : null;
                return new[] { adhocGet, adhocSet };
            case AttributeScope.Instanced:
                var instanced = CreateAttributeInstanced(property.Parent, attribute);
                return new[] { hasGet ? instanced : null, hasSet ? instanced : null };
            case AttributeScope.Singleton:
                var singleton = CreateAttributeSingleton(property.Parent, attribute);
                return new[] { hasGet ? singleton : null, hasSet ? singleton : null };
            default:
                throw new NotSupportedException($"Cannot create attribute '{attribute.AttributeType.FullName}' with scope '{scope}'");
        }
    }

    /// <summary>
    /// Create an adhoc variable used to retain an instance of an attribute.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable CreateAttributeAdhoc(MethodEmitter method, CustomAttribute attribute)
    {
        var type = attribute.AttributeType;
        var variable = method.EmitLocal(type);
        var il = method.GetIL();

        il.Emit(Codes.Create(attribute.Constructor));
        il.Emit(Codes.Store(variable));

        if (attribute.HasInterface<IInstanceAware>() && !method.Target.IsStatic)
        {
            il.Emit(Codes.Load(variable));
            il.Emit(Codes.This);
            il.Emit(Codes.Invoke(Context.Refs.InstanceAwareInstanceSet));
        }

        return variable;
    }

    /// <summary>
    /// Creates an instance-level field used to retain an instance of an attribute.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable CreateAttributeInstanced(TypeEmitter emitter, CustomAttribute attribute)
    {
        var type = attribute.AttributeType;
        var name = $"<{type.Name}>k__Attribute";
        var field = emitter.EmitField(name, type);
        
        foreach (var ctor in emitter.GetConstructors())
        {
            var il = ctor.GetIL();

            il.Insert = CodeInsertion.Before;
            il.Position = il.GetFirst();

            il.Emit(Codes.Nop);
            il.Emit(Codes.This);
            il.Emit(Codes.Create(attribute.Constructor));
            il.Emit(Codes.Store(field));

            if (attribute.HasInterface<IInstanceAware>())
            {
                il.Emit(Codes.Load(field));
                il.Emit(Codes.This);
                il.Emit(Codes.Invoke(Context.Refs.InstanceAwareInstanceSet));
            }
        }

        return field;
    }

    /// <summary>
    /// Creates a singleton field used to retain an instance of an attribute.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable CreateAttributeSingleton(TypeEmitter emitter, CustomAttribute attribute)
    {
        var type = attribute.AttributeType;
        var name = $"<{type.Name}>k__Attribute";
        var field = emitter.EmitField(name, type, toStatic: true);

        var il = emitter.GetStaticConstructor().GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.Create(attribute.Constructor));
        il.Emit(Codes.Store(field));

        return field;
    }

    /// <summary>
    /// Creates a property which replaces a provided field, and implements the <c>get</c> and <c>set</c> methods.
    /// </summary>
    /// <param name="emitter">The type emitter.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    public PropertyEmitter CreateFieldProperty(TypeEmitter emitter, FieldDefinition field)
    {
        var variable = new Variable(field);
        var property = emitter.EmitProperty(field.Name, field.FieldType, field.IsStatic);
        var context = emitter.Context;

        field.Name = $"<{field.Name}>k__BackingField";

        context.AddCompilerGenerated(field);
        context.AddNonSerialized(field);

        var getter = property.GetGetter();
        var gil = getter.GetIL();
        
        gil.Emit(Codes.Nop);
        gil.Emit(Codes.ThisIf(variable));
        gil.Emit(Codes.Load(variable));
        gil.Emit(Codes.Return);

        var setter = property.GetSetter();
        var sil = setter.GetIL();

        sil.Emit(Codes.Nop);
        sil.Emit(Codes.ThisIf(variable));
        sil.Emit(Codes.Arg(field.IsStatic ? 0 : 1));
        sil.Emit(Codes.Store(variable));
        sil.Emit(Codes.Return);

        return property;
    }

    /// <summary>
    /// Create a singleton field used to store method information for the provided method.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <returns></returns>
    public Variable CreateMethodInfo(MethodEmitter method)
    {
        var parent = method.Parent;
        var type = parent.Target;

        var id = method.Target.GetHashString();
        var name = $"<{method.Target.Name}${id}>k__MethodInfo";
        var field = parent.EmitField(name, Context.Refs.MethodBase, toStatic: true);

        var il = parent.GetStaticConstructor().GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(type.HasGenericParameters ? method.Target : method.Target.GetGeneric()));
        il.Emit(Codes.LoadToken(type.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Refs.MethodBaseGetMethodFromHandleAndType));
        il.Emit(Codes.Store(field));

        return field;
    }

    /// <summary>
    /// Create a singleton field used to store parameter information for the provided method.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <param name="param">The parameter.</param>
    /// <returns></returns>
    public Variable CreateParameterInfo(MethodEmitter method, ParameterReference param)
    {
        var parent = method.Parent;
        var type = parent.Target;

        var id = method.Target.GetHashString();
        var name = $"<{method.Target.Name}${id}${param.Index}>k__ParameterInfo";
        var field = parent.EmitField(name, Context.Refs.PropertyInfo, toStatic: true);

        var il = parent.GetStaticConstructor().GetIL();
        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(type.HasGenericParameters ? method.Target : method.Target.GetGeneric()));
        il.Emit(Codes.LoadToken(type.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Refs.MethodBaseGetMethodFromHandleAndType));
        il.Emit(Codes.Invoke(Context.Refs.MethodBaseGetParameters));
        il.Emit(Codes.Int(param.Index));
        il.Emit(Codes.LoadArray);
        il.Emit(Codes.Store(field));

        return field;
    }

    /// <summary>
    /// Create a singleton field used to store property information for the provided property.
    /// </summary>
    /// <param name="property">The property weaver.</param>
    /// <returns></returns>
    public Variable CreatePropertyInfo(PropertyEmitter property)
    {
        var parent = property.Parent;
        var type = parent.Target;
        var sta = (property.Target.GetMethod ?? property.Target.SetMethod).IsStatic;

        var name = $"<{property.Target.Name}>k__PropertyInfo";
        var field = parent.EmitField(name, Context.Refs.PropertyInfo, toStatic: true);
        var flags = BindingFlags.NonPublic | BindingFlags.Public | (sta ? BindingFlags.Static : BindingFlags.Instance);

        var il = parent.GetStaticConstructor().GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(type.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Refs.TypeGetTypeFromHandle));
        il.Emit(Codes.String(property.Target.Name));
        il.Emit(Codes.Int((int)flags));
        il.Emit(Codes.Invoke(Context.Refs.TypeGetProperty));
        il.Emit(Codes.Store(field));

        return field;
    }
}