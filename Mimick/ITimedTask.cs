using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a task which executes on a timed or configured interval.
    /// </summary>
    public interface ITimedTask : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets optional data associated with the timed task.
        /// </summary>
        object Data
        {
            get; set;
        }

        /// <summary>
        /// Gets the timed interval which describes when the task should execute.
        /// </summary>
        ITimedInterval Interval
        {
            get;
        }

        /// <summary>
        /// Gets whether the timed task is enabled.
        /// </summary>
        bool IsEnabled
        {
            get;
        }

        /// <summary>
        /// Gets whether the timed task is currently executing.
        /// </summary>
        bool IsExecuting
        {
            get;
        }

        /// <summary>
        /// Gets the date and time that the task was last executed. If the task has not been executed in the scheduling system, this will return <c>null</c>.
        /// </summary>
        DateTime? LastExecutedAt
        {
            get;
        }

        #endregion
        
        /// <summary>
        /// Starts the timed task processing within the application. If the task is already enabled, this method will do nothing.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timed task from processing within the application. This does not terminate any running tasks, but the
        /// <see cref="IsEnabled"/> property can be checked to see whether the task was terminated during execution.
        /// </summary>
        void Stop();

        /// <summary>
        /// Triggers the timed task immediately, regardless of whether the task is enabled or of the scheduled state of the task. If the task
        /// is already executing, this method will do nothing.
        /// </summary>
        /// <remarks>
        /// If a task is triggered before the interval elapses, the task's last execution time will be updated to 
        /// </remarks>
        void Trigger();
    }
}
