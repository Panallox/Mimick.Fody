using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a synchronization base used when introducing concurrency.
    /// </summary>
    public interface IRequireSynchronization
    {
        #region Properties

        /// <summary>
        /// Gets or sets the synchronization context used for concurrent read and write locks.
        /// </summary>
        ReaderWriterLockSlim SynchronizationContext
        {
            get; set;
        }

        #endregion
    }
}
