using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed class CachedAttribute : Attribute, IMethodInterceptor
    {
        /// <summary>
        /// The managed encryption instance used to generate hashes of parameter values.
        /// </summary>
        private static readonly SHA256Managed sha256 = new SHA256Managed();

        private readonly ICache<string, object> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAttribute" /> class.
        /// </summary>
        public CachedAttribute() => cache = new Cache<string, object>();

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
        public void OnException(MethodInterceptionArgs e, Exception ex) => throw ex;
        
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
        /// <param name="parameters">The parameter values.</param>
        /// <returns>The unique hash.</returns>
        private string GetHash(object[] parameters)
        {
            var merged = Encoding.UTF8.GetBytes(string.Join("_", parameters.Select(p => p?.ToString()?.Replace('_', '.') ?? "null")));
            var hash = sha256.ComputeHash(merged);
            return Convert.ToBase64String(hash);
        }
    }
}
