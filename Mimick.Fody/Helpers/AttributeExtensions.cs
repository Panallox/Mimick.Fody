using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A class containing extension methods for the <see cref="CustomAttribute"/> class.
/// </summary>
static class AttributeExtensions
{
    private const int InlineNone = 0;
    private const int InlineInline = 1;
    private const int InlineTruncate = 2;

    private static readonly string AttributeUsageFullName = typeof(AttributeUsageAttribute).FullName;

    public static IEnumerable<CustomAttribute> GetCustomAttributes(this ICustomAttributeProvider member)
    {
        if (member.HasCustomAttributes)
        {
            foreach (var attribute in member.CustomAttributes.Where(a => !a.AttributeType.IsSystem()))
            {
                yield return attribute;

                foreach (var child in attribute.AttributeType.Resolve().GetCustomAttributes())
                    yield return child;
            }
        }

        yield break;
    }

    public static CustomAttribute GetAttribute(this CustomAttribute a, TypeReference type)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        var current = a.AttributeType;

        while (current != null)
        {
            var def = current.Resolve();
            var attribute = def.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == type.FullName);

            if (attribute != null)
                return attribute;

            current = def.BaseType;
        }

        return null;
    }

    public static T GetProperty<T>(this CustomAttribute a, string name, T notFound = default(T))
    {
        var value = a?.Properties.Where(x => x.Name == name).Select(x => x.Argument.Value).FirstOrDefault();
        return value == null ? notFound : (T)value;
    }

    public static bool HasInterface(this CustomAttribute a, TypeReference type)
    {
        var current = a.AttributeType;

        while (current != null)
        {
            var def = current.Resolve();

            if (def.Interfaces.Any(b => b.InterfaceType.FullName == type.FullName))
                return true;

            current = def.BaseType;
        }

        return false;
    }

    public static bool HasAttribute(this MethodDefinition m, TypeReference type) => m.CustomAttributes.Any(a => a.HasInterface(type));

    public static bool HasAttribute(this ParameterDefinition p, TypeReference type) => p.CustomAttributes.Any(a => a.HasInterface(type));

    public static bool HasRequiredMethod(this CustomAttribute a, MethodReference method)
    {
        var def = method as MethodDefinition ?? method.Resolve();
        return HasRequiredMethod(a, method.Name, returns: def.ReturnType, parameters: def.Parameters.Select(p => p.ParameterType).ToArray(), generics: def.GenericParameters.ToArray());
    }

    public static bool HasRequiredMethod(this CustomAttribute a, string method, TypeReference returns = null, TypeReference[] parameters = null, GenericParameter[] generics = null)
    {
        var options = a.GetAttribute(ModuleWeaver.GlobalContext.Finder.CompilationOptionsAttribute);
        var inlining = options != null ? options.GetProperty("Inlining", notFound: InlineTruncate) : InlineTruncate;
        
        if ((inlining & InlineTruncate) != InlineTruncate)
            return true;

        var match = a.AttributeType.GetMethod(method, returns: returns, parameters: parameters, generics: generics);

        if (match == null)
            return false;
        
        return match.HasBody();
    }
}
