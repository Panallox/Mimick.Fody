using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;
using Mimick.Fody;
using Mimick.Fody.Weavers;
using Mono.Cecil;

/// <summary>
/// Weaves all types which are candidates for constructor interception.
/// </summary>
public partial class ModuleWeaver
{
    /// <summary>
    /// Weaves the constructor interceptors.
    /// </summary>
    public void WeaveConstructorInterceptors()
    {
        var candidates = Context.Candidates.FindTypeByConstructorInterceptors();

        foreach (var item in candidates)
        {
            var weaver = new TypeEmitter(ModuleDefinition, item.Type, Context);
            WeaveConstructorInterceptors(weaver, item.Constructors);
        }
    }

    /// <summary>
    /// Weaves the constructor interceptors for a provided type and interceptor information.
    /// </summary>
    /// <param name="weaver">The weaver.</param>
    /// <param name="item">The interceptors.</param>
    public void WeaveConstructorInterceptors(TypeEmitter weaver, ConstructorInterceptorInfo item)
    {
        var constructors = weaver.GetConstructors();

        foreach (var init in item.Initializers)
        {
            var after = init.GetAttribute<IInjectAfterInitializer>();
            var before = init.GetAttribute<IInjectBeforeInitializer>();

            foreach (var ctor in constructors)
            {
                var il = ctor.GetIL();
                var st = init.IsStatic;

                il.Insert = CodeInsertion.Before;
                il.Position = before != null ? il.GetFirst() : il.GetLast();

                if (!st)
                    il.Emit(Codes.This);

                var type = ctor.Parent.Target.GetGeneric();
                var method = (MethodReference)init;
                
                if (type.IsGenericInstance)
                {
                    var generic = (GenericInstanceType)type;
                    method = init.MakeGeneric(generic.GenericArguments.ToArray());
                }

                il.Emit(st ? Codes.InvokeStatic(method) : Codes.Invoke(method));

                if (method.IsReturn())
                    il.Emit(Codes.Pop);
            }

            if (after != null)
                init.CustomAttributes.Remove(after);

            if (before != null)
                init.CustomAttributes.Remove(before);
        }
    }
}
