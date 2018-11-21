using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a context which maintains timed and asynchronous tasks.
    /// </summary>
    public interface ITaskContext : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets a collection of tasks which are configured to execute on intervals.
        /// </summary>
        IReadOnlyList<ITimedTask> TimedTasks
        {
            get;
        }

        #endregion
    }
}
