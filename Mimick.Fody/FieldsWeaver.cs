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
            var weaver = new TypeWeaver(Context.Module, item.Type, Context);

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
    /// <param name="weaver">The type weaver.</param>
    /// <param name="item">The interceptor information.</param>
    public PropertyDefinition WeaveFieldReplacements(TypeWeaver weaver, FieldInterceptorInfo item)
    {
        var field = item.Field;
        var path = field.FullName;

        var variable = new Variable(field);
        var property = weaver.CreateProperty(field.Name, field.FieldType);

        field.Name = $"<{field.Name}>k__BackingField";

        Context.AddCompilerGenerated(field);
        Context.AddNonSerialized(field);

        var getter = property.GetGetter();
        var gil = getter.GetWeaver();

        gil.Emit(Codes.Nop);

        if (variable.IsThisNeeded)
            gil.Emit(Codes.This);

        gil.Emit(Codes.Load(variable));
        gil.Emit(Codes.Return);

        var setter = property.GetSetter();
        var sil = setter.GetWeaver();

        sil.Emit(Codes.Nop);

        if (variable.IsThisNeeded)
            sil.Emit(Codes.This);

        sil.Emit(Codes.Arg(1));
        sil.Emit(Codes.Store(variable));
        sil.Emit(Codes.Return);
        
        var search = field.IsPrivate ? weaver.Target.Methods : Context.Candidates.SelectMany(a => a.Resolve().Methods);

        foreach (var method in search)
        {
            if (method == getter.Target || method == setter.Target)
                continue;

            var il = method.Body.Instructions.Where(i => i.OpCode == OpCodes.Ldfld || i.OpCode == OpCodes.Ldsfld || i.OpCode == OpCodes.Stfld || i.OpCode == OpCodes.Stsfld);

            foreach (var i in il)
            {
                var reference = i.Operand as FieldReference ?? i.Operand as FieldDefinition;

                if (reference == null || !reference.FullName.Equals(field.FullName))
                    continue;
                
                if (i.OpCode == OpCodes.Ldfld || i.OpCode == OpCodes.Ldsfld)
                {
                    i.OpCode = getter.Target.IsAbstract ? OpCodes.Callvirt : OpCodes.Call;
                    i.Operand = getter.Target;
                }
                else if (i.OpCode == OpCodes.Stfld || i.OpCode == OpCodes.Stsfld)
                {
                    i.OpCode = setter.Target.IsAbstract ? OpCodes.Callvirt : OpCodes.Call;
                    i.Operand = setter.Target;
                }
            }
        }

        return property.Target;
    }
}