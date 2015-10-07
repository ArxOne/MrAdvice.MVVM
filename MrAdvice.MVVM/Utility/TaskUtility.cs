#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace MrAdvice.MVVM.Utility
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Task utility
    /// </summary>
    public static class TaskUtility
    {
        /// <summary>
        /// Awaits the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public static Task<TResult> Await<TResult>(this Func<TResult> func)
        {
            var tcs = new TaskCompletionSource<TResult>();
            var worker = new BackgroundWorker();
            worker.DoWork += delegate { tcs.SetResult(func()); };
            worker.RunWorkerAsync();
            return tcs.Task;
        }
    }
}
