using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

/// <summary>
/// 
/// </summary>
namespace Mimick.Fody
{
    /// <summary>
    /// A class containing methods for resolving references to members.
    /// </summary>
    public class ReferenceFinder
    {
        private readonly ModuleDefinition core;

        private IList<ModuleDefinition> modules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceFinder"/> class.
        /// </summary>
        /// <param name="module">The module.</param>
        public ReferenceFinder(ModuleDefinition module)
        {
            core = module;
            Initialize();
        }
 
        #region Properties

        // interfaces
        public TypeReference IInjectAfterInitializer { get; set; }
        public TypeReference IInjectBeforeInitializer { get; set; }
        public TypeReference IInstanceAware { get; set; }
        public TypeReference IMemberAware { get; set; }
        public TypeReference IMethodInterceptor { get; set; }
        public TypeReference IMethodReturnInterceptor { get; set; }
        public TypeReference IParameterInterceptor { get; set; }
        public TypeReference IPropertyGetInterceptor { get; set; }
        public TypeReference IPropertySetInterceptor { get; set; }
        public TypeReference IRequireInitialization { get; set; }

        // classes
        public TypeReference CompilationImplementsAttribute { get; set; }
        public TypeReference CompilationOptionsAttribute { get; set; }
        public TypeReference Exception { get; set; }
        public TypeReference MethodBase { get; set; }
        public TypeReference MethodInfo { get; set; }
        public TypeReference MethodInterceptionArgs { get; set; }
        public TypeReference MethodReturnInterceptionArgs { get; set; }
        public TypeReference ObjectArray { get; set; }
        public TypeReference ParameterInfo { get; set; }
        public TypeReference ParameterInterceptionArgs { get; set; }
        public TypeReference PropertyInfo { get; set; }
        public TypeReference PropertyInterceptionArgs { get; set; }
        public TypeReference Type { get; set; }
        public TypeReference TypeArray { get; set; }

        // constructors
        public MethodReference CompilerGeneratedAttributeCtor { get; set; }
        public MethodReference MethodInterceptionArgsCtor { get; set; }
        public MethodReference MethodReturnInterceptionArgsCtor { get; set; }
        public MethodReference NonSerializedAttributeCtor { get; set; }
        public MethodReference ParameterInterceptionArgsCtor { get; set; }
        public MethodReference PropertyInterceptionArgsCtor { get; set; }

        // methods
        public MethodReference MethodBaseGetMethodFromHandle { get; set; }
        public MethodReference MethodBaseGetMethodFromHandleAndType { get; set; }
        public MethodReference MethodBaseGetParameters { get; set; }
        public MethodReference MethodInterceptorOnEnter { get; set; }
        public MethodReference MethodInterceptorOnException { get; set; }
        public MethodReference MethodInterceptorOnExit { get; set; }
        public MethodReference MethodReturnInterceptorOnReturn { get; set; }
        public MethodReference ParameterInterceptorOnEnter { get; set; }
        public MethodReference PropertyGetInterceptorOnExit { get; set; }
        public MethodReference PropertyGetInterceptorOnGet { get; set; }
        public MethodReference PropertySetInterceptorOnExit { get; set; }
        public MethodReference PropertySetInterceptorOnSet { get; set; }
        public MethodReference RequireInitializationInitialize { get; set; }
        public MethodReference TypeGetProperty { get; set; }
        public MethodReference TypeGetTypeFromHandle { get; set; }
        public MethodReference TypeMakeGenericType { get; set; }

        // properties
        public MethodReference InstanceAwareInstanceSet { get; set; }
        public MethodReference MemberAwareMemberSet { get; set; }
        public MethodReference MethodInterceptionArgsCancelGet { get; set; }
        public MethodReference MethodInterceptionArgsReturnGet { get; set; }
        public MethodReference MethodInterceptionArgsReturnSet { get; set; }
        public MethodReference MethodReturnInterceptionArgsValueGet { get; set; }
        public MethodReference MethodReturnInterceptionArgsValueSet { get; set; }
        public MethodReference ParameterInterceptionArgsValueGet { get; set; }
        public MethodReference PropertyInterceptionArgsIsDirtyGet { get; set; }
        public MethodReference PropertyInterceptionArgsValueGet { get; set; }
        public MethodReference PropertyInterceptionArgsValueSet { get; set; }
        public MethodReference TypeTypeHandleGet { get; set; }

        #endregion

        /// <summary>
        /// Initializes the reference finder mechanism by ensuring that referenced assemblies are loaded.
        /// </summary>
        private void Initialize()
        {
            InitializeReferencedModules();
            InitializeReferencedMembers();
        }

        /// <summary>
        /// Initializes the modules which have been imported.
        /// </summary>
        private void InitializeReferencedModules()
        {
            modules = new List<ModuleDefinition>();

            var pending = new Queue<ModuleDefinition>();
            var loaded = new HashSet<string>();

            pending.Enqueue(core);

            while (pending.Count > 0)
            {
                var module = pending.Dequeue();
                modules.Add(module);

                foreach (var referenced in module.AssemblyReferences)
                {
                    if (loaded.Contains(referenced.FullName))
                        continue;

                    try
                    {
                        var import = core.AssemblyResolver.Resolve(referenced);

                        if (import?.MainModule == null)
                            continue;

                        modules.Add(import.MainModule);
                        loaded.Add(referenced.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the members which will have been and will be referenced.
        /// </summary>
        private void InitializeReferencedMembers()
        {
            // interfaces
            IInjectAfterInitializer = ResolveType("IInjectAfterInitializer");
            IInjectBeforeInitializer = ResolveType("IInjectBeforeInitializer");
            IInstanceAware = ResolveType("IInstanceAware");
            IMemberAware = ResolveType("IMemberAware");
            IMethodInterceptor = ResolveType("IMethodInterceptor");
            IMethodReturnInterceptor = ResolveType("IMethodReturnInterceptor");
            IParameterInterceptor = ResolveType("IParameterInterceptor");
            IPropertyGetInterceptor = ResolveType("IPropertyGetInterceptor");
            IPropertySetInterceptor = ResolveType("IPropertySetInterceptor");
            IRequireInitialization = ResolveType("IRequireInitialization");

            // classes
            CompilationImplementsAttribute = ResolveType("CompilationImplementsAttribute");
            CompilationOptionsAttribute = ResolveType("CompilationOptionsAttribute");
            Exception = ResolveType("Exception");
            MethodBase = ResolveType("MethodBase");
            MethodInfo = ResolveType("MethodInfo");
            MethodInterceptionArgs = ResolveType("MethodInterceptionArgs");
            MethodReturnInterceptionArgs = ResolveType("MethodReturnInterceptionArgs");
            ObjectArray = new ArrayType(ResolveType("Object"));
            ParameterInfo = ResolveType("ParameterInfo");
            ParameterInterceptionArgs = ResolveType("ParameterInterceptionArgs");
            PropertyInfo = ResolveType("PropertyInfo");
            PropertyInterceptionArgs = ResolveType("PropertyInterceptionArgs");
            Type = ResolveType("Type");
            TypeArray = new ArrayType(Type);

            // constructors
            CompilerGeneratedAttributeCtor = ResolveType("CompilerGeneratedAttribute").GetConstructor();
            MethodInterceptionArgsCtor = MethodInterceptionArgs.GetConstructor();
            MethodReturnInterceptionArgsCtor = MethodReturnInterceptionArgs.GetConstructor();
            NonSerializedAttributeCtor = ResolveType("NonSerializedAttribute").GetConstructor();
            ParameterInterceptionArgsCtor = ParameterInterceptionArgs.GetConstructor();
            PropertyInterceptionArgsCtor = PropertyInterceptionArgs.GetConstructor();

            // methods
            MethodBaseGetMethodFromHandle = ResolveType("MethodBase").GetMethod("GetMethodFromHandle", parameters: new[] { ResolveType("RuntimeMethodHandle") });
            MethodBaseGetMethodFromHandleAndType = ResolveType("MethodBase").GetMethod("GetMethodFromHandle", parameters: new[] { ResolveType("RuntimeMethodHandle"), ResolveType("RuntimeTypeHandle") });
            MethodBaseGetParameters = ResolveType("MethodBase").GetMethod("GetParameters");
            MethodInterceptorOnEnter = IMethodInterceptor.GetMethod("OnEnter");
            MethodInterceptorOnException = IMethodInterceptor.GetMethod("OnException");
            MethodInterceptorOnExit = IMethodInterceptor.GetMethod("OnExit");
            MethodReturnInterceptorOnReturn = IMethodReturnInterceptor.GetMethod("OnReturn");
            ParameterInterceptorOnEnter = IParameterInterceptor.GetMethod("OnEnter");
            PropertyGetInterceptorOnExit = IPropertyGetInterceptor.GetMethod("OnExit");
            PropertyGetInterceptorOnGet = IPropertyGetInterceptor.GetMethod("OnGet");
            PropertySetInterceptorOnExit = IPropertySetInterceptor.GetMethod("OnExit");
            PropertySetInterceptorOnSet = IPropertySetInterceptor.GetMethod("OnSet");
            RequireInitializationInitialize = IRequireInitialization.GetMethod("Initialize");
            TypeGetProperty = Type.GetMethod("GetProperty", parameters: new[] { ResolveType("String"), ResolveType("BindingFlags") });
            TypeGetTypeFromHandle = Type.GetMethod("GetTypeFromHandle", parameters: new[] { ResolveType("RuntimeTypeHandle") });
            TypeMakeGenericType = Type.GetMethod("MakeGenericType");

            // properties

            InstanceAwareInstanceSet = IInstanceAware.GetProperty("Instance").Resolve().SetMethod.Import();
            MemberAwareMemberSet = IMemberAware.GetProperty("Member").Resolve().SetMethod.Import();
            MethodInterceptionArgsCancelGet = MethodInterceptionArgs.GetProperty("Cancel").Resolve().GetMethod.Import();
            MethodInterceptionArgsReturnGet = MethodInterceptionArgs.GetProperty("Return").Resolve().GetMethod.Import();
            MethodInterceptionArgsReturnSet = MethodInterceptionArgs.GetProperty("Return").Resolve().SetMethod.Import();
            MethodReturnInterceptionArgsValueGet = MethodReturnInterceptionArgs.GetProperty("Value").Resolve().GetMethod.Import();
            MethodReturnInterceptionArgsValueSet = MethodReturnInterceptionArgs.GetProperty("Value").Resolve().SetMethod.Import();
            ParameterInterceptionArgsValueGet = ParameterInterceptionArgs.GetProperty("Value").Resolve().GetMethod.Import();
            PropertyInterceptionArgsIsDirtyGet = PropertyInterceptionArgs.GetProperty("IsDirty").Resolve().GetMethod.Import();
            PropertyInterceptionArgsValueGet = PropertyInterceptionArgs.GetProperty("Value").Resolve().GetMethod.Import();
            PropertyInterceptionArgsValueSet = PropertyInterceptionArgs.GetProperty("Value").Resolve().SetMethod.Import();
            TypeTypeHandleGet = Type.GetProperty("TypeHandle").Resolve().GetMethod.Import();
        }

        /// <summary>
        /// Resolves a reference to a type.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <param name="full">Whether to match the type based on the fully qualified type name.</param>
        /// <param name="throws">Whether to throw an exception if the type cannot be found.</param>
        /// <returns>The type reference.</returns>
        public TypeReference ResolveType(string name, bool full = false, bool throws = true)
        {
            foreach (var module in modules)
            {
                var match = module.Types.Where(a => full ? a.FullName == name : a.Name == name).FirstOrDefault();

                if (match != null)
                    return core.ImportReference(match);
            }

            if (throws)
                throw new MissingMemberException($"Cannot resolve a reference to {name} in any of '" + string.Join(", ", modules.Select(m => m.Name)) + "'");

            return null;
        }
    }
}