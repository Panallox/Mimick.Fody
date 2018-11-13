using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A class containing extension methods for the <see cref="PropertyReference"/> class.
/// </summary>
static class PropertyExtensions
{
    public static FieldReference GetBackingField(this PropertyReference property)
    {
        var definition = property.Resolve();
        var optimistic = $"<{property.Name}>k__BackingField";
        return definition.DeclaringType.Fields.FirstOrDefault(f => f.Name == optimistic && f.CustomAttributes.Any(c => c.AttributeType.FullName == typeof(CompilerGeneratedAttribute).FullName));

    }
}
