using Mono.Cecil;
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
}
