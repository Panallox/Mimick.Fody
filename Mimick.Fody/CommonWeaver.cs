using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mimick.Fody;
using Mimick.Fody.Weavers;
using Mono.Cecil;
using Mono.Cecil.Cil;

/// <summary>
/// A class containing common weaving methods shared across the member weavers.
/// </summary>
public partial class ModuleWeaver
{
    public const Mono.Cecil.TypeAttributes ContainerTypeAttributes = Mono.Cecil.TypeAttributes.Class | Mono.Cecil.TypeAttributes.AutoClass | Mono.Cecil.TypeAttributes.Sealed | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.BeforeFieldInit | Mono.Cecil.TypeAttributes.Public;

    public const int AttributeScopeAdhoc = 1;
    public const int AttributeScopeInstanced = 2;
    public const int AttributeScopeMultiInstanced = 3;
    public const int AttributeScopeSingleton = 4;
    public const int AttributeScopeMultiSingleton = 5;

    /// <summary>
    /// A unique identifier for attribute variables containing properties or constructor arguments.
    /// </summary>
    private int id = 0;

    /// <summary>
    /// An emitter for the container which will hold all singleton references.
    /// </summary>
    private TypeEmitter singletons;

    public void CopyArgumentArrayToReferences(MethodEmitter method, Variable array)
    {
        var parameters = method.Target.Parameters;
        var count = parameters.Count;

        var il = method.GetIL();

        for (int i = 0; i < count; i++)
        {
            var p = parameters[i];
            var type = p.ParameterType;

            if (!type.IsByReference)
                continue;

            var spec = (TypeSpecification)type;
            var unboxing = true;

            il.Emit(Codes.Arg(p));
            il.Emit(Codes.Load(array));
            il.Emit(Codes.Int(i));
            il.Emit(Codes.LoadArray);
            
            switch (spec.ElementType.MetadataType)
            {
                case MetadataType.Boolean:
                case MetadataType.SByte:
                    il.Emit()
            }
        }
    }

    /// <summary>
    /// Create an array variable containing the arguments of the method invocation.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <returns></returns>
    public Variable CreateArgumentArray(MethodEmitter method)
    {
        var parameters = method.Target.Parameters;
        var count = parameters.Count;

        var array = method.EmitLocal(Context.Finder.ObjectArray);
        var il = method.GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.Int(count));
        il.Emit(Codes.CreateArray(TypeSystem.ObjectReference));
        il.Emit(Codes.Store(array));

        for (int i = 0; i < count; i++)
        {
            var p = parameters[i];
            var type = p.ParameterType;

            il.Emit(Codes.Load(array));
            il.Emit(Codes.Int(i));
            il.Emit(Codes.Arg(p));

            if (type.IsByReference)
            {
                var spec = (TypeSpecification)type;
                var boxing = true;
                
                switch (spec.ElementType.MetadataType)
                {
                    case MetadataType.Boolean:
                    case MetadataType.SByte:
                        il.Emit(OpCodes.Ldind_I1);
                        break;
                    case MetadataType.Int16:
                        il.Emit(OpCodes.Ldind_I2);
                        break;
                    case MetadataType.Int32:
                        il.Emit(OpCodes.Ldind_I4);
                        break;
                    case MetadataType.Int64:
                    case MetadataType.UInt64:
                        il.Emit(OpCodes.Ldind_I8);
                        break;
                    case MetadataType.Byte:
                        il.Emit(OpCodes.Ldind_U1);
                        break;
                    case MetadataType.UInt16:
                        il.Emit(OpCodes.Ldind_U2);
                        break;
                    case MetadataType.UInt32:
                        il.Emit(OpCodes.Ldind_U4);
                        break;
                    case MetadataType.Single:
                        il.Emit(OpCodes.Ldind_R4);
                        break;
                    case MetadataType.Double:
                        il.Emit(OpCodes.Ldind_R8);
                        break;
                    case MetadataType.IntPtr:
                    case MetadataType.UIntPtr:
                        il.Emit(OpCodes.Ldind_I);
                        break;
                    default:
                        if (spec.ElementType.IsValueType)
                            il.Emit(Instruction.Create(OpCodes.Ldobj, spec.ElementType));
                        else
                        {
                            il.Emit(OpCodes.Ldind_Ref);
                            boxing = false;
                        }
                        break;
                }

                if (boxing)
                    il.Emit(Codes.Box(spec.ElementType));
            }
            else
            {
                il.Emit(Codes.Box(p.ParameterType));
            }

            il.Emit(Codes.StoreArray);
        }

        return array;
    }

    /// <summary>
    /// Create an attribute variable for a method definition.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The optional parameter member.</param>
    /// <returns></returns>
    public Variable CreateAttribute(MethodEmitter method, CustomAttribute attribute)
    {
        var options = attribute.GetAttribute(Context.Finder.CompilationOptionsAttribute);
        var scope = options != null ? options.GetProperty("Scope", notFound: AttributeScopeSingleton) : AttributeScopeSingleton;

        if (scope == AttributeScopeInstanced && attribute.HasConstructorArguments)
            scope = AttributeScopeMultiInstanced;
        
        switch (scope)
        {
            case AttributeScopeInstanced:
                if (method.Target.IsStatic)
                    scope = AttributeScopeSingleton;
                break;
            case AttributeScopeMultiInstanced:
                if (method.Target.IsStatic)
                    scope = AttributeScopeMultiSingleton;
                break;
        }
        
        switch (scope)
        {
            case AttributeScopeAdhoc:
                return CreateAttributeAdhoc(method, attribute, method.Target);
            case AttributeScopeInstanced:
                return CreateAttributeInstanced(method.Parent, attribute, method.Target);
            case AttributeScopeMultiInstanced:
                return CreateAttributeMultiInstanced(method.Parent, attribute, method.Target);
            case AttributeScopeSingleton:
                return CreateAttributeSingleton(method.Parent, attribute, method.Target);
            case AttributeScopeMultiSingleton:
                return CreateAttributeMultiSingleton(method.Parent, attribute, method.Target);
            default:
                throw new NotSupportedException($"Cannot create attribute '{attribute.AttributeType.FullName}' with scope {scope}");
        }
    }

    /// <summary>
    /// Create an attribute variable for a type reference.
    /// </summary>
    /// <param name="emitter">The type emitter.</param>
    /// <param name="attribute">The attribute.</param>
    /// <returns></returns>
    public Variable CreateAttribute(TypeEmitter emitter, CustomAttribute attribute)
    {
        var options = attribute.GetAttribute(Context.Finder.CompilationOptionsAttribute);
        var scope = options.GetProperty("Scope", notFound: AttributeScopeSingleton);

        switch (scope)
        {
            case AttributeScopeAdhoc:
            case AttributeScopeInstanced:
                return CreateAttributeInstanced(emitter, attribute, emitter.Target);
            case AttributeScopeMultiInstanced:
                return CreateAttributeInstanced(emitter, attribute, emitter.Target);
            case AttributeScopeSingleton:
                return CreateAttributeSingleton(emitter, attribute, emitter.Target);
            case AttributeScopeMultiSingleton:
                return CreateAttributeMultiSingleton(emitter, attribute, emitter.Target);
            default:
                throw new NotSupportedException($"Cannot create attribute '{attribute.AttributeType.FullName}' with scope '{scope}'");
        }
    }

    /// <summary>
    /// Create an attribute variable for a property definition.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="get">The getter method.</param>
    /// <param name="set">The setter method.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="field">The optional field reference which was routed.</param>
    /// <returns></returns>
    public Variable[] CreateAttribute(PropertyEmitter property, MethodEmitter get, MethodEmitter set, CustomAttribute attribute, MemberReference field)
    {
        var method = get ?? set;
        var sta = method.Target.IsStatic;

        var options = attribute.GetAttribute(Context.Finder.CompilationOptionsAttribute);
        var scope = options.GetProperty("Scope", notFound: AttributeScopeSingleton);

        if (sta)
        {
            if (scope == AttributeScopeInstanced)
                scope = AttributeScopeSingleton;
            else if (scope == AttributeScopeMultiInstanced)
                scope = AttributeScopeMultiSingleton;
        }

        var hasGet = attribute.HasInterface(Context.Finder.IPropertyGetInterceptor);
        var hasSet = attribute.HasInterface(Context.Finder.IPropertySetInterceptor);

        switch (scope)
        {
            case AttributeScopeAdhoc:
                var adhocGet = hasGet && get != null ? CreateAttributeAdhoc(get, attribute, property.Target) : null;
                var adhocSet = hasSet && set != null ? CreateAttributeAdhoc(set, attribute, property.Target) : null;
                return new[] { adhocGet, adhocSet };
            case AttributeScopeInstanced:
                var instanced = CreateAttributeInstanced(property.Parent, attribute, field ?? property.Target);
                return new[] { hasGet ? instanced : null, hasSet ? instanced : null };
            case AttributeScopeMultiInstanced:
                var multi = CreateAttributeMultiInstanced(property.Parent, attribute, field ?? property.Target);
                return new[] { hasGet ? multi : null, hasSet ? multi : null };
            case AttributeScopeSingleton:
                var singleton = CreateAttributeSingleton(property.Parent, attribute, field ?? property.Target);
                return new[] { hasGet ? singleton : null, hasSet ? singleton : null };
            case AttributeScopeMultiSingleton:
                var multiSingleton = CreateAttributeMultiSingleton(property.Parent, attribute, field ?? property.Target);
                return new[] { hasGet ? multiSingleton : null, hasSet ? multiSingleton : null };
            default:
                throw new NotSupportedException($"Cannot create attribute '{attribute.AttributeType.FullName}' with scope '{scope}'");
        }
    }

    /// <summary>
    /// Create an adhoc variable used to retain an instance of an attribute.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    public Variable CreateAttributeAdhoc(MethodEmitter method, CustomAttribute attribute, MemberReference member)
    {
        var type = attribute.AttributeType;
        var variable = method.EmitLocal(type);
        var il = method.GetIL();
                
        foreach (var arg in attribute.ConstructorArguments)
            CreateAttributeParameter(method, attribute, arg);
        
        il.Emit(Codes.Create(attribute.Constructor));
        il.Emit(Codes.Store(variable));

        if (attribute.HasInterface(Context.Finder.IInstanceAware) && !method.Target.IsStatic)
        {
            il.Emit(Codes.Load(variable));
            il.Emit(Codes.This);
            il.Emit(Codes.Invoke(Context.Finder.InstanceAwareInstanceSet));
        }

        foreach (var prop in attribute.Properties)
            CreateAttributeProperty(method, attribute, variable, prop);

        CreateAttributeRequirements(il, attribute, member, variable, false);

        return variable;
    }

    /// <summary>
    /// Creates an instance-level field used to retain an instance of an attribute.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    public Variable CreateAttributeInstanced(TypeEmitter emitter, CustomAttribute attribute, MemberReference member)
    {
        var type = attribute.AttributeType;
        var name = $"<{type.Name}>k__Attribute";

        var existing = emitter.GetField(name, type, toStatic: false);

        if (existing != null)
            return existing;

        var field = emitter.EmitField(name, type);
        
        foreach (var ctor in emitter.GetConstructors())
        {
            var il = ctor.GetIL();

            il.Insert = CodeInsertion.Before;
            il.Position = il.GetConstructorBaseOrThis()?.Next ?? il.GetFirst();

            il.Emit(Codes.Nop);
            il.Emit(Codes.This);
            
            foreach (var arg in attribute.ConstructorArguments)
                CreateAttributeParameter(ctor, attribute, arg);

            il.Emit(Codes.Create(attribute.Constructor));
            il.Emit(Codes.Store(field));

            if (attribute.HasInterface(Context.Finder.IInstanceAware))
            {
                il.Emit(Codes.ThisIf(field));
                il.Emit(Codes.Load(field));
                il.Emit(Codes.This);
                il.Emit(Codes.Invoke(Context.Finder.InstanceAwareInstanceSet));
            }

            foreach (var prop in attribute.Properties)
                CreateAttributeProperty(ctor, attribute, field, prop);

            CreateAttributeRequirements(il, attribute, member, field, false);
        }

        return field;
    }

    /// <summary>
    /// Creates an multi-instance-level field used to retain an instance of an attribute.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    public Variable CreateAttributeMultiInstanced(TypeEmitter emitter, CustomAttribute attribute, MemberReference member)
    {
        var index = Interlocked.Increment(ref id);
        var type = attribute.AttributeType;
        var name = $"<{type.Name}${index}>k__Attribute";

        var existing = emitter.GetField(name, type, toStatic: false);

        if (existing != null)
            return existing;

        var field = emitter.EmitField(name, type);

        foreach (var ctor in emitter.GetConstructors())
        {
            var il = ctor.GetIL();

            il.Insert = CodeInsertion.Before;
            il.Position = il.GetConstructorBaseOrThis()?.Next ?? il.GetFirst();

            il.Emit(Codes.Nop);
            il.Emit(Codes.This);

            foreach (var arg in attribute.ConstructorArguments)
                CreateAttributeParameter(ctor, attribute, arg);

            il.Emit(Codes.Create(attribute.Constructor));
            il.Emit(Codes.Store(field));

            if (attribute.HasInterface(Context.Finder.IInstanceAware))
            {
                il.Emit(Codes.ThisIf(field));
                il.Emit(Codes.Load(field));
                il.Emit(Codes.This);
                il.Emit(Codes.Invoke(Context.Finder.InstanceAwareInstanceSet));
            }

            foreach (var prop in attribute.Properties)
                CreateAttributeProperty(ctor, attribute, field, prop);

            CreateAttributeRequirements(il, attribute, member, field, false);
        }

        return field;
    }

    /// <summary>
    /// Creates a multi-singleton field used to retain an instance of an attribute.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    public Variable CreateAttributeMultiSingleton(TypeEmitter emitter, CustomAttribute attribute, MemberReference member)
    {
        var index = Interlocked.Increment(ref id);
        var type = attribute.AttributeType;
        var name = $"<{type.Name}${index}>k__Attribute";
        var field = singletons.EmitField(name, type, toStatic: true, toPublic: true);

        var emit = singletons.GetStaticConstructor();
        var il = emit.GetIL();

        il.Emit(Codes.Nop);

        foreach (var arg in attribute.ConstructorArguments)
            CreateAttributeParameter(emit, attribute, arg);

        il.Emit(Codes.Create(attribute.Constructor));
        il.Emit(Codes.Store(field));

        foreach (var prop in attribute.Properties)
            CreateAttributeProperty(emit, attribute, field, prop);

        CreateAttributeRequirements(il, attribute, member, field, true);

        return field;
    }

    /// <summary>
    /// Creates a singleton field used to retain an instance of an attribute.
    /// </summary>
    /// <param name="emitter">The type weaver.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    public Variable CreateAttributeSingleton(TypeEmitter emitter, CustomAttribute attribute, MemberReference member)
    {
        var type = attribute.AttributeType;
        var name = $"<{type.Name}>k__Attribute";

        var existing = singletons.GetField(name, type, toStatic: true);

        if (existing != null)
            return existing;

        var field = singletons.EmitField(name, type, toStatic: true, toPublic: true);

        var emit = singletons.GetStaticConstructor();
        var il = emit.GetIL();

        il.Emit(Codes.Nop);

        foreach (var arg in attribute.ConstructorArguments)
            CreateAttributeParameter(emit, attribute, arg);

        il.Emit(Codes.Create(attribute.Constructor));
        il.Emit(Codes.Store(field));

        foreach (var prop in attribute.Properties)
            CreateAttributeProperty(emit, attribute, field, prop);

        CreateAttributeRequirements(il, attribute, member, field, true);

        return field;
    }

    /// <summary>
    /// Creates a reference to a parameter used in a custom attribute.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="arg">The argument.</param>
    public void CreateAttributeParameter(MethodEmitter emitter, CustomAttribute attribute, CustomAttributeArgument arg)
    {
        var il = emitter.GetIL();

        if (arg.Value == null)
        {
            il.Emit(Codes.Null);
            return;
        }
                
        var type = arg.Type;

        if (type.IsArray)
        {
            var elements = (arg.Value as IEnumerable).Cast<CustomAttributeArgument>().ToArray();

            il.Emit(Codes.Int(elements.Length));
            il.Emit(Codes.CreateArray(type.GetElementType()));

            if (elements.Length == 0)
                return;

            il.Emit(Codes.Duplicate);

            for (int i = 0, count = elements.Length; i < count; i++)
            {
                il.Emit(Codes.Int(i));

                if (elements[i].Value == null)
                {
                    il.Emit(Codes.Null);
                    il.Emit(Codes.StoreArray);
                }
                else
                {
                    il.Emit(Codes.Load(elements[i].Value));
                    il.Emit(Codes.StoreArray);
                }

                if (i + 1 < count)
                    il.Emit(Codes.Duplicate);
            }
        }
        else
        {
            il.Emit(Codes.Load(arg.Value));
        }
    }

    /// <summary>
    /// Creates a reference to a property used in a custom attribute.
    /// </summary>
    /// <param name="emitter">The emitter.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="field">The attribute variable.</param>
    /// <param name="property">The property.</param>
    public void CreateAttributeProperty(MethodEmitter emitter, CustomAttribute attribute, Variable field, Mono.Cecil.CustomAttributeNamedArgument property)
    {
        var il = emitter.GetIL();
        var member = field.Type.Resolve().Properties.FirstOrDefault(p => p.Name == property.Name)?.SetMethod;

        if (member == null)
            throw new MissingMemberException($"Cannot resolve property {property.Name} of {attribute.AttributeType.Name} as there is no setter");

        il.Emit(Codes.ThisIf(field));
        il.Emit(Codes.Load(field));
        CreateAttributeParameter(emitter, attribute, property.Argument);
        il.Emit(Codes.Invoke(member.Import()));
    }

    /// <summary>
    /// Creates a property which replaces a provided field, and implements the <c>get</c> and <c>set</c> methods.
    /// </summary>
    /// <param name="emitter">The type emitter.</param>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    public PropertyEmitter CreateFieldProperty(TypeEmitter emitter, FieldDefinition field)
    {
        var variable = new Variable(field);
        var property = emitter.EmitProperty(field.Name, field.FieldType, field.IsStatic);
        var context = emitter.Context;

        field.Name = $"<{field.Name}>k__BackingField";

        context.AddCompilerGenerated(field);
        context.AddNonSerialized(field);

        var getter = property.GetGetter();
        var gil = getter.GetIL();
        
        gil.Emit(Codes.Nop);
        gil.Emit(Codes.ThisIf(variable));
        gil.Emit(Codes.Load(variable));
        gil.Emit(Codes.Return);

        var setter = property.GetSetter();
        var sil = setter.GetIL();

        sil.Emit(Codes.Nop);
        sil.Emit(Codes.ThisIf(variable));
        sil.Emit(Codes.Arg(field.IsStatic ? 0 : 1));
        sil.Emit(Codes.Store(variable));
        sil.Emit(Codes.Return);

        return property;
    }

    /// <summary>
    /// Create a type container which can be used to store static references for the provided type.
    /// </summary>
    /// <param name="parent">The target type.</param>
    /// <returns>The target type container emitter.</returns>
    public TypeEmitter CreateTypeContainer(TypeDefinition parent)
    {
        var cname = $"<{parent.FullName.Replace('.', '_')}>k__definition";
        var container = Context.Finder.ResolveType($"Mimick.Runtime.Types.{cname}", full: true, throws: false)?.Resolve();

        if (container == null)
        {
            container = new TypeDefinition("Mimick.Runtime.Types", cname, ContainerTypeAttributes, TypeSystem.ObjectReference.Import());
            Context.Module.Types.Add(container);
        }

        return new TypeEmitter(Context.Module, container.Import(), Context);
    }

    /// <summary>
    /// Create a singleton field used to store method information for the provided method.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <returns></returns>
    public Variable CreateMethodInfo(MethodEmitter method)
    {
        var parent = method.Parent.Target.IsNotPublic ? method.Parent : CreateTypeContainer(method.Parent.Target);
        var type = method.Parent.Target;

        var id = method.Target.GetHashString();
        var name = $"<{method.Target.Name}${id}>k__MethodInfo";
        var existing = parent.GetField(name, Context.Finder.MethodBase, toStatic: true);

        if (existing != null)
            return existing;

        var field = parent.EmitField(name, Context.Finder.MethodBase, toStatic: true, toPublic: true);

        var il = parent.GetStaticConstructor().GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(type.HasGenericParameters ? method.Target : method.Target.GetGeneric()));
        il.Emit(Codes.LoadToken(type.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Finder.MethodBaseGetMethodFromHandleAndType));
        il.Emit(Codes.Store(field));

        return field;
    }

    /// <summary>
    /// Create a singleton field used to store parameter information for the provided method.
    /// </summary>
    /// <param name="method">The method weaver.</param>
    /// <param name="param">The parameter.</param>
    /// <returns></returns>
    public Variable CreateParameterInfo(MethodEmitter method, ParameterReference param)
    {
        var parent = method.Parent.Target.IsNotPublic ? method.Parent : CreateTypeContainer(method.Parent.Target);
        var type = method.Parent.Target;

        var id = method.Target.GetHashString();
        var name = $"<{method.Target.Name}${id}${param.Index}>k__ParameterInfo";

        var existing = parent.GetField(name, Context.Finder.PropertyInfo, toStatic: true);

        if (existing != null)
            return existing;

        var field = parent.EmitField(name, Context.Finder.PropertyInfo, toStatic: true, toPublic: true);

        var il = parent.GetStaticConstructor().GetIL();
        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(type.HasGenericParameters ? method.Target : method.Target.GetGeneric()));
        il.Emit(Codes.LoadToken(type.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Finder.MethodBaseGetMethodFromHandleAndType));
        il.Emit(Codes.Invoke(Context.Finder.MethodBaseGetParameters));
        il.Emit(Codes.Int(param.Index));
        il.Emit(Codes.LoadArray);
        il.Emit(Codes.Store(field));

        return field;
    }

    /// <summary>
    /// Create a singleton field used to store property information for the provided property.
    /// </summary>
    /// <param name="property">The property weaver.</param>
    /// <returns></returns>
    public Variable CreatePropertyInfo(PropertyEmitter property)
    {
        var parent = property.Parent.Target.IsNotPublic ? property.Parent : CreateTypeContainer(property.Parent.Target);
        var type = property.Parent.Target;
        var sta = (property.Target.GetMethod ?? property.Target.SetMethod).IsStatic;
        var name = $"<{property.Target.Name}>k__PropertyInfo";

        var existing = parent.GetField(name, Context.Finder.PropertyInfo, toStatic: true);

        if (existing != null)
            return existing;

        var field = parent.EmitField(name, Context.Finder.PropertyInfo, toStatic: true, toPublic: true);
        var flags = BindingFlags.NonPublic | BindingFlags.Public | (sta ? BindingFlags.Static : BindingFlags.Instance);

        var il = parent.GetStaticConstructor().GetIL();

        il.Emit(Codes.Nop);
        il.Emit(Codes.LoadToken(type.GetGeneric()));
        il.Emit(Codes.InvokeStatic(Context.Finder.TypeGetTypeFromHandle));
        il.Emit(Codes.String(property.Target.Name));
        il.Emit(Codes.Int((int)flags));
        il.Emit(Codes.Invoke(Context.Finder.TypeGetProperty));
        il.Emit(Codes.Store(field));

        return field;
    }

    /// <summary>
    /// Creates and populates any additional contract requirements of the attribute.
    /// </summary>
    /// <param name="il">The code emitter.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="member">The member the attribute is associated with.</param>
    /// <param name="variable">The variable containing the attribute.</param>
    /// <param name="isStatic">Whether the attribute container is static.</param>
    public void CreateAttributeRequirements(CodeEmitter il, CustomAttribute attribute, MemberReference member, Variable variable, bool isStatic)
    {
        if (attribute.HasInterface(Context.Finder.IMemberAware) && member != null)
        {
            il.Emit(Codes.ThisIf(variable));
            il.Emit(Codes.Load(variable));

            if (member is MethodReference method)
            {
                var field = CreateMethodInfo(new MethodEmitter(il.Parent.Parent, method));
                il.Emit(Codes.ThisIf(field));
                il.Emit(Codes.Load(field));
            }

            il.Emit(Codes.Invoke(Context.Finder.MemberAwareMemberSet));
        }

        if (attribute.HasInterface(Context.Finder.IRequireInitialization))
        {
            il.Emit(Codes.ThisIf(variable));
            il.Emit(Codes.Load(variable));
            il.Emit(Codes.Invoke(Context.Finder.RequireInitializationInitialize));
        }
    }

    /// <summary>
    /// Creates the static container which will hold references to singleton values in order to keep types clean.
    /// </summary>
    public void InitializeStaticContainer()
    {
        var type = Context.Finder.ResolveType("Mimick.Runtime.SingletonContainer", full: true, throws: false)?.Resolve();

        if (type == null)
        {
            type = new TypeDefinition("Mimick.Runtime", "SingletonContainer", ContainerTypeAttributes, TypeSystem.ObjectReference.Import());
            ModuleDefinition.Types.Add(type);
        }

        singletons = new TypeEmitter(Context.Module, Context.Module.ImportReference(type), Context);
    }
}
