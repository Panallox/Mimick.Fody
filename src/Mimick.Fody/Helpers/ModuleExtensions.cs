using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A class containing extension methods for the <see cref="ModuleDefinition"/> class.
/// </summary>
static class ModuleExtensions
{
    public static MethodReference Constructor<T>(this ModuleDefinition m) => m.ImportReference(typeof(T).GetConstructors().First());

    public static MethodReference Getter<T>(this ModuleDefinition m, string name) => m.ImportReference(typeof(T).GetProperty(name).GetGetMethod());
    public static MethodReference Setter<T>(this ModuleDefinition m, string name) => m.ImportReference(typeof(T).GetProperty(name).GetSetMethod());

    public static MethodReference Method<T>(this ModuleDefinition m, string name) => m.ImportReference(typeof(T).GetMethod(name));
    public static MethodReference Method<T>(this ModuleDefinition m, string name, params Type[] param) => m.ImportReference(typeof(T).GetMethod(name, param));
    public static MethodReference Method(this ModuleDefinition m, Type type, string name) => m.ImportReference(type.GetMethod(name));
    public static MethodReference Method(this ModuleDefinition m, Type type, string name, params Type[] param) => m.ImportReference(type.GetMethod(name, param));

    public static TypeReference Type<T>(this ModuleDefinition m) => m.ImportReference(typeof(T));
    public static TypeReference Type(this ModuleDefinition m, Type type) => m.ImportReference(type);
}