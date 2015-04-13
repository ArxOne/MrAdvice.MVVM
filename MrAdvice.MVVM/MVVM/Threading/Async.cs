#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Threading
{
    using System;
    using System.Reflection;
    using System.Threading;
    using Advice;

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

        [NonSerialized]
        private Thread _thread;

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

        /// <summary>
        /// Launches the target method in a separate thread
        /// </summary>
        /// <param name="context">The method advice context.</param>
        public void Advise(MethodAdviceContext context)
        {
            if (KillExisting && _thread != null && _thread.IsAlive)
                _thread.Abort();
            _thread = new Thread(context.Proceed) { IsBackground = true, Name = context.TargetMethod.Name };
            _thread.Start();
        }
    }
}
