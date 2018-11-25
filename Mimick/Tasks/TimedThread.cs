using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Tasks
{
    /// <summary>
    /// A class representing a thread which executes during the application lifetime to ensure that timed tasks are executed.
    /// </summary>
    class TimedThread : IDisposable
    {
        private readonly Random random;
        private readonly object sync;
        private readonly List<ITimedTask> tasks;

        private CancellationTokenSource cancellation;
        private Thread thread;

        private volatile bool pending;
        private volatile bool running;
        private volatile bool shutdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedThread"/> class.
        /// </summary>
        public TimedThread()
        {
            pending = false;
            random = new Random();
            running = false;
            shutdown = false;
            sync = new object();
            tasks = new List<ITimedTask>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TimedThread"/> class.
        /// </summary>
        ~TimedThread() => Dispose(false);

        #region Properties

        /// <summary>
        /// Gets whether the thread is running.
        /// </summary>
        public bool IsRunning
        {
            get { lock (sync) return running; }
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
        private void Dispose(bool disposing) => Stop();

        /// <summary>
        /// Adds a task to the timed thread for processing.
        /// </summary>
        /// <param name="task">The task.</param>
        public void Add(ITimedTask task)
        {
            lock (sync) tasks.AddIfMissing(task);
            StartIfRequired();
        }

        /// <summary>
        /// Executes the operations of the timed thread.
        /// </summary>
        private void Execute()
        {
            lock (sync) cancellation = new CancellationTokenSource();

            var process = new ITimedTask[0];
            var factory = new TaskFactory(cancellation.Token);

            while (!cancellation.IsCancellationRequested)
            {
                lock (sync)
                {
                    if (shutdown || !running)
                        break;

                    process = tasks.ToArray();
                }

                var now = DateTime.Now;
                var estimated = -1L;

                foreach (var task in process)
                {
                    if (!task.IsEnabled || task.IsExecuting)
                        continue;

                    var interval = task.Interval;

                    if (interval.HasElapsed(task.LastExecutedAt, now))
                        factory.StartNew(() => task.Trigger());
                    else
                        estimated = Math.Min(estimated, interval.GetElapseTime(task.LastExecutedAt, now));
                }

                if (estimated <= 0)
                    estimated = random.Next(10, 500);

                lock (sync) Monitor.Wait(sync, (int)estimated);
            }
        }

        /// <summary>
        /// Starts the execution of the timed thread. If the thread is already running, this method will do nothing.
        /// </summary>
        public void Start()
        {
            lock (sync)
            {
                if (running)
                    return;

                pending = true;

                if (tasks.Count == 0 || !tasks.Any(o => o.IsEnabled))
                    return;

                StartThread();
            }
        }

        /// <summary>
        /// Starts the execution of the timed thread if the thread is not running but is expected to do so.
        /// </summary>
        private void StartIfRequired()
        {
            lock (sync)
            {
                if (running || !pending || tasks.Count == 0 || !tasks.Any(o => o.IsEnabled))
                    return;

                StartThread();
            }
        }

        /// <summary>
        /// Starts the internal thread responsible for running the operational logic of the timed thread.
        /// </summary>
        private void StartThread()
        {
            running = true;
            shutdown = false;

            thread = new Thread(Execute);
            thread.IsBackground = true;
            thread.Name = "Mimick Schedule Thread";
            thread.Priority = ThreadPriority.Normal;
            thread.Start();
        }

        /// <summary>
        /// Stops the execution of the time thread, cancelling any future tasks.
        /// </summary>
        public void Stop()
        {
            lock (sync)
            {
                pending = false;
                running = false;

                if (shutdown)
                    return;

                shutdown = true;
                thread = null;

                if (cancellation != null)
                    cancellation.Cancel();

                Monitor.Pulse(sync);
            }
        }
    }
}
