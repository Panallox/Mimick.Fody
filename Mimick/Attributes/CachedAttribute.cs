using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method return value should be cached dependending on the parameters.
    /// </summary>
    [CompilationOptions(Scope = AttributeScope.MultiInstanced)]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CachedAttribute : Attribute, IMemberAware, IMethodInterceptor, IRequireInitialization
    {
        /// <summary>
        /// The managed encryption instance used to generate hashes of parameter values.
        /// </summary>
        private static readonly SHA256Managed sha256 = new SHA256Managed();

        private readonly ICache<string, object> cache;

        private bool[] accepts;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAttribute" /> class.
        /// </summary>
        public CachedAttribute() : this(int.MaxValue, int.MaxValue)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAttribute" /> class.
        /// </summary>
        /// <param name="maxCount">The maximum count.</param>
        public CachedAttribute(int maxCount) : this(maxCount, int.MaxValue)
        {

        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAttribute"/> class.
        /// </summary>
        /// <param name="maxCount">The maximum count.</param>
        /// <param name="maxTimeMilliseconds">The maximum time in milliseconds.</param>
        public CachedAttribute(int maxCount, int maxTimeMilliseconds)
        {
            cache = new Cache<string, object>()
            {
                MaximumCount = maxCount,
                MaximumTime = TimeSpan.FromMilliseconds(maxTimeMilliseconds)
            };
        }

        #region Properties

        /// <summary>
        /// Gets or sets the member that the attribute was associated with.
        /// </summary>
        public MemberInfo Member
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Initialize the attribute.
        /// </summary>
        public void Initialize()
        {
            var method = Member as MethodInfo;

            if (method == null || method.IsConstructor)
                throw new ArgumentException($"Cannot initialize a cache against a non-method");

            var parameters = method.GetParameters();

            if (parameters == null)
                accepts = new bool[0];
            else
                accepts = parameters.Select(p => !p.IsOut && !p.ParameterType.IsByRef).ToArray();
        }

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnEnter(MethodInterceptionArgs e)
        {
            var id = GetHash(e.Arguments);

            if (cache.TryGet(id, out var value))
            {
                e.Return = value;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Called when a method has been invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(MethodInterceptionArgs e, Exception ex) { }
        
        /// <summary>
        /// Called when a method has been invoked, and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnExit(MethodInterceptionArgs e)
        {
            if (e.Cancel)
                return;

            var id = GetHash(e.Arguments);
            cache[id] = e.Return;
        }

        /// <summary>
        /// Gets a unique hash for the provided parameter collection.
        /// </summary>
        /// <param name="values">The parameter values.</param>
        /// <returns>The unique hash.</returns>
        private string GetHash(object[] values)
        {
            var merged = Encoding.UTF8.GetBytes(string.Join("_", values.Where((p, i) => accepts[i]).Select(p => p?.ToString()?.Replace('_', '.') ?? "null")));
            var hash = sha256.ComputeHash(merged);
            return Convert.ToBase64String(hash);
        }
    }
}
