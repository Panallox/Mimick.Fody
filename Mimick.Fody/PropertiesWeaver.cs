using Mimick.Aspect;
using Mimick.Fody;
using Mimick.Fody.Weavers;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Weaves all types which are candidates for property interception.
/// </summary>
public partial class ModuleWeaver
{
    /// <summary>
    /// Weaves all types which are candidates for property interception.
    /// </summary>
    public void WeavePropertyInterceptors()
    {
        var candidates = Context.Candidates.FindTypeByPropertyInterceptors();

        foreach (var item in candidates)
        {
            var weaver = new TypeWeaver(Context.Module, item.Type, Context);

            foreach (var property in item.Properties)
                WeavePropertyInterceptors(weaver, property);
        }
    }

    /// <summary>
    /// Weaves the interception logic of a property.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="item"></param>
    public void WeavePropertyInterceptors(TypeWeaver parent, PropertyInterceptorInfo item)
    {
        var property = item.Property;
        var getter = property.GetMethod != null ? new MethodWeaver(parent, property.GetMethod) : null;
        var setter = property.SetMethod != null ? new MethodWeaver(parent, property.SetMethod) : null;
        var count = item.Interceptors.Length;

        if (count == 0)
            return;

        var gInterceptors = new Variable[count];
        var sInterceptors = new Variable[count];

        for (int i = 0; i < count; i++)
        {
            var variables = WeavePropertyAttributeVariable(item.Property, getter, setter, item.Interceptors[i]);

            gInterceptors[i] = variables[0];
            sInterceptors[i] = variables[1];
        }

        var hasGetter = gInterceptors.Any(i => i != null);
        var hasSetter = sInterceptors.Any(i => i != null);

        var type = property.PropertyType;
        var info = WeavePropertyVariable(property, parent);
        var field = property.GetBackingField();
        var backing = (Variable)null;

        if (field != null)
            backing = new Variable(field.Resolve());

        if (getter != null && hasGetter)
        {
            var il = getter.GetWeaver();
            var result = getter.CreateVariable(property.PropertyType);

            il.Position = il.GetFirst();
            il.Insert = CodeInsertion.Before;
            
            if (backing != null)
            {
                if (backing.IsThisNeeded)
                    il.Emit(Codes.This);

                il.Emit(Codes.Load(backing));
                il.Emit(Codes.Store(result));
            }
            else
            {
                il.Emit(type.IsValueType ? Codes.Init(type) : Codes.Null);
                il.Emit(Codes.Store(result));
            }

            il.Try();

            var leave = WeaveMethodReturnsRoute(getter, result, true);
            var cancel = il.CreateLabel();
            var args = il.CreateLocal(Context.Refs.PropertyInterceptionArgs);

            il.Emit(getter.Target.IsStatic ? Codes.Null : Codes.This);
            il.Emit(Codes.Load(info));
            il.Emit(Codes.Load(result));
            il.Emit(Codes.Box(type));
            il.Emit(Codes.Create(Context.Refs.PropertyInterceptionArgsCtor));
            il.Emit(Codes.Store(args));

            for (int i = 0; i < count; i++)
            {
                var inc = gInterceptors[i];

                if (inc == null)
                    continue;

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertyGetInterceptorOnGet));
            }

            il.Position = leave;
            il.Finally();

            for (int i = 0; i < count; i++)
            {
                var inc = gInterceptors[i];

                if (inc == null)
                    continue;

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertyGetInterceptorOnExit));
            }

            il.Mark(cancel);
            il.EndTry();

            il.Insert = CodeInsertion.After;

            if (setter != null || backing != null)
            {
                var unchanged = il.CreateLabel();

                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertyInterceptionArgsIsDirtyGet));
                il.Emit(Codes.IfFalse(unchanged));
                
                if (!setter.Target.IsStatic)
                    il.Emit(Codes.This);

                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertyInterceptionArgsValueGet));
                il.Emit(Codes.Unbox(type));

                if (backing != null)
                    il.Emit(Codes.Store(backing));
                else
                    il.Emit(Codes.Invoke(setter.Target));

                il.Emit(Codes.Nop);
                il.Mark(unchanged);
            }

            il.Emit(Codes.Load(args));
            il.Emit(Codes.Invoke(Context.Refs.PropertyInterceptionArgsValueGet));
            il.Emit(Codes.Unbox(type));
        }

        if (setter != null && hasSetter)
        {
            var il = setter.GetWeaver();

            il.Position = il.GetFirst();
            il.Insert = CodeInsertion.Before;

            var args = il.CreateLocal(Context.Refs.PropertyInterceptionArgs);
            var cancel = il.CreateLabel();
            var argument = new Variable(setter.Target.Parameters.First());

            il.Try();

            il.Emit(setter.Target.IsStatic ? Codes.Null : Codes.This);
            il.Emit(Codes.Load(info));
            il.Emit(Codes.Load(argument));
            il.Emit(Codes.Box(type));
            il.Emit(Codes.Create(Context.Refs.PropertyInterceptionArgsCtor));
            il.Emit(Codes.Store(args));

            for (int i = 0; i < count; i++)
            {
                var inc = sInterceptors[i];

                if (inc == null)
                    continue;

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertySetInterceptorOnSet));
            }

            il.Emit(Codes.Load(args));
            il.Emit(Codes.Invoke(Context.Refs.PropertyInterceptionArgsIsDirtyGet));
            il.Emit(Codes.IfFalse(cancel));

            il.Emit(Codes.Load(args));
            il.Emit(Codes.Invoke(Context.Refs.PropertyInterceptionArgsValueGet));
            il.Emit(Codes.Unbox(argument.Type));
            il.Emit(Codes.Store(argument));
            il.Emit(Codes.Nop);

            il.Position = il.Position.Previous;
            il.Mark(cancel);

            il.Position = il.GetLast();
            il.Emit(Codes.Leave(il.Position));
            il.Finally();

            for (int i = 0; i < count; i++)
            {
                var inc = sInterceptors[i];

                if (inc == null)
                    continue;

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertySetInterceptorOnExit));
            }

            il.EndTry();
        }
    }

    /// <summary>
    /// Weave a variable store for a provided custom attribute.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="getter">The getter method weaver.</param>
    /// <param name="setter">The setter method weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable[] WeavePropertyAttributeVariable(PropertyDefinition property, MethodWeaver getter, MethodWeaver setter, CustomAttribute attribute)
    {
        var options = attribute.GetAttribute<CompilationOptionsAttribute>();
        var scope = options.GetProperty("Scope", notFound: AttributeScope.Singleton);
        var isStatic = (getter ?? setter).Target.IsStatic;
        var context = (getter ?? setter).Parent.Context;

        if (scope == AttributeScope.Instanced && isStatic)
            scope = AttributeScope.Singleton;

        var isGet = attribute.HasInterface<IPropertyGetInterceptor>();
        var isSet = attribute.HasInterface<IPropertySetInterceptor>();

        switch (scope)
        {
            case AttributeScope.Instanced:
                var field = (getter ?? setter).Parent.CreateField($"<>__interceptor${attribute.AttributeType.Name}", attribute.AttributeType);
                var ctors = (getter ?? setter).Parent.GetConstructors();
                var def = (FieldDefinition)field;
                context.AddCompilerGenerated(def);
                context.AddNonSerialized(def);
                foreach (var ctor in ctors)
                {
                    var cil = ctor.GetWeaver();
                    cil.Insert = CodeInsertion.Before;
                    cil.Position = cil.GetFirst();
                    cil.Emit(Codes.Nop);
                    cil.Emit(Codes.This);
                    cil.Emit(Codes.Create(attribute.Constructor));
                    cil.Emit(Codes.Store(field));
                }
                return new[] { isGet ? field : null, isSet ? field : null };

            case AttributeScope.Singleton:
                var sfield = (getter ?? setter).Parent.CreateField($"<>__interceptor${attribute.AttributeType.Name}", attribute.AttributeType, toStatic: true);
                var sil = (getter ?? setter).Parent.GetStaticConstructor().GetWeaver();
                var sdef = (FieldDefinition)sfield;
                context.AddCompilerGenerated(sdef);
                context.AddNonSerialized(sdef);
                sil.Emit(Codes.Nop);
                sil.Emit(Codes.Create(attribute.Constructor));
                sil.Emit(Codes.Store(sfield));
                return new[] { isGet ? sfield : null, isSet ? sfield : null };
        }

        var variables = new Variable[2];
        var methods = new[] { getter, setter };

        for (int i = 0; i < 2; i++)
        {
            if (i == 0 && !isGet)
                continue;
            if (i == 1 && !isSet)
                continue;

            var method = methods[i];

            if (method == null)
                continue;

            var il = method.GetWeaver();

            variables[i] = method.CreateVariable(attribute.AttributeType);
            il.Emit(Codes.Nop);
            il.Emit(Codes.Create(attribute.Constructor));
            il.Emit(Codes.Store(variables[i]));
        }

        return variables;
    }

    /// <summary>
    /// Weave a variable store for the provided property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="weaver">The type weaver.</param>
    /// <returns></returns>
    public Variable WeavePropertyVariable(PropertyDefinition property, TypeWeaver weaver)
    {
        var field = weaver.CreateField($"<>__property{property.Name}", Context.Refs.PropertyInfo, toStatic: true);
        var il = weaver.GetStaticConstructor().GetWeaver();
        var flags = BindingFlags.NonPublic | BindingFlags.Public;
        var isStatic = (property.GetMethod ?? property.SetMethod).IsStatic;
        var def = (FieldDefinition)field;

        weaver.Context.AddCompilerGenerated(def);
        weaver.Context.AddNonSerialized(def);

        flags |= isStatic ? BindingFlags.Static : BindingFlags.Instance;

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(weaver.Target));
        il.Emit(Codes.InvokeStatic(Context.Refs.TypeGetTypeFromHandle));
        il.Emit(Codes.String(property.Name));
        il.Emit(Codes.Int((int)flags));
        il.Emit(Codes.Invoke(Context.Refs.TypeGetProperty));
        il.Emit(Codes.Store(field));

        return field;
    }
}
