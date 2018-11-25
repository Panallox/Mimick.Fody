using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Mimick.Tasks
{
    /// <summary>
    /// A timed task class representing a task which executes at an interval between executions.
    /// </summary>
    class TimedIntervalTask : ITimedTask
    {
        private readonly IntervalExecutionHandler callback;
        private readonly object instance;
        private readonly object sync;

        private volatile bool enabled;
        private volatile bool executing;
        private DateTime? lastExecution;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntervalTask"/> class.
        /// </summary>
        /// <param name="interval">The interval between executions.</param>
        /// <param name="handler">The handler executed when the interval has elapsed.</param>
        /// <param name="target">The optional target object instance the task executes against.</param>
        public TimedIntervalTask(ITimedInterval interval, IntervalExecutionHandler handler, object target = null)
        {
            callback = handler;
            instance = target;
            Interval = interval;
            lastExecution = DateTime.Now;
            sync = new object();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TimedIntervalTask"/> class.
        /// </summary>
        ~TimedIntervalTask() => Dispose(false);

        #region Properties

        /// <summary>
        /// Gets or sets optional data associated with the timed task.
        /// </summary>
        public object Data
        {
            get; set;
        }

        /// <summary>
        /// Gets the timed interval which describes when the task should execute.
        /// </summary>
        public ITimedInterval Interval { get; }

        /// <summary>
        /// Gets whether the timed task is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { lock (sync) return enabled; }
        }

        /// <summary>
        /// Gets whether the timed task is currently executing.
        /// </summary>
        public bool IsExecuting
        {
            get { lock (sync) return executing; }
        }

        /// <summary>
        /// Gets the date and time that the task was last executed. If the task has not been executed in the scheduling system, this will return <c>null</c>.
        /// </summary>
        public DateTime? LastExecutedAt
        {
            get { lock (sync) return lastExecution; }
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
                Stop();
        }
        
        /// <summary>
        /// Starts the timed task processing within the application. If the task is already enabled, this method will do nothing.
        /// </summary>
        public void Start()
        {
            lock (sync)
            {
                enabled = true;
                lastExecution = null;
            }
        }

        /// <summary>
        /// Stops the timed task from processing within the application. This does not terminate any running tasks, but the
        /// <see cref="IsEnabled" /> property can be checked to see whether the task was terminated during execution.
        /// </summary>
        public void Stop()
        {
            lock (sync) enabled = false;
        }

        /// <summary>
        /// Triggers the timed task immediately, regardless of whether the task is enabled or of the scheduled state of the task. If the task
        /// is already executing, this method will do nothing.
        /// </summary>
        /// <remarks>
        /// If manually triggered, this will not affect the scheduled state of the task, such that the task will trigger again when the interval
        /// has next elapsed. If the interval elapses while the task is processing, the trigger will do nothing but the scheduling system will record
        /// that the task was executed.
        /// </remarks>
        public void Trigger()
        {
            lock (sync)
            {
                if (executing)
                    return;

                executing = true;
            }

            callback(instance);

            lock (sync)
            {
                executing = false;
                lastExecution = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// A delegate method representing the handler invoked when a timed interval task has elapsed.
    /// </summary>
    /// <param name="instance">The optional object instance to execute the task against.</param>
    delegate void IntervalExecutionHandler(object instance);
}
