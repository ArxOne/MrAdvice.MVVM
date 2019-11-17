#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Threading
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using Advice;
    using Introduction;

    /// <summary>
    /// Allows to invoke a method asynchronously (here, in a background thread)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Async : Attribute, IMethodAdvice, IMethodInfoAdvice
    {
        /// <summary>
        /// Gets or sets a value indicating whether kill existing async method.
        /// This allows to run a method asynchronously only once at a time (and avoid overloads)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [kill existing]; otherwise, <c>false</c>.
        /// </value>
        public bool KillExisting { get; set; }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>
        /// The delay.
        /// </value>
        public int Delay { get; set; }

        /// <summary>
        /// The threads
        /// </summary>
        [NonSerialized]
        // ReSharper disable once UnassignedField.Global
        public IntroducedField<IDictionary<MethodBase, BackgroundWorker>> Threads;

        private IDictionary<MethodBase, BackgroundWorker> GetThreads(AdviceContext context)
        {
            var threads = Threads[context];
            if (threads == null)
                Threads[context] = threads = new Dictionary<MethodBase, BackgroundWorker>();
            return threads;
        }

        private BackgroundWorker GetThread(MethodAdviceContext context)
        {
            var fibers = GetThreads(context);
            lock (fibers)
            {
                BackgroundWorker thread;
                fibers.TryGetValue(context.TargetMethod, out thread);
                return thread;
            }
        }

        internal void SetThread(MethodAdviceContext context, BackgroundWorker thread)
        {
            var fibers = GetThreads(context);
            lock (fibers)
            {
                if (thread == null)
                    fibers.Remove(context.TargetMethod);
                else
                    fibers[context.TargetMethod] = thread;
            }
        }

        /// <summary>
        /// Invoked once per method, when assembly is loaded
        /// </summary>
        /// <param name="context">The method info advice context</param>
        /// <exception cref="InvalidOperationException">Impossible to run asynchronously a non-void method (you MoFo!)</exception>
        public void Advise(MethodInfoAdviceContext context)
        {
            var methodInfo = context.TargetMethod as MethodInfo;
            if (methodInfo == null)
                return;
            if (methodInfo.ReturnType != typeof(void))
                throw new InvalidOperationException("Impossible to run asynchronously a non-void method (you MoFo!)");
        }

        private class ThreadContext
        {
            public MethodAdviceContext AdviceContext;
            public int Delay;
            public bool KillExisting;
            public Async Advice;
        }

        /// <summary>
        /// Launches the target method in a separate thread
        /// </summary>
        /// <param name="context">The method advice context.</param>
        public void Advise(MethodAdviceContext context)
        {
            var fiber = KillExisting ? GetThread(context) : null;
            if (fiber != null && fiber.IsBusy)
                fiber.CancelAsync();
            fiber = new BackgroundWorker();
            fiber.DoWork += ThreadedAdvice;
            if (KillExisting)
                SetThread(context, fiber);

            fiber.RunWorkerAsync(new ThreadContext { AdviceContext = context, Delay = Delay, KillExisting = KillExisting, Advice = this });
        }

        /// <summary>
        /// Runs the advice in another thread.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private static void ThreadedAdvice(object sender, DoWorkEventArgs e)
        {
            var threadContext = (ThreadContext)e.Argument;
            if (threadContext.Delay > 0)
                Thread.Sleep(threadContext.Delay);
            threadContext.AdviceContext.Proceed();
            if (threadContext.KillExisting)
                threadContext.Advice.SetThread(threadContext.AdviceContext, null);
        }
    }
}
