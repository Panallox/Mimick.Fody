using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyToProcess.Framework;
using Mimick;

namespace AssemblyToProcess.Attributes
{
    /// <summary>
    /// A class containing methods introducing the <see cref="AutowireAttribute"/> decoration.
    /// </summary>
    public class AutowireAttributes
    {
        [Autowire]
        private SingletonComponent autoPrivateField = null;

        [Autowire]
        public SingletonComponent autoField;

        /// <summary>
        /// Gets an autowired component.
        /// </summary>
        [Autowire]
        public SingletonComponent AutoProperty
        {
            get;
        }

        /// <summary>
        /// Autowires the optional parameter with the component.
        /// </summary>
        /// <param name="autoParameter">The component.</param>
        /// <returns>The component.</returns>
        public SingletonComponent AutoMethod([Autowire] SingletonComponent autoParameter = null) => autoParameter;

        /// <summary>
        /// Autowires all optional parameters with the component.
        /// </summary>
        /// <param name="autoParameter">The component.</param>
        /// <returns>The component.</returns>
        [Autowire]
        public SingletonComponent AutoAnyMethod(SingletonComponent autoParameter = null) => autoParameter;

        /// <summary>
        /// Gets an auto-wired component which has been associated with a private field.
        /// </summary>
        /// <returns>The component.</returns>
        public SingletonComponent GetAutoPrivateField() => autoPrivateField;
    }
}
