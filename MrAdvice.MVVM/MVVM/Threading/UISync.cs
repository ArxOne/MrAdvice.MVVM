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
    using System.Threading.Tasks;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Core;
#else
    using System.Windows;
#endif
    using Advice;
    using Utility;

    /// <summary>
    /// Allows to invoke a method asynchronously (here, in a background thread)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UISync : Attribute, IMethodAsyncAdvice, IMethodInfoAdvice
    {
        /// <summary>
        /// Invoked once per method, when assembly is loaded
        /// </summary>
        /// <param name="context">The method info advice context</param>
        /// <exception cref="InvalidOperationException">Impossible to run asynchronously a non-void method (you MoFo!)</exception>
        public void Advise(MethodInfoAdviceContext context)
        {
            var targetMethod = context.TargetMethod as MethodInfo;
            if (targetMethod != null && targetMethod.ReturnType != typeof(void) && !typeof(Task).IsAssignableFrom(targetMethod.ReturnType))
                throw new InvalidOperationException("Impossible to run asynchronously a non-void or task method");
        }

        /// <summary>
        /// Implements advice logic.
        /// Usually, advice must invoke context.Proceed()
        /// </summary>
        /// <param name="context">The method advice context.</param>
        public Task Advise(MethodAsyncAdviceContext context)
        {
            return Invoke(context.ProceedAsync);
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public static async Task Invoke(Func<Task> action)
        {
#if WINDOWS_UWP
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async delegate
            {
                await action();
            });
#elif SILVERLIGHT
            Application.Current?.GetDispatcher()?.Invoke(async delegate { await action(); });
#else
            // when application exits, dispatcher may be null, but we nicely ignore (I had no other idea at the moment)
            await Application.Current?.GetDispatcher()?.Invoke(async delegate { await action(); });
#endif
        }
    }
}
