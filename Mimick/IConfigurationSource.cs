using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a source which loads and resolves configuration values.
    /// </summary>
    public interface IConfigurationSource
    {
        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        /// <exception cref="ConfigurationException">If the configuration source could not be loaded.</exception>
        void Load();

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        /// <exception cref="ConfigurationException">If the configuration source could not be refreshed.</exception>
        void Refresh();

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>The configuration value; otherwise, <c>null</c> if the configuration could not be found.</returns>
        /// <exception cref="ConfigurationException">If the configuration expression causes problems or the resolved value cannot be processed.</exception>
        string Resolve(string name);

        /// <summary>
        /// Attempt to resolve the value of a configuration with the provided name, and return whether it was resolved successfully.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="value">The configuration value.</param>
        /// <returns><c>true</c> if the configuration is resolved; otherwise, <c>false</c>.</returns>
        /// <exception cref="ConfigurationException">If the configuration expression causes problems or the resolved value cannot be processed.</exception>
        bool TryResolve(string name, out string value);
    }

    /// <summary>
    /// An exception class raised when a problem occurs during configuration reading or resolution.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConfigurationException(string message) : base(message) => Expression = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cause">The exception which caused the configuration failure.</param>
        public ConfigurationException(string message, Exception cause) : base(message, cause) => Expression = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="expression">The configuration expression.</param>
        public ConfigurationException(string message, string expression) : base(message) =>  Expression = expression;

        #region Properties

        /// <summary>
        /// Gets or sets the configuration expression which caused the issue.
        /// </summary>
        public string Expression
        {
            get;
        }

        #endregion
    }
}
