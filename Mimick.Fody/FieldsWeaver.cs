using Mimick.Fody;
using Mimick.Fody.Weavers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Weaves all types which are candidates for field interception.
/// </summary>
public partial class ModuleWeaver
{
    /// <summary>
    /// Weaves all types which are candidates for field interception.
    /// </summary>
    public void WeaveFieldInterceptors()
    {
        var candidates = Context.Candidates.FindTypeByFieldInterceptors();

        foreach (var item in candidates)
        {
            var weaver = new TypeEmitter(Context.Module, item.Type, Context);

            foreach (var field in item.Fields)
            {
                var replacement = WeaveFieldReplacements(weaver, field);
                var interceptor = new PropertyInterceptorInfo { Interceptors = field.Interceptors, Property = replacement };
                WeavePropertyInterceptors(weaver, interceptor);
            }
        }
    }

    /// <summary>
    /// Weaves the replacement accessors for a particular field.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="item">The interceptor information.</param>
    public PropertyDefinition WeaveFieldReplacements(TypeEmitter emitter, FieldInterceptorInfo item)
    {
        var field = item.Field;
        var path = field.FullName;

        var property = CreateFieldProperty(emitter, field);

        var getter = property.GetGetter();
        var setter = property.GetSetter();

        var search = field.IsPrivate ? (field.DeclaringType.HasAsyncStateMachine() ? emitter.Target.GetMethodsInNested() : emitter.Target.Methods) : Context.Candidates.SelectMany(a => a.Resolve().Methods);

        foreach (var method in search)
        {
            if (method == getter.Target || method == setter.Target)
                continue;

            var il = method.Body.Instructions.Where(i => i.OpCode == OpCodes.Ldfld || i.OpCode == OpCodes.Ldsfld || i.OpCode == OpCodes.Stfld || i.OpCode == OpCodes.Stsfld);

            foreach (var i in il)
            {
                var reference = i.Operand as FieldReference ?? i.Operand as FieldDefinition;
                                
                if (reference == null || (!reference.IsGenericMatch(path) && !reference.IsGenericMatch(field.FullName)))
                    continue;

                var declaring = reference.DeclaringType;
                var get = (MethodReference)getter.Target;
                var set = (MethodReference)setter.Target;

                if (declaring.IsGenericInstance)
                {
                    var generic = (GenericInstanceType)declaring;

                    get = get.MakeGeneric(generic.GenericArguments.ToArray());
                    set = set.MakeGeneric(generic.GenericArguments.ToArray());
                }
                
                if (i.OpCode == OpCodes.Ldfld || i.OpCode == OpCodes.Ldsfld)
                {
                    i.OpCode = getter.Target.IsAbstract && !getter.Target.IsStatic ? OpCodes.Callvirt : OpCodes.Call;
                    i.Operand = get;
                }
                else if (i.OpCode == OpCodes.Stfld || i.OpCode == OpCodes.Stsfld)
                {
                    i.OpCode = setter.Target.IsAbstract && !getter.Target.IsStatic ? OpCodes.Callvirt : OpCodes.Call;
                    i.Operand = set;
                }
            }
        }

        return property.Target;
    }
}