using Mimick.Aspect;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing references to required types within the module.
    /// </summary>
    public class WeaveReferences
    {
        private readonly ModuleDefinition module;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaveReferences" /> class.
        /// </summary>
        /// <param name="moduleDefinition">The module definition.</param>
        public WeaveReferences(ModuleDefinition moduleDefinition)
        {
            module = moduleDefinition;
            Initialize();
        }

        #region Properties

        // interfaces
        public TypeReference IMethodInterceptor { get; set; }
        public TypeReference IParameterInterceptor { get; set; }

        // classes
        public TypeReference CompilationOptionsAttribute { get; set; }
        public TypeReference Exception { get; set; }
        public TypeReference MethodBase { get; set; }
        public TypeReference MethodInfo { get; set; }
        public TypeReference MethodInterceptionArgs { get; set; }
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
        public MethodReference NonSerializedAttributeCtor { get; set; }
        public MethodReference ParameterInterceptionArgsCtor { get; set; }
        public MethodReference PropertyInterceptionArgsCtor { get; set; }
        
        // methods
        public MethodReference DebugWriteLine { get; set; }
        public MethodReference ConsoleWriteLine { get; set; }
        public MethodReference MethodBaseGetMethodFromHandle { get; set; }
        public MethodReference MethodBaseGetMethodFromHandleAndType { get; set; }
        public MethodReference MethodBaseGetParameters { get; set; }
        public MethodReference MethodInterceptorOnEnter { get; set; }
        public MethodReference MethodInterceptorOnException { get; set; }
        public MethodReference MethodInterceptorOnExit { get; set; }
        public MethodReference ParameterInterceptorOnEnter { get; set; }
        public MethodReference PropertyGetInterceptorOnExit { get; set; }
        public MethodReference PropertyGetInterceptorOnGet { get; set; }
        public MethodReference PropertySetInterceptorOnExit { get; set; }
        public MethodReference PropertySetInterceptorOnSet { get; set; }
        public MethodReference TypeGetProperty { get; set; }
        public MethodReference TypeGetTypeFromHandle { get; set; }
        public MethodReference TypeMakeGenericType { get; set; }

        // properties
        public MethodReference InstanceAwareInstanceSet { get; set; }
        public MethodReference MethodInterceptionArgsCancelGet { get; set; }
        public MethodReference MethodInterceptionArgsReturnGet { get; set; }
        public MethodReference MethodInterceptionArgsReturnSet { get; set; }
        public MethodReference ParameterInterceptionArgsValueGet { get; set; }
        public MethodReference PropertyInterceptionArgsIsDirtyGet { get; set; }
        public MethodReference PropertyInterceptionArgsValueGet { get; set; }
        public MethodReference PropertyInterceptionArgsValueSet { get; set; }
        public MethodReference TypeTypeHandleGet { get; set; }

        #endregion

        /// <summary>
        /// Initialize the references by loading the reference types.
        /// </summary>
        private void Initialize()
        {
            // interfaces
            IMethodInterceptor = module.Type<IMethodInterceptor>();
            IParameterInterceptor = module.Type<IParameterInterceptor>();

            // classes
            CompilationOptionsAttribute = module.Type<CompilationOptionsAttribute>();
            Exception = module.Type<Exception>();
            MethodBase = module.Type<MethodBase>();
            MethodInfo = module.Type<MethodInfo>();
            MethodInterceptionArgs = module.Type<MethodInterceptionArgs>();
            ObjectArray = module.Type<object[]>();
            ParameterInfo = module.Type<ParameterInfo>();
            ParameterInterceptionArgs = module.Type<ParameterInterceptionArgs>();
            PropertyInfo = module.Type<PropertyInfo>();
            PropertyInterceptionArgs = module.Type<PropertyInterceptionArgs>();
            Type = module.Type<Type>();
            TypeArray = module.Type<Type[]>();

            // constructors
            CompilerGeneratedAttributeCtor = module.Constructor<CompilerGeneratedAttribute>();
            MethodInterceptionArgsCtor = module.Constructor<MethodInterceptionArgs>();
            NonSerializedAttributeCtor = module.Constructor<NonSerializedAttribute>();
            ParameterInterceptionArgsCtor = module.Constructor<ParameterInterceptionArgs>();
            PropertyInterceptionArgsCtor = module.Constructor<PropertyInterceptionArgs>();

            // methods
            DebugWriteLine = module.Method(typeof(Debug), "WriteLine", typeof(string));
            ConsoleWriteLine = module.Method(typeof(Console), "WriteLine", typeof(object));
            MethodBaseGetMethodFromHandle = module.Method<MethodBase>("GetMethodFromHandle", typeof(RuntimeMethodHandle));
            MethodBaseGetMethodFromHandleAndType = module.Method<MethodBase>("GetMethodFromHandle", typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle));
            MethodBaseGetParameters = module.Method<MethodBase>("GetParameters");
            MethodInterceptorOnEnter = module.Method<IMethodInterceptor>("OnEnter");
            MethodInterceptorOnException = module.Method<IMethodInterceptor>("OnException");
            MethodInterceptorOnExit = module.Method<IMethodInterceptor>("OnExit");
            ParameterInterceptorOnEnter = module.Method<IParameterInterceptor>("OnEnter");
            PropertyGetInterceptorOnExit = module.Method<IPropertyGetInterceptor>("OnExit");
            PropertyGetInterceptorOnGet = module.Method<IPropertyGetInterceptor>("OnGet");
            PropertySetInterceptorOnExit = module.Method<IPropertySetInterceptor>("OnExit");
            PropertySetInterceptorOnSet = module.Method<IPropertySetInterceptor>("OnSet");
            TypeGetProperty = module.Method<Type>("GetProperty", typeof(string), typeof(BindingFlags));
            TypeGetTypeFromHandle = module.Method<Type>("GetTypeFromHandle", typeof(RuntimeTypeHandle));
            TypeMakeGenericType = module.Method<Type>("MakeGenericType");

            // properties
            InstanceAwareInstanceSet = module.Setter<IInstanceAware>("Instance");
            MethodInterceptionArgsCancelGet = module.Getter<MethodInterceptionArgs>("Cancel");
            MethodInterceptionArgsReturnGet = module.Getter<MethodInterceptionArgs>("Return");
            MethodInterceptionArgsReturnSet = module.Setter<MethodInterceptionArgs>("Return");
            ParameterInterceptionArgsValueGet = module.Getter<ParameterInterceptionArgs>("Value");
            PropertyInterceptionArgsIsDirtyGet = module.Getter<PropertyInterceptionArgs>("IsDirty");
            PropertyInterceptionArgsValueGet = module.Getter<PropertyInterceptionArgs>("Value");
            PropertyInterceptionArgsValueSet = module.Setter<PropertyInterceptionArgs>("Value");
            TypeTypeHandleGet = module.Getter<Type>("TypeHandle");
        }
    }
}
