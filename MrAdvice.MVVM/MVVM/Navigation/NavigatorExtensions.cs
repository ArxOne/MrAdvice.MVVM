#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Windows;

    public static class NavigatorExtensions
    {
        public static void Configure<TViewModel, TView>(this INavigator navigator)
            where TView : FrameworkElement
        {
            navigator.Associate(typeof(TViewModel), typeof(TView));
        }

        public static TViewModel Show<TViewModel>(this INavigator navigator, Action<TViewModel> initializer = null)
        {
            var objectInitializer = initializer != null ? delegate(object o) { initializer((TViewModel)o); } : (Action<object>)null;
            return (TViewModel)navigator.Show(typeof(TViewModel), objectInitializer);
        }
    }
}
