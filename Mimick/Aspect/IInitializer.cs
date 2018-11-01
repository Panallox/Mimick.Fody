using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing an attribute which injects an associated method into the constructor of a type.
    /// </summary>
    /// <remarks>
    /// The interface does not contain any methods, and implementing attributes will automatically introduce the behaviour
    /// of copying the associated method invocation into the constructor.
    /// </remarks>
    public interface IInitializer
    {
        #region Properties

        /// <summary>
        /// Gets the scope of when the initializer should be called.
        /// </summary>
        InitializeScope When
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// Indicates when the initializer should be called from the constructor.
    /// </summary>
    public enum InitializeScope : int
    {
        /// <summary>
        /// Called after the constructor body has been invoked.
        /// </summary>
        AfterInit = 0,

        /// <summary>
        /// Called before the constructor body has been invoked.
        /// </summary>
        BeforeInit = 1,
    }
}
