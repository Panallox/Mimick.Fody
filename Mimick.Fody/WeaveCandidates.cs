using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing references to types which are candidates for scanning, and methods for refining the list.
    /// </summary>
    public class WeaveCandidates : IEnumerable<TypeReference>
    {
        private readonly ReferenceFinder finder;
        private readonly ModuleDefinition module;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaveCandidates"/> class.
        /// </summary>
        /// <param name="moduleDefinition">The module definition.</param>
        /// <param name="referenceFinder">The reference finder.</param>
        public WeaveCandidates(ModuleDefinition moduleDefinition, ReferenceFinder referenceFinder)
        {
            finder = referenceFinder;
            module = moduleDefinition;
            Types = new List<TypeReference>();
            Initialize();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the collection of candidate types.
        /// </summary>
        public List<TypeReference> Types
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Gets a collection of type candidate results for types containing members which implement constructor interceptors.
        /// </summary>
        /// <returns></returns>
        public List<TypeInterceptorInfo> FindTypeByConstructorInterceptors()
        {
            var list = new List<TypeInterceptorInfo>();

            foreach (var type in Types.Select(a => a.Resolve()).Where(a => !a.IsEnum && !a.IsInterface))
            {
                var ctors = new ConstructorInterceptorInfo { Initializers = new MethodDefinition[0] };
                var item = new TypeInterceptorInfo { Constructors = ctors, Type = type };

                ctors.Initializers = type.Methods.Where(m => !m.IsAbstract).Where(m => m.GetCustomAttributes().Any(a => a.HasInterface(finder.IInjectBeforeInitializer) || a.HasInterface(finder.IInjectAfterInitializer))).ToArray();

                if (ctors.Initializers.Length > 0)
                    list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Gets a collection of type candidate results for types containing members which implement property interceptors.
        /// </summary>
        /// <returns></returns>
        public List<TypeInterceptorInfo> FindTypeByFieldInterceptors()
        {
            var list = new List<TypeInterceptorInfo>();

            foreach (var type in Types.Select(a => a.Resolve()).Where(a => !a.IsEnum && !a.IsInterface))
            {
                var item = new TypeInterceptorInfo { Type = type };
                var fields = new List<FieldInterceptorInfo>();
                
                foreach (var field in type.Fields)
                {
                    var attributes = field.GetCustomAttributes().Where(a => a.HasInterface(finder.IPropertyGetInterceptor) || a.HasInterface(finder.IPropertySetInterceptor));

                    if (attributes.Any())
                        fields.Add(new FieldInterceptorInfo { Field = field, Interceptors = attributes.ToArray() });
                }

                if (fields.Count > 0)
                {
                    item.Fields = fields.ToArray();
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a collection of type candidate results for types requiring implementation.
        /// </summary>
        /// <returns></returns>
        public List<TypeInterceptorInfo> FindTypeByImplements()
        {
            var list = new List<TypeInterceptorInfo>();

            foreach (var type in Types.Select(a => a.Resolve()).Where(a => !a.IsEnum && !a.IsInterface))
            {
                var implements = type.GetCustomAttributes().Where(a => a.GetAttribute(finder.CompilationImplementsAttribute) != null).ToList();
                
                foreach (var member in type.GetMembers())
                {
                    var inner = member.Resolve().GetCustomAttributes().Where(c => c.GetAttribute(finder.CompilationImplementsAttribute) != null);
                    implements.AddRange(inner);
                }

                if (implements.Any())
                    list.Add(new TypeInterceptorInfo { Implements = implements.ToArray(), Type = type });
            }

            return list;
        }
                
        /// <summary>
        /// Gets a collection of type candidate results for types containing members which implement method interceptors.
        /// </summary>
        /// <returns></returns>
        public List<TypeInterceptorInfo> FindTypeByMethodInterceptors()
        {
            var list = new List<TypeInterceptorInfo>();

            foreach (var type in Types.Select(a => a.Resolve()).Where(a => !a.IsEnum && !a.IsInterface))
            {
                var item = new TypeInterceptorInfo { Type = type };
                var methods = new List<MethodInterceptorInfo>();

                var top = type.GetCustomAttributes().Where(a => a.HasInterface(finder.IMethodInterceptor));

                foreach (var method in type.Methods.Where(m => !m.IsAbstract))
                {
                    var these = method.GetCustomAttributes().Where(a => a.HasInterface(finder.IMethodInterceptor)).Concat(top);
                    var parameters = method.Parameters.Where(p => p.GetCustomAttributes().Any(a => a.HasInterface(finder.IParameterInterceptor))).Select(p => new ParameterInterceptorInfo { Index = p.Index, Attributes = p.GetCustomAttributes().Where(a => a.HasInterface(finder.IParameterInterceptor)).ToArray() });
                    var allParameters = new ParameterInterceptorInfo { Index = -1, Attributes = method.GetCustomAttributes().Where(a => a.HasInterface(finder.IParameterInterceptor)).ToArray() };

                    if (these.Any() || parameters.Any() || allParameters.Attributes.Length > 0)
                    {
                        var generic = type.HasGenericParameters ? method.MakeGeneric(type.GenericParameters.ToArray()) : method;

                        if (allParameters.Attributes.Length > 0)
                            parameters = parameters.Concat(new[] { allParameters });

                        methods.Add(new MethodInterceptorInfo
                        {
                            Method = generic.Resolve(),
                            MethodInterceptors = these.ToArray(),
                            Parameters = parameters.ToArray()
                        });
                    }
                }

                if (methods.Count > 0)
                {
                    item.Methods = methods.ToArray();
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a collection of type candidate results for types containing members which implement property interceptors.
        /// </summary>
        /// <returns></returns>
        public List<TypeInterceptorInfo> FindTypeByPropertyInterceptors()
        {
            var list = new List<TypeInterceptorInfo>();

            foreach (var type in Types.Select(a => a.Resolve()).Where(a => !a.IsEnum && !a.IsInterface))
            {
                var item = new TypeInterceptorInfo { Type = type };
                var properties = new List<PropertyInterceptorInfo>();

                var top = type.GetCustomAttributes().Where(a => a.HasInterface(finder.IPropertyGetInterceptor) || a.HasInterface(finder.IPropertySetInterceptor));

                foreach (var property in type.Properties)
                {
                    var attributes = property.GetCustomAttributes().Where(a => a.HasInterface(finder.IPropertyGetInterceptor) || a.HasInterface(finder.IPropertySetInterceptor)).Concat(top);

                    if (attributes.Any())
                        properties.Add(new PropertyInterceptorInfo { Interceptors = attributes.ToArray(), Property = property });
                }

                if (properties.Count > 0)
                {
                    item.Properties = properties.ToArray();
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TypeReference> GetEnumerator() => Types.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => Types.GetEnumerator();

        /// <summary>
        /// Initialize the candidate list by scanning the module immediately.
        /// </summary>
        private void Initialize()
        {
            foreach (var x in module.Types)
                InitializeBy(x);
        }

        /// <summary>
        /// Initialize a candidate by scanning a type.
        /// </summary>
        /// <param name="type">The type.</param>
        private void InitializeBy(TypeDefinition type)
        {
            var m = type.Module;

            foreach (var s in type.NestedTypes)
                InitializeBy(s);

            if (type.IsSystem())
                return;

            if (type.IsInterface || !type.IsClass)
                return;

            Types.Add(type);
        }
    }
}
