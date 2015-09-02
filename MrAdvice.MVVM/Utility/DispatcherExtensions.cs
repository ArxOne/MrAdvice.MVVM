#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Extensions to <see cref="Dispatcher"/>
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the <see cref="T:System.Windows.Threading.Dispatcher" /> was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The delegate to a method that takes parameters, which is pushed onto the <see cref="T:System.Windows.Threading.Dispatcher" /> event queue.</param>
        public static void Invoke(this Dispatcher dispatcher, Action method)
        {
            if (dispatcher.CheckAccess())
                method();
            else
                dispatcher.BeginInvoke(method);
        }
    }
}
