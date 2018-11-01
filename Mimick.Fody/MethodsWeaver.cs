using Mimick.Aspect;
using Mimick.Fody;
using Mimick.Fody.Weavers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A partial definition for the foundation weaver which handles the weaving any methods.
/// </summary>
public partial class ModuleWeaver
{
    /// <summary>
    /// Weaves all types which are candidates for method interception.
    /// </summary>
    public void WeaveMethodInterceptors()
    {
        var candidates = Context.Candidates.FindTypeByMethodInterceptors();

        foreach (var item in candidates)
        {
            var weaver = new TypeWeaver(Context.Module, item.Type, Context);
            
            foreach (var method in item.Methods)
            {
                var inner = new MethodWeaver(weaver, method.Method);
                WeaveMethodInterceptors(inner, method);
            }
        }
    }

    /// <summary>
    /// Weaves the interception logic of a method.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <param name="item">The method information.</param>
    public void WeaveMethodInterceptors(MethodWeaver weaver, MethodInterceptorInfo item)
    {
        var pAttributes = item.ParameterInterceptors;
        var mAttributes = item.MethodInterceptors;

        var pInterceptors = new Variable[item.ParameterInterceptors.Length];
        var mInterceptors = new Variable[item.MethodInterceptors.Length];

        var method = weaver.Target;
        var il = weaver.GetWeaver();

        il.Position = il.GetFirst();
        il.Insert = CodeInsertion.Before;

        var result = (Variable)null;

        if (weaver.Target.IsReturn())
        {
            var type = weaver.Target.ReturnType;

            result = weaver.CreateVariable(type);

            il.Emit(type.IsValueType ? Codes.Init(type) : Codes.Null);
            il.Emit(Codes.Store(result));
        }

        var declaring = weaver.Parent.Target.HasGenericParameters ? (TypeReference)weaver.Parent.Target.MakeGenericInstanceType(weaver.Parent.Target.GenericParameters.ToArray()) : weaver.Parent.Target;
        var leave = WeaveMethodReturnsRoute(weaver, result, mInterceptors.Length > 0);
        var cancel = il.CreateLabel();

        for (int i = 0, count = pInterceptors.Length; i < count; i++)
            pInterceptors[i] = WeaveMethodAttributeVariable(weaver, pAttributes[i]);

        for (int i = 0, count = mInterceptors.Length; i < count; i++)
            mInterceptors[i] = WeaveMethodAttributeVariable(weaver, mAttributes[i]);

        var hasMethod = mInterceptors.Length > 0;
        var hasParams = pInterceptors.Length > 0;

        var arguments = hasMethod ? WeaveMethodArgumentsArray(weaver) : null;
        var invocation = WeaveMethodBaseVariable(weaver);
        var mEventArgs = hasMethod ? weaver.CreateVariable(Context.Refs.MethodInterceptionArgs, name: "methodEventArgs") : null;
        
        if (hasMethod)
        {
            il.Emit(method.IsStatic ? Codes.Null : Codes.This);
            il.Emit(Codes.Load(arguments));
            il.Emit(Codes.Null);
            il.Emit(Codes.Load(invocation));
            il.Emit(Codes.Create(Context.Refs.MethodInterceptionArgsCtor));
            il.Emit(Codes.Store(mEventArgs));
            il.Try();
        }

        if (hasParams)
        {
            var pEventArgs = weaver.CreateVariable(Context.Refs.ParameterInterceptionArgs);

            for (int i = 0, count = pInterceptors.Length; i < count; i++)
            {
                var inc = pInterceptors[i];
                var prm = method.Parameters[i];
                var pVariable = new Variable(prm);
                var pInfo = WeaveMethodParameterVariable(weaver, prm);

                il.Emit(method.IsStatic ? Codes.Null : Codes.This);
                il.Emit(Codes.Load(pInfo));
                il.Emit(Codes.Arg(pVariable));
                il.Emit(Codes.Box(prm.ParameterType));
                il.Emit(Codes.Create(Context.Refs.ParameterInterceptionArgsCtor));
                il.Emit(Codes.Store(pEventArgs));

                if (inc.IsThisNeeded)
                    il.Emit(Codes.This);

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(pEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.ParameterInterceptorOnEnter));

                il.Emit(Codes.Load(pEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.ParameterInterceptionArgsValueGet));
                il.Emit(Codes.Unbox(prm.ParameterType));
                il.Emit(Codes.Store(pVariable));
            }
        }

        if (hasMethod)
        {
            for (int i = 0, count = mInterceptors.Length; i < count; i++)
            {
                var inc = mInterceptors[i];

                if (inc.IsThisNeeded)
                    il.Emit(Codes.This);

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptorOnEnter));

                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptionArgsCancelGet));
                il.Emit(Codes.IfTrue(cancel));
            }

            var exception = il.CreateLocal(Context.Refs.Exception);

            il.Position = leave;
            il.Catch(exception);

            for (int i = 0, count = mInterceptors.Length; i < count; i++)
            {
                var inc = mInterceptors[i];

                if (inc.IsThisNeeded)
                    il.Emit(Codes.This);

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Load(exception));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptorOnException));
            }

            il.Emit(Codes.Nop);
            il.Emit(Codes.Leave(leave));

            il.Finally(leave);

            if (result != null)
            {
                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Load(result));
                il.Emit(Codes.Box(weaver.Target.ReturnType));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptionArgsReturnSet));
            }

            for (int i = 0, count = mInterceptors.Length; i < count; i++)
            {
                var inc = mInterceptors[i];

                if (inc.IsThisNeeded)
                    il.Emit(Codes.This);

                il.Emit(Codes.Load(inc));
                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptorOnExit));

                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptionArgsCancelGet));
                il.Emit(Codes.IfTrue(cancel));
            }

            il.Mark(cancel);
            il.EndTry();

            if (result != null)
            {
                il.Insert = CodeInsertion.After;
                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Invoke(Context.Refs.MethodInterceptionArgsReturnGet));
                il.Emit(Codes.Unbox(weaver.Target.ReturnType));
            }
        }
    }
    
    /// <summary>
    /// Weaves an argument array to contain the parameters of the method.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <returns></returns>
    public Variable WeaveMethodArgumentsArray(MethodWeaver weaver)
    {
        var parameters = weaver.Target.Parameters;
        var count = parameters.Count;
        var variable = weaver.CreateVariable(Context.Refs.ObjectArray);
        var il = weaver.GetWeaver();

        il.Emit(Codes.Int(count));
        il.Emit(Codes.CreateArray(TypeSystem.ObjectReference));
        il.Emit(Codes.Store(variable));

        for (int i = 0; i < count; i++)
        {
            var p = parameters[i];

            il.Emit(Codes.Load(variable));
            il.Emit(Codes.Int(i));
            il.Emit(Codes.Arg(p));
            il.Emit(Codes.Box(p.ParameterType));
            il.Emit(Codes.StoreArray);
        }

        return variable;
    }

    /// <summary>
    /// Weaves a variable store for a provided custom attribute.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable WeaveMethodAttributeVariable(MethodWeaver weaver, CustomAttribute attribute)
    {
        var options = attribute.GetAttribute<CompilationOptionsAttribute>();
        var scope = options.GetProperty("Scope", notFound: AttributeScope.Singleton);

        if (scope == AttributeScope.Instanced && weaver.Target.IsStatic)
            scope = AttributeScope.Singleton;

        switch (scope)
        {
            case AttributeScope.Adhoc:
                var variable = weaver.CreateVariable(attribute.AttributeType);
                var il = weaver.GetWeaver();
                il.Emit(Codes.Create(attribute.Constructor));
                il.Emit(Codes.Store(variable));
                return variable;

            case AttributeScope.Instanced:
                var field = weaver.Parent.CreateField($"<>__interceptor${attribute.AttributeType.Name}", attribute.AttributeType);
                var ctors = weaver.Parent.GetConstructors();
                var def = (FieldDefinition)field;
                weaver.Parent.Context.AddCompilerGenerated(def);
                weaver.Parent.Context.AddNonSerialized(def);
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
                return field;

            case AttributeScope.Singleton:
                var sfield = weaver.Parent.CreateField($"<>__interceptor${attribute.AttributeType.Name}", attribute.AttributeType, toStatic: true);
                var sil = weaver.Parent.GetStaticConstructor().GetWeaver();
                var sdef = (FieldDefinition)sfield;
                weaver.Parent.Context.AddCompilerGenerated(sdef);
                weaver.Parent.Context.AddNonSerialized(sdef);
                sil.Emit(Codes.Nop);
                sil.Emit(Codes.Create(attribute.Constructor));
                sil.Emit(Codes.Store(sdef));
                return sfield;
        }

        throw new NotSupportedException($"Cannot add an attribute storage for attribute '{attribute.AttributeType.FullName}'");
    }

    /// <summary>
    /// Weave a variable store for the provided method.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <returns></returns>
    public Variable WeaveMethodBaseVariable(MethodWeaver weaver)
    {
        var parent = weaver.Parent;
        var type = parent.Target;

        var id = weaver.Target.GetHashString();
        var field = weaver.Parent.CreateField($"<>__method{weaver.Target.Name}{id}", Context.Refs.MethodInfo, toStatic: true);

        var def = (FieldDefinition)field;

        weaver.Parent.Context.AddCompilerGenerated(def);
        weaver.Parent.Context.AddNonSerialized(def);

        var il = weaver.Parent.GetStaticConstructor().GetWeaver();

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(weaver.Parent.Target.HasGenericParameters ? weaver.Target : weaver.Target.GetGeneric()));
        il.Emit(Codes.LoadToken(weaver.Parent.Target.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Refs.MethodBaseGetMethodFromHandleAndType));
        il.Emit(Codes.Cast(Context.Refs.MethodInfo));
        il.Emit(Codes.Store(def.GetGeneric()));

        return field;
    }

    /// <summary>
    /// Weave a variable store for the provided parameter.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    public Variable WeaveMethodParameterVariable(MethodWeaver weaver, ParameterDefinition parameter)
    {
        var id = weaver.Target.GetHashString();
        var field = weaver.Parent.CreateField($"<>__parameter{weaver.Target.Name}{id}${parameter.Index}", Context.Refs.ParameterInfo, toStatic: true);
        var il = weaver.Parent.GetStaticConstructor().GetWeaver();

        var def = (FieldDefinition)field;
        weaver.Parent.Context.AddCompilerGenerated(def);
        weaver.Parent.Context.AddNonSerialized(def);

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(weaver.Target.GetGeneric()));
        il.Emit(Codes.LoadToken(weaver.Parent.Target.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Refs.MethodBaseGetMethodFromHandleAndType));
        il.Emit(Codes.Invoke(Context.Refs.MethodBaseGetParameters));
        il.Emit(Codes.Int(parameter.Index));
        il.Emit(Codes.LoadArray);
        il.Emit(Codes.Store(def.GetGeneric()));

        return field;
    }

    /// <summary>
    /// Weaves the return instructions of the provided method to leave to a new instruction.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <param name="storage">The storage variable for any captured return value.</param>
    /// <param name="hasMethodInterceptors">Whether method interceptors are present which could change the return value.</param>
    /// <returns>The instruction now located at the tail of the method.</returns>
    public Instruction WeaveMethodReturnsRoute(MethodWeaver weaver, Variable storage, bool hasMethodInterceptors)
    {
        var il = weaver.Body.Instructions;

        if (weaver.Target.IsReturn())
        {
            var instruction = hasMethodInterceptors ? Codes.Nop : Codes.Load(storage);

            il.Add(instruction);
            il.Add(Codes.Return);

            for (int i = 0; i < il.Count - 2; i++)
            {
                if (il[i].OpCode == OpCodes.Ret)
                {
                    il[i] = Codes.Leave(instruction);
                    il.Insert(i, Codes.Store(storage));
                    i++;
                }
            }

            return instruction;
        }
        else
        {
            var instruction = Codes.Return;

            il.Add(instruction);

            for (int i = 0, count = il.Count - 1; i < count; i++)
            {
                if (il[i].OpCode == OpCodes.Ret)
                    il[i] = Codes.Leave(instruction);
            }

            return instruction;
        }
    }
}