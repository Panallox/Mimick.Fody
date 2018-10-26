using Mimick.Aspect;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public TypeReference MethodInterceptionArgs { get; set; }
        public TypeReference ObjectArray { get; set; }
        public TypeReference ParameterInfo { get; set; }
        public TypeReference ParameterInterceptionArgs { get; set; }

        // constructors
        public MethodReference MethodInterceptionArgsCtor { get; set; }
        public MethodReference ParameterInterceptionArgsCtor { get; set; }

        // methods
        public MethodReference MethodBaseGetMethodFromHandle { get; set; }
        public MethodReference MethodBaseGetParameters { get; set; }
        public MethodReference MethodInterceptorOnEnter { get; set; }
        public MethodReference MethodInterceptorOnException { get; set; }
        public MethodReference MethodInterceptorOnExit { get; set; }
        public MethodReference ParameterInterceptorOnEnter { get; set; }

        // properties
        public MethodReference MethodInterceptionArgsCancelGet { get; set; }
        public MethodReference MethodInterceptionArgsReturnGet { get; set; }
        public MethodReference MethodInterceptionArgsReturnSet { get; set; }
        public MethodReference ParameterInterceptionArgsValueGet { get; set; }

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
            MethodInterceptionArgs = module.Type<MethodInterceptionArgs>();
            ObjectArray = module.Type<object[]>();
            ParameterInfo = module.Type<ParameterInfo>();
            ParameterInterceptionArgs = module.Type<ParameterInterceptionArgs>();

            // constructors
            MethodInterceptionArgsCtor = module.Constructor<MethodInterceptionArgs>();
            ParameterInterceptionArgsCtor = module.Constructor<ParameterInterceptionArgs>();

            // methods
            MethodBaseGetMethodFromHandle = module.Method<MethodBase>("GetMethodFromHandle", typeof(RuntimeMethodHandle));
            MethodBaseGetParameters = module.Method<MethodBase>("GetParameters");
            MethodInterceptorOnEnter = module.Method<IMethodInterceptor>("OnEnter");
            MethodInterceptorOnException = module.Method<IMethodInterceptor>("OnException");
            MethodInterceptorOnExit = module.Method<IMethodInterceptor>("OnExit");
            ParameterInterceptorOnEnter = module.Method<IParameterInterceptor>("OnEnter");

            // properties
            MethodInterceptionArgsCancelGet = module.Getter<MethodInterceptionArgs>("Cancel");
            MethodInterceptionArgsReturnGet = module.Getter<MethodInterceptionArgs>("Return");
            MethodInterceptionArgsReturnSet = module.Setter<MethodInterceptionArgs>("Return");
            ParameterInterceptionArgsValueGet = module.Getter<ParameterInterceptionArgs>("Value");
        }
    }
}
