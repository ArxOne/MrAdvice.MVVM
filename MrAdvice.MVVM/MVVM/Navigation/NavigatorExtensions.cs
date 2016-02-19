#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Threading.Tasks;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Extensions to INavigator
    /// </summary>
    public static class NavigatorExtensions
    {
        /// <summary>
        /// Associates given view and view-model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="navigator">The navigator.</param>
        public static void Configure<TViewModel, TView>(this INavigator navigator)
            where TView : FrameworkElement
        {
            navigator.Associate(typeof(TViewModel), typeof(TView));
        }

        /// <summary>
        /// Shows the specified view-model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="navigator">The navigator.</param>
        /// <param name="viewModelInitializer"></param>
        /// <returns></returns>
        public static async Task<TViewModel> Show<TViewModel>(this INavigator navigator, Func<TViewModel, Task> viewModelInitializer = null)
        {
            var objectInitializer = viewModelInitializer != null
                ? async delegate (object o) { await viewModelInitializer((TViewModel)o); }
            : (Func<object, Task>)null;
            var result = await navigator.Show(typeof(TViewModel), objectInitializer);
            return (TViewModel)result;
        }

        /// <summary>
        /// Creates the view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="navigator">The navigator.</param>
        /// <param name="viewModelInitializer">The view model initializer.</param>
        /// <returns></returns>
        public static async Task<TViewModel> CreateViewModel<TViewModel>(this INavigator navigator, Func<TViewModel, Task> viewModelInitializer = null)
        {
            var objectInitializer = viewModelInitializer != null
                ? async delegate (object o) { await viewModelInitializer((TViewModel)o); }
            : (Func<object, Task>)null;
            var result = await navigator.CreateViewModel(typeof(TViewModel), objectInitializer);
            return (TViewModel)result;
        }
    }
}
