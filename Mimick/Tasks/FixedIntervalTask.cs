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
    class FixedIntervalTask : ITimedTask
    {
        private readonly IntervalExecutionHandler callback;
        private readonly object instance;
        private readonly Timer timer;
        private readonly object sync;

        private volatile bool executing;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIntervalTask"/> class.
        /// </summary>
        /// <param name="interval">The interval between executions.</param>
        /// <param name="handler">The handler executed when the interval has elapsed.</param>
        /// <param name="target">The optional target object instance the task executes against.</param>
        public FixedIntervalTask(TimeSpan interval, IntervalExecutionHandler handler, object target = null)
        {
            callback = handler;
            instance = target;
            timer = new Timer(interval.TotalMilliseconds);
            timer.Elapsed += OnTimerElapsed;
            sync = new object();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FixedIntervalTask"/> class.
        /// </summary>
        ~FixedIntervalTask() => Dispose(false);

        #region Properties

        /// <summary>
        /// Gets or sets optional data associated with the timed task.
        /// </summary>
        public object Data
        {
            get; set;
        }

        /// <summary>
        /// Gets whether the timed task is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { lock (sync) return timer.Enabled; }
        }

        /// <summary>
        /// Gets whether the timed task is currently executing.
        /// </summary>
        public bool IsExecuting
        {
            get { lock (sync) return executing; }
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
            {
                lock (sync)
                {
                    timer.Enabled = false;
                    timer.Dispose();
                }
            }
        }

        /// <summary>
        /// Called when the timed interval timer has elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (sync) executing = true;

                callback(instance);
            }
            catch
            {
                // ignore exception to allow next interval to fire
            }
            finally
            {
                lock (sync) executing = false;
            }
        }

        /// <summary>
        /// Starts the timed task processing within the application. If the task is already enabled, this method will do nothing.
        /// </summary>
        public void Start()
        {
            lock (sync) timer.Enabled = true;
        }

        /// <summary>
        /// Stops the timed task from processing within the application. This does not terminate any running tasks, but the
        /// <see cref="IsEnabled" /> property can be checked to see whether the task was terminated during execution.
        /// </summary>
        public void Stop()
        {
            lock (sync) timer.Enabled = false;
        }
    }

    /// <summary>
    /// A delegate method representing the handler invoked when a timed interval task has elapsed.
    /// </summary>
    /// <param name="instance">The optional object instance to execute the task against.</param>
    delegate void IntervalExecutionHandler(object instance);
}
