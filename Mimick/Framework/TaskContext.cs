using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing the task context of the framework which maintains timed and asynchronous tasks.
    /// </summary>
    sealed class TaskContext : ITaskContext
    {
        private readonly IList<ITimedTask> timedTasks;
        private readonly TimedThread timedThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskContext" /> class.
        /// </summary>
        public TaskContext()
        {
            timedTasks = new ReadWriteList<ITimedTask>();
            timedThread = new TimedThread();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TaskContext"/> class.
        /// </summary>
        ~TaskContext() => Dispose(false);

        #region Properties

        /// <summary>
        /// Gets a collection of tasks which are configured to execute on intervals.
        /// </summary>
        public IReadOnlyList<ITimedTask> TimedTasks => new ReadOnlyList<ITimedTask>(timedTasks);

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
                timedThread.Dispose();

                foreach (var e in timedTasks)
                    e.Dispose();
            }
        }

        /// <summary>
        /// Creates a timed interval execution handler for the provided method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A <see cref="IntervalExecutionHandler"/> delegate method.</returns>
        private IntervalExecutionHandler CreateTimedIntervalHandler(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
                throw new MemberAccessException($"Cannot register a timed task against a generic method for '{method.DeclaringType.FullName}.{method.Name}'");

            if (method.GetParameters().Any(p => !p.IsOptional))
                throw new MemberAccessException($"Cannot register a timed task against a method with non-optional parameters for '{method.DeclaringType.FullName}.{method.Name}'");

            var instance = Expression.Parameter(typeof(object), "instance");

            if (method.IsStatic)
                return Expression.Lambda<IntervalExecutionHandler>(Expression.Call(null, method), instance).Compile();
            else
                return Expression.Lambda<IntervalExecutionHandler>(Expression.Call(Expression.Convert(instance, method.DeclaringType), method), instance).Compile();
        }

        /// <summary>
        /// Initialize the task context resulting in any timed tasks starting.
        /// </summary>
        public void Initialize()
        {
            var components = FrameworkContext.Current.ComponentContext;

            foreach (var component in components.GetAllComponents())
            {
                var timed = ReflectionHelper.GetMethodsWithAttribute<ScheduledAttribute>(component.Type, binding: ReflectionHelper.All);
                                                
                if (timed.Any())
                {
                    foreach (var method in timed)
                    {
                        var instance = method.IsStatic ? null : component.Designer.GetComponent();
                        var decoration = ReflectionHelper.GetAttributeInherited<ScheduledAttribute>(method);

                        /*if (decoration.CronInterval != null)
                        {
                            var interval = new CronInterval(decoration.CronInterval);
                            Register(interval, method, instance);
                        }
                        else*/
                        {
                            var interval = new FixedInterval(decoration.FixedInterval.TotalMilliseconds);
                            Register(interval, method, instance);
                        }
                    }
                }
            }

            foreach (var task in timedTasks)
                task.Start();

            timedThread.Start();
        }

        /// <summary>
        /// Registers a method invocation as a timed task within the context.
        /// </summary>
        /// <param name="interval">The interval between method executions.</param>
        /// <param name="method">The method which must execute.</param>
        /// <param name="instance">The object instance which the method must execute against.</param>
        public void Register(ITimedInterval interval, MethodInfo method, object instance)
        {
            if (interval == null)
                throw new ArgumentException("Cannot register a timed task with no interval", "interval");

            if (instance == null && !method.IsStatic)
                throw new ArgumentException($"Cannot register a non-static method without a target instance for '{method.DeclaringType.FullName}.{method.Name}'");

            var handler = CreateTimedIntervalHandler(method);
            var task = new TimedIntervalTask(interval, handler, instance);

            timedTasks.Add(task);
            timedThread.Add(task);
        }
    }
}
