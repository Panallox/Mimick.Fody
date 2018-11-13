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

    public static CustomAttribute GetAttribute<T>(this CustomAttribute a) where T : Attribute
    {
        var match = typeof(T).FullName;
        var current = a.AttributeType;

        while (current != null)
        {
            var def = current.Resolve();
            var attribute = def.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == match);

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

    public static bool HasInterface<T>(this CustomAttribute a)
    {
        var match = typeof(T).FullName;
        var current = a.AttributeType;

        while (current != null)
        {
            var def = current.Resolve();

            if (def.Interfaces.Any(b => b.InterfaceType.FullName == match))
                return true;

            current = def.BaseType;
        }

        return false;
    }

    public static bool HasAttribute<T>(this MethodDefinition m) => m.CustomAttributes.Any(a => a.HasInterface<T>());

    public static bool HasAttribute<T>(this ParameterDefinition p) => p.CustomAttributes.Any(a => a.HasInterface<T>());
}
