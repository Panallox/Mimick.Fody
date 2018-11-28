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

    public static IEnumerable<MethodDefinition> GetAllMethods(this TypeReference type)
    {
        var current = type.Resolve();

        while (current != null)
        {
            foreach (var method in current.Methods)
                yield return method;

            current = current.BaseType?.Resolve();
        }
    }
    
    public static EventReference GetEvent(this TypeReference type, string name, TypeReference eventType)
        => type.Resolve().Events.FirstOrDefault(e => e.Name == name && e.EventType.FullName == eventType.FullName);

    public static IEnumerable<MemberReference> GetMembers(this TypeReference type)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        return def.Fields.Cast<MemberReference>().Concat(def.Methods).Concat(def.Properties);
    }

    public static List<MemberReference> GetMembersWithInterface(this TypeReference type, TypeReference interfaceType)
        => GetMembers(type).Where(m => m.Resolve().CustomAttributes.Any(c => c.HasInterface(interfaceType))).ToList();

    public static List<MemberReference> GetMembersWithAttribute<T>(this TypeReference type)
        => GetMembers(type).Where(m => m.Resolve().CustomAttributes.Any(c => c.AttributeType.FullName == typeof(T).FullName)).ToList();

    public static MethodReference GetConstructor(this TypeReference type, TypeReference[] parameters = null)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        var query = def.Methods.Where(m => m.Name == ".ctor");

        if (parameters != null)
            query = query.Where(m => m.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(parameters.Select(p => p.FullName)));

        return query.FirstOrDefault()?.Import();
    }

    public static MethodReference GetMethod(this TypeReference type, MethodReference method)
    {
        var def = method as MethodDefinition ?? method.Resolve();
        return GetMethod(type, method.Name, returns: method.ReturnType, parameters: method.Parameters.Select(p => p.ParameterType).ToArray(), generics: method.GenericParameters.ToArray());
    }
    
    public static MethodReference GetMethod(this TypeReference type, string name, TypeReference returns = null, TypeReference[] parameters = null, GenericParameter[] generics = null)
    {
        foreach (var method in type.Resolve().GetAllMethods().Where(m => m.Name == name))
        {
            if (returns != null && !method.ReturnType.FullName.Equals(returns.FullName))
                continue;

            if (parameters != null && !method.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(parameters.Select(p => p.FullName)))
                continue;

            if (generics != null && !method.GenericParameters.Select(p => p.Name).SequenceEqual(generics.Select(p => p.Name)))
                continue;

            return method.Import();
        }

        return null;
    }

    public static IEnumerable<MethodDefinition> GetMethodsInNested(this TypeReference type)
    {
        var def = type as TypeDefinition ?? type.Resolve();
        return def.Methods.Concat(def.NestedTypes.SelectMany(a => GetMethodsInNested(a)));
    }

    public static PropertyReference GetProperty(this TypeReference type, string name, TypeReference returnType = null)
        => type.Resolve().Properties.FirstOrDefault(p => p.Name == name && (returnType == null || p.PropertyType.FullName == returnType.FullName));

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