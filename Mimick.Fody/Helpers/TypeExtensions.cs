using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

/// <summary>
/// A class containing extension methods for the <see cref="TypeReference"/> class.
/// </summary>
static class TypeExtensions
{
    public static TypeReference ToReference(this Type type)
        => ModuleWeaver.GlobalModule.ImportReference(type);

    public static TypeReference Import(this TypeReference type)
        => ModuleWeaver.GlobalModule.ImportReference(type) ?? throw new InvalidOperationException($"Cannot import type {type.FullName}");
    
    public static EventReference GetEvent(this TypeReference type, string name, TypeReference eventType)
        => type.Resolve().Events.FirstOrDefault(e => e.Name == name && e.EventType.FullName == eventType.FullName);

    public static IEnumerable<MemberReference> GetMembers(this TypeReference type)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        return def.Fields.Cast<MemberReference>().Concat(def.Methods).Concat(def.Properties);
    }

    public static List<MemberReference> GetMembersWithInterface<T>(this TypeReference type)
        => GetMembers(type).Where(m => m.Resolve().CustomAttributes.Any(c => c.HasInterface<T>())).ToList();

    public static List<MemberReference> GetMembersWithAttribute<T>(this TypeReference type)
        => GetMembers(type).Where(m => m.Resolve().CustomAttributes.Any(c => c.AttributeType.FullName == typeof(T).FullName)).ToList();
    
    public static MethodReference GetMethod(this TypeReference type, string name, TypeReference returnType, TypeReference[] parameterTypes, GenericParameter[] genericTypes)
    {
        foreach (var method in type.Resolve().Methods.Where(m => m.Name == name && m.ReturnType.FullName == returnType.FullName))
        {
            if (!method.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(parameterTypes.Select(p => p.FullName)))
                continue;

            if (!method.GenericParameters.Select(p => p.Name).SequenceEqual(genericTypes.Select(p => p.Name)))
                continue;

            return method;
        }

        return null;
    }

    public static IEnumerable<MethodDefinition> GetMethodsInNested(this TypeReference type)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        return def.Methods.Concat(def.NestedTypes.SelectMany(a => GetMethodsInNested(a)));
    }

    public static PropertyReference GetProperty(this TypeReference type, string name, TypeReference returnType)
        => type.Resolve().Properties.FirstOrDefault(p => p.Name == name && p.PropertyType.FullName == returnType.FullName);

    public static bool HasAsyncStateMachine(this TypeReference type)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        return def.NestedTypes.Any(a => a.Interfaces.Any(i => i.InterfaceType.FullName == typeof(IAsyncStateMachine).FullName));
    }

    public static bool IsSystem(this TypeReference type)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        var m = def.Module;

        if (m.Name == "System" || m.Name == "mscorlib" || m.Name == "netstandard" || m.Name == "WindowsBase" || m.Name == "testhost")
            return true;

        if (m.Name.StartsWith("System.") || m.Name.StartsWith("Microsoft.") || m.Name.StartsWith("Windows."))
            return true;

        if (type.Namespace == "System" || type.Namespace.StartsWith("System."))
            return true;

        return false;
    }
}