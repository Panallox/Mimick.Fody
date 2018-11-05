using System;
using System.Collections.Generic;
using System.Linq;
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
        => ModuleWeaver.GlobalModule.ImportReference(type) ?? type;

    public static EventReference GetEvent(this TypeReference type, string name, TypeReference eventType)
        => type.Resolve().Events.FirstOrDefault(e => e.Name == name && e.EventType.FullName == eventType.FullName);

    public static PropertyReference GetProperty(this TypeReference type, string name, TypeReference returnType)
        => type.Resolve().Properties.FirstOrDefault(p => p.Name == name && p.PropertyType.FullName == returnType.FullName);
}