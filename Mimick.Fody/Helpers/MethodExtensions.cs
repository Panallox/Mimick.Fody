using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A class containing extension methods for the <see cref="MethodReference"/> class.
/// </summary>
static class MethodExtensions
{
    public static CustomAttribute GetAttribute<T>(this MethodReference method) =>
        (method as MethodDefinition ?? method.Resolve()).CustomAttributes.FirstOrDefault(a => a.HasInterface<T>());

    public static string GetHashString(this MethodReference method)
    {
        var m = method as MethodDefinition ?? method.Resolve();
        var hash = 23L;
        hash = hash * 31L + method.FullName.GetHashCode();

        var generics = method.GenericParameters;
        for (int i = 0, count = generics.Count; i < count; i++)
            hash = hash * 31L + generics[i].FullName.GetHashCode();

        var parameters = method.Parameters;
        for (int i = 0, count = parameters.Count; i < count; i++)
            hash = hash * 31L + parameters[i].ParameterType.FullName.GetHashCode();

        var returns = method.ReturnType;
        hash = hash * 31L + returns.FullName.GetHashCode();

        return string.Format("{0:X}", Math.Abs(hash));
    }

    public static bool HasBody(this MethodReference method)
    {
        if (method == null)
            return false;

        var body = method.Resolve().Body;
        var il = body.GetILProcessor();
        var it = body.Instructions;
        var count = it.Count;
        
        switch (count)
        {
            case 0:
                return false;
            case 1:
                var code = it[0].OpCode;

                if (code == OpCodes.Nop ||
                    code == OpCodes.Ret)
                    return false;
                break;
            case 2:
                var code21 = it[0].OpCode;
                var code22 = it[1].OpCode;

                if ((code21 == OpCodes.Nop && code22 == OpCodes.Ret))
                    return false;
                break;
            case 3:
                var code31 = it[0].OpCode;
                var code32 = it[1].OpCode;
                var code33 = it[2].OpCode;

                if (code31 == OpCodes.Nop && (code32 == OpCodes.Br || code32 == OpCodes.Br_S) && code33.Equals(it[1].Operand))
                    return false;
                if (code31 == OpCodes.Nop && code32 == OpCodes.Newobj && it[1].Operand is TypeReference type && code33 == OpCodes.Throw)
                    return type.FullName == typeof(NotImplementedException).FullName;

                break;
        }


        return true;
    }

    public static MethodReference Import(this MethodReference method)
        => ModuleWeaver.GlobalModule.ImportReference(method);

    public static bool IsReturn(this MethodReference method)
    {
        if (method == null)
            return false;

        if (method.ReturnType.FullName == typeof(void).FullName)
            return false;

        if (method.ReturnType.FullName.StartsWith(typeof(Task).FullName) && !method.ReturnType.IsGenericInstance)
            return false;

        return true;
    }

    public static MethodReference MakeGeneric(this MethodReference method, params TypeReference[] types)
    {
        if (method.GenericParameters.Count == 0)
            return MakeHostInstanceGeneric(method, types.Select(x => method.Module.ImportReference(x)).ToArray());
        else
        {
            var instance = new GenericInstanceMethod(method);

            foreach (var generic in types)
                instance.GenericArguments.Add(generic);

            return instance;
        }
    }

    public static MethodReference MakeHostInstanceGeneric(this MethodReference method, params TypeReference[] types)
    {
        var genericType = method.DeclaringType.MakeGenericInstanceType(types);
        var genericMethod = new MethodReference(method.Name, method.ReturnType, genericType);

        genericMethod.CallingConvention = method.CallingConvention;
        genericMethod.ExplicitThis = method.ExplicitThis;
        genericMethod.HasThis = method.HasThis;

        foreach (var param in method.Parameters)
            genericMethod.Parameters.Add(new ParameterDefinition(param.ParameterType));

        foreach (var generic in method.GenericParameters)
            genericMethod.GenericParameters.Add(new GenericParameter(generic.Name, genericMethod));

        return genericMethod;
    }
}
