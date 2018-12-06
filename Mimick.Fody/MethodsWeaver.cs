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
            var weaver = new TypeEmitter(Context.Module, item.Type, Context);
                        
            foreach (var method in item.Methods)
            {
                var inner = new MethodEmitter(weaver, method.Method);
                WeaveMethodInterceptors(inner, method);
            }
        }
    }

    /// <summary>
    /// Weaves the interception logic of a method.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <param name="item">The method information.</param>
    public void WeaveMethodInterceptors(MethodEmitter weaver, MethodInterceptorInfo item)
    {
        var pAttributes = item.Parameters.SelectMany(p => p.Attributes.Select(a => new { p.Index, Attribute = a })).ToArray();
        var mAttributes = item.MethodInterceptors;
        var rAttributes = item.ReturnInterceptors;

        var pInterceptors = new Variable[pAttributes.Length];
        var mInterceptors = new Variable[item.MethodInterceptors.Length];
        var rInterceptors = new Variable[rAttributes.Length];

        var needsEnter = mAttributes.Any(m => m.HasRequiredMethod(Context.Finder.MethodInterceptorOnEnter));
        var needsCatch = mAttributes.Any(m => m.HasRequiredMethod(Context.Finder.MethodInterceptorOnException));
        var needsExit = mAttributes.Any(m => m.HasRequiredMethod(Context.Finder.MethodInterceptorOnExit));
        var needsParams = pAttributes.Any(m => m.Attribute.HasRequiredMethod(Context.Finder.ParameterInterceptorOnEnter));
        var needsReturns = rAttributes.Any(m => m.HasRequiredMethod(Context.Finder.MethodReturnInterceptorOnReturn));

        var needsMethodArgs = mAttributes.Any(m =>
        {
            return m.AttributeType.GetMethod(Context.Finder.MethodInterceptorOnEnter).UsesParameter(0) ||
                   m.AttributeType.GetMethod(Context.Finder.MethodInterceptorOnException).UsesParameter(0) ||
                   m.AttributeType.GetMethod(Context.Finder.MethodInterceptorOnExit).UsesParameter(0);
        });

        var needsParamArgs = pAttributes.Any(m => m.Attribute.AttributeType.GetMethod(Context.Finder.ParameterInterceptorOnEnter).UsesParameter(0));
        var needsReturnsArgs = rAttributes.Any(m => m.AttributeType.GetMethod(Context.Finder.MethodReturnInterceptorOnReturn).UsesParameter(0));

        var needsMethodCopyArgs = mAttributes.Any(m => m.GetAttribute(Context.Finder.CompilationOptionsAttribute)?.GetProperty("CopyArguments", notFound: false) ?? false);
                                
        if (!needsEnter && !needsCatch && !needsExit && !needsParams && !needsReturns)
            return;

        var method = weaver.Target;
        var il = weaver.GetIL();

        il.Position = il.GetFirst();
        il.Insert = CodeInsertion.Before;

        var result = (Variable)null;

        if (weaver.Target.IsReturn())
        {
            var type = weaver.Target.ReturnType;

            result = weaver.EmitLocal(type);

            if (type.IsValueType)
            {
                il.Emit(Codes.Address(result));
                il.Emit(Codes.Init(type));
            }
            else
            {
                il.Emit(Codes.Null);
                il.Emit(Codes.Store(result));
            }
        }

        var leave = WeaveMethodReturnsRoute(weaver, result, mInterceptors.Length > 0);
        var cancel = il.EmitLabel();
        
        for (int i = 0, count = pInterceptors.Length; i < count; i++)
            pInterceptors[i] = CreateAttribute(weaver, pAttributes[i].Attribute);

        for (int i = 0, count = mInterceptors.Length; i < count; i++)
            mInterceptors[i] = CreateAttribute(weaver, mAttributes[i]);

        for (int i = 0, count = rInterceptors.Length; i < count; i++)
            rInterceptors[i] = CreateAttribute(weaver, rAttributes[i]);
        
        foreach (var attribute in mAttributes)
            weaver.Target.CustomAttributes.Remove(attribute);

        foreach (var attribute in rAttributes)
            weaver.Target.MethodReturnType.CustomAttributes.Remove(attribute);

        foreach (var parameter in item.Parameters)
        {
            foreach (var attribute in parameter.Attributes)
            {
                if (parameter.Index == -1)
                    weaver.Target.CustomAttributes.Remove(attribute);
                else
                    weaver.Target.Parameters[parameter.Index].CustomAttributes.Remove(attribute);
            }
        }

        var hasMethod = mInterceptors.Length > 0;
        var hasParams = pInterceptors.Length > 0;
        var hasReturn = rInterceptors.Length > 0;

        var arguments = hasMethod ? CreateArgumentArray(weaver) : null;
        var invocation = CreateMethodInfo(weaver);
        var mEventArgs = hasMethod && needsMethodArgs ? weaver.EmitLocal(Context.Finder.MethodInterceptionArgs, name: "methodEventArgs") : null;
        
        if (hasMethod || hasReturn)
        {
            if (hasMethod && needsMethodArgs)
            {
                il.Emit(method.IsStatic ? Codes.Null : Codes.This);
                il.Emit(Codes.Load(arguments));

                if (result == null)
                    il.Emit(Codes.Null);
                else
                {
                    il.Emit(Codes.Load(result));
                    il.Emit(Codes.Box(result.Type));
                }

                il.Emit(Codes.Load(invocation));
                il.Emit(Codes.Create(Context.Finder.MethodInterceptionArgsCtor));
                il.Emit(Codes.Store(mEventArgs));
            }

            il.Try();
        }

        if (hasParams && needsParams)
        {
            var pEventArgs = needsParamArgs ? weaver.EmitLocal(Context.Finder.ParameterInterceptionArgs) : null;

            for (int i = 0, count = pInterceptors.Length; i < count; i++)
            {
                var inc = pInterceptors[i];
                var parameters = pAttributes[i].Index == -1 ? method.Parameters.ToArray() : new[] { method.Parameters[pAttributes[i].Index] };

                for (int j = 0, pCount = parameters.Length; j < pCount; j++)
                {
                    var prm = parameters[j];
                    var pVariable = new Variable(prm);
                    var pInfo = CreateParameterInfo(weaver, prm);
                    var needsCopyArgs = pAttributes[i].Attribute.GetAttribute(Context.Finder.CompilationOptionsAttribute)?.GetProperty("CopyArguments", notFound: false) ?? false;

                    if (needsParamArgs)
                    {
                        il.Emit(method.IsStatic ? Codes.Null : Codes.This);
                        il.Emit(Codes.Load(pInfo));
                        il.Emit(Codes.Arg(pVariable));
                        il.Emit(Codes.Box(prm.ParameterType));
                        il.Emit(Codes.Create(Context.Finder.ParameterInterceptionArgsCtor));
                        il.Emit(Codes.Store(pEventArgs));
                    }
                    
                    il.Emit(Codes.ThisIf(inc));
                    il.Emit(Codes.Load(inc));

                    if (needsParamArgs)
                        il.Emit(Codes.Load(pEventArgs));
                    else
                        il.Emit(Codes.Null);

                    il.Emit(Codes.Invoke(Context.Finder.ParameterInterceptorOnEnter));

                    if (needsParamArgs && !prm.IsOut && needsCopyArgs)
                    {
                        il.Emit(Codes.Load(pEventArgs));
                        il.Emit(Codes.Invoke(Context.Finder.ParameterInterceptionArgsValueGet));
                        il.Emit(Codes.Unbox(prm.ParameterType));
                        il.Emit(Codes.Store(pVariable));
                    }
                }
            }
        }

        if (hasMethod || hasReturn)
        {
            if (hasMethod && needsEnter)
            {
                for (int i = 0, count = mInterceptors.Length; i < count; i++)
                {
                    var inc = mInterceptors[i];
                    var att = mAttributes[i];

                    if (att.HasRequiredMethod("OnEnter"))
                    {
                        il.Emit(Codes.ThisIf(inc));
                        il.Emit(Codes.Load(inc));

                        if (needsMethodArgs)
                            il.Emit(Codes.Load(mEventArgs));
                        else
                            il.Emit(Codes.Null);

                        il.Emit(Codes.Invoke(Context.Finder.MethodInterceptorOnEnter));

                        if (needsMethodArgs)
                        {
                            il.Emit(Codes.Load(mEventArgs));
                            il.Emit(Codes.Invoke(Context.Finder.MethodInterceptionArgsCancelGet));
                            il.Emit(Codes.IfTrue(cancel));
                        }
                    }
                }

                if (needsMethodArgs && needsMethodCopyArgs && arguments != null)
                    CopyArgumentArrayToParameters(weaver, arguments);
            }

            il.Position = leave;

            if (hasMethod && needsCatch)
            { 
                var exception = il.EmitLocal(Context.Finder.Exception);

                il.Catch(exception);

                for (int i = 0, count = mInterceptors.Length; i < count; i++)
                {
                    var inc = mInterceptors[i];
                    var att = mAttributes[i];

                    if (att.HasRequiredMethod("OnException"))
                    {
                        il.Emit(Codes.ThisIf(inc));
                        il.Emit(Codes.Load(inc));

                        if (needsMethodArgs)
                            il.Emit(Codes.Load(mEventArgs));
                        else
                            il.Emit(Codes.Null);

                        il.Emit(Codes.Load(exception));
                        il.Emit(Codes.Invoke(Context.Finder.MethodInterceptorOnException));
                    }
                }

                il.Emit(Codes.Nop);
                il.Emit(Codes.Leave(leave));
            }

            if (needsExit || needsReturns)
                il.Finally(leave);

            if (hasMethod)
            {
                if (result != null && needsMethodArgs)
                {
                    il.Emit(Codes.Load(mEventArgs));
                    il.Emit(Codes.Load(result));
                    il.Emit(Codes.Box(weaver.Target.ReturnType));
                    il.Emit(Codes.Invoke(Context.Finder.MethodInterceptionArgsReturnSet));
                }

                if (needsExit)
                {
                    for (int i = 0, count = mInterceptors.Length; i < count; i++)
                    {
                        var inc = mInterceptors[i];
                        var att = mAttributes[i];

                        if (att.HasRequiredMethod("OnExit"))
                        {
                            il.Emit(Codes.ThisIf(inc));
                            il.Emit(Codes.Load(inc));

                            if (needsMethodArgs)
                                il.Emit(Codes.Load(mEventArgs));
                            else
                                il.Emit(Codes.Null);

                            il.Emit(Codes.Invoke(Context.Finder.MethodInterceptorOnExit));

                            if (needsMethodArgs)
                            {
                                il.Emit(Codes.Load(mEventArgs));
                                il.Emit(Codes.Invoke(Context.Finder.MethodInterceptionArgsCancelGet));
                                il.Emit(Codes.IfTrue(cancel));
                            }
                        }
                    }
                }

                il.Mark(cancel);
            }

            if (hasReturn && needsReturns && result != null)
            {
                var rEventArgs = needsReturnsArgs ? il.EmitLocal(Context.Finder.MethodReturnInterceptionArgs) : null;

                if (needsReturnsArgs)
                {
                    il.Emit(method.IsStatic ? Codes.Null : Codes.This);
                    il.Emit(Codes.Load(result));
                    il.Emit(Codes.Box(result.Type));
                    il.Emit(Codes.Load(invocation));
                    il.Emit(Codes.Create(Context.Finder.MethodReturnInterceptionArgsCtor));
                    il.Emit(Codes.Store(rEventArgs));
                }

                for (int i = 0, count = rInterceptors.Length; i < count; i++)
                {
                    var inc = rInterceptors[i];
                    var att = rAttributes[i];

                    if (att.HasRequiredMethod("OnReturn"))
                    {
                        il.Emit(Codes.ThisIf(inc));
                        il.Emit(Codes.Load(inc));

                        if (needsReturnsArgs)
                            il.Emit(Codes.Load(rEventArgs));
                        else
                            il.Emit(Codes.Null);

                        il.Emit(Codes.Invoke(Context.Finder.MethodReturnInterceptorOnReturn));
                    }
                }

                if (needsReturnsArgs)
                {
                    il.Emit(Codes.Load(rEventArgs));
                    il.Emit(Codes.Invoke(Context.Finder.MethodReturnInterceptionArgsValueGet));
                    il.Emit(Codes.Unbox(result.Type));
                    il.Emit(Codes.Store(result));
                }
            }

            il.EndTry();
            il.Insert = CodeInsertion.After;

            if (hasMethod && arguments != null)
                CopyArgumentArrayToReferences(weaver, arguments);

            if (hasMethod && needsMethodArgs && result != null)
            {
                il.Emit(Codes.Nop);
                il.Emit(Codes.Load(mEventArgs));
                il.Emit(Codes.Invoke(Context.Finder.MethodInterceptionArgsReturnGet));
                il.Emit(Codes.Unbox(weaver.Target.ReturnType));
            }
        }
    }
    
    /// <summary>
    /// Weaves the return instructions of the provided method to leave to a new instruction.
    /// </summary>
    /// <param name="weaver">The method weaver.</param>
    /// <param name="storage">The storage variable for any captured return value.</param>
    /// <param name="hasMethodInterceptors">Whether method interceptors are present which could change the return value.</param>
    /// <returns>The instruction now located at the tail of the method.</returns>
    public Instruction WeaveMethodReturnsRoute(MethodEmitter weaver, Variable storage, bool hasMethodInterceptors)
    {
        var il = weaver.Body.Instructions;
        var pos = weaver.GetIL().Position;

        if (weaver.Target.IsReturn())
        {
            var instruction = hasMethodInterceptors ? Codes.Nop : Codes.Load(storage);

            il.Add(instruction);
            il.Add(Codes.Return);

            for (int i = 0; i < il.Count - 2; i++)
            {
                if (il[i].OpCode == OpCodes.Ret)
                {
                    var current = il[i] == pos;
                    var replacement = il[i] = Codes.Leave(instruction);

                    if (current)
                        weaver.GetIL().Position = replacement;

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
                {
                    var current = il[i] == pos;
                    var replacement = il[i] = Codes.Leave(instruction);

                    if (current)
                        weaver.GetIL().Position = replacement;
                }
            }

            return instruction;
        }
    }
}