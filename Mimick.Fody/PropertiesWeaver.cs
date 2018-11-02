﻿using Mimick.Aspect;
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
            var weaver = new TypeEmitter(Context.Module, item.Type, Context);

            foreach (var property in item.Properties)
                WeavePropertyInterceptors(weaver, property);
        }
    }

    /// <summary>
    /// Weaves the interception logic of a property.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="item"></param>
    public void WeavePropertyInterceptors(TypeEmitter parent, PropertyInterceptorInfo item)
    {
        var property = item.Property;
        var getter = property.GetMethod != null ? new MethodEmitter(parent, property.GetMethod) : null;
        var setter = property.SetMethod != null ? new MethodEmitter(parent, property.SetMethod) : null;
        var count = item.Interceptors.Length;
        var weaver = new PropertyEmitter(parent, property);

        if (count == 0)
            return;

        var gInterceptors = new Variable[count];
        var sInterceptors = new Variable[count];

        for (int i = 0; i < count; i++)
        {
            var variables = CreateAttribute(weaver, getter, setter, item.Interceptors[i]);

            gInterceptors[i] = variables[0];
            sInterceptors[i] = variables[1];
        }

        var hasGetter = gInterceptors.Any(i => i != null);
        var hasSetter = sInterceptors.Any(i => i != null);

        var type = property.PropertyType;
        var info = CreatePropertyInfo(weaver);
        var field = property.GetBackingField();
        var backing = (Variable)null;

        if (field != null)
            backing = new Variable(field.Resolve());

        if (getter != null && hasGetter)
        {
            var il = getter.GetIL();
            var result = getter.EmitLocal(property.PropertyType);

            il.Position = il.GetFirst();
            il.Insert = CodeInsertion.Before;
            
            if (backing != null)
            {
                il.Emit(Codes.ThisIf(backing));
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
            var cancel = il.EmitLabel();
            var args = il.EmitLocal(Context.Refs.PropertyInterceptionArgs);

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

                il.Emit(Codes.ThisIf(inc));
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

                il.Emit(Codes.ThisIf(inc));
                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertyGetInterceptorOnExit));
            }

            il.Mark(cancel);
            il.EndTry();

            il.Insert = CodeInsertion.After;

            if (setter != null || backing != null)
            {
                var unchanged = il.EmitLabel();

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
            var il = setter.GetIL();

            il.Position = il.GetFirst();
            il.Insert = CodeInsertion.Before;

            var args = il.EmitLocal(Context.Refs.PropertyInterceptionArgs);
            var cancel = il.EmitLabel();
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

                il.Emit(Codes.ThisIf(inc));
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

                il.Emit(Codes.ThisIf(inc));
                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(args));
                il.Emit(Codes.Invoke(Context.Refs.PropertySetInterceptorOnExit));
            }

            il.EndTry();
        }
    }
}