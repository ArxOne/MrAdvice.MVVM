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
    public class UISync : Attribute, IMethodAdvice, IMethodInfoAdvice
    {
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
        /// Implements advice logic.
        /// Usually, advice must invoke context.Proceed()
        /// </summary>
        /// <param name="context">The method advice context.</param>
        public void Advise(MethodAdviceContext context)
        {
            Invoke(context.Proceed);
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void Invoke(Action action)
        {
#if WINDOWS_UWP
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                action();
            });
#else
            // when application exits, dispatcher may be null, but we nicely ignore (I had no other idea at the moment)
            Application.Current.GetDispatcher()?.Invoke(action);
#endif
        }
    }
}
