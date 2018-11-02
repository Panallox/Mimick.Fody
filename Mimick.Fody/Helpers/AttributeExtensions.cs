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
    public static CustomAttribute GetAttribute<T>(this CustomAttribute a) where T : Attribute
    {
        var match = typeof(T).FullName;
        return a.AttributeType.Resolve().CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == match);
    }

    public static T GetProperty<T>(this CustomAttribute a, string name, T notFound = default(T))
    {
        var value = a?.Properties.Where(x => x.Name == name).Select(x => x.Argument.Value).FirstOrDefault();
        return value == null ? notFound : (T)value;
    }

    public static bool HasInterface<T>(this CustomAttribute a)
    {
        var match = typeof(T).FullName;
        return a.AttributeType.Resolve().Interfaces.Any(b => b.InterfaceType.FullName == match);
    }

    public static bool HasAttribute<T>(this MethodDefinition m)
    {
        var match = typeof(T).FullName;
        return m.CustomAttributes.Any(a => a.AttributeType.Resolve().Interfaces.Any(b => b.InterfaceType.FullName == match));
    }

    public static bool HasAttribute<T>(this ParameterDefinition p)
    {
        var match = typeof(T).FullName;
        return p.CustomAttributes.Any(a => a.AttributeType.Resolve().Interfaces.Any(b => b.InterfaceType.FullName == match));
    }
}
