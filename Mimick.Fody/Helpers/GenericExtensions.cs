using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

/// <summary>
/// A class containing extension methods for common generic operations.
/// </summary>
static class GenericExtensions
{
    public static FieldReference GetGeneric(this FieldDefinition field)
    {
        if (field.DeclaringType.HasGenericParameters)
        {
            var generic = field.DeclaringType.GetGeneric();
            return new FieldReference(field.Name, field.FieldType, generic);
        }

        return field;
    }

    public static MethodReference GetGeneric(this MethodDefinition method)
    {
        if (method.HasGenericParameters)
        {
            var instance = new GenericInstanceMethod(method);
            
            foreach (var param in method.GenericParameters)
                instance.GenericArguments.Add(param);

            return instance;
        }
        else if (method.DeclaringType.HasGenericParameters)
        {
            var generic = method.DeclaringType.GetGeneric();
            var self = new MethodReference(method.Name, method.ReturnType, generic);

            self.CallingConvention = method.CallingConvention;
            self.ExplicitThis = method.ExplicitThis;
            self.HasThis = method.HasThis;

            foreach (var param in method.Parameters)
                self.Parameters.Add(param);

            foreach (var gen in method.GenericParameters)
                self.GenericParameters.Add(gen);
            
            return self;
        }

        return method;
    }

    public static TypeReference GetGeneric(this TypeDefinition type)
    {
        if (type.HasGenericParameters)
        {
            var generic = new GenericInstanceType(type);

            foreach (var parameter in type.GenericParameters)
                generic.GenericArguments.Add(parameter);

            return generic;
        }

        return type;
    }

    public static bool IsGenericMatch(this FieldReference field, string path)
    {
        var parts = new[] { field.FullName, path };

        for (int i = 0; i < 2; i++)
        {
            var start = parts[i].IndexOf('<');

            while (start != -1)
            {
                var end = parts[i].IndexOf('>');
                
                if (end != -1)
                    parts[i] = parts[i].Remove(start, end - start + 1);
                else
                    break;

                start = parts[i].IndexOf('<');
            }
        }

        return parts[0].Equals(parts[1]);
    }

}