#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using DependencyProperty = Windows.UI.Xaml.DependencyProperty;
#else
    using System.Windows;
    using DependencyProperty = System.Windows.DependencyProperty;
#endif

    /// <summary>
    /// Static navigator extensions
    /// </summary>
    public static class Navigator
    {
        private static readonly DependencyProperty KeepHiddenProperty = DependencyProperty.RegisterAttached(
            "KeepHidden", typeof (bool), typeof (Navigator), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Sets the keep hidden property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetKeepHidden(DependencyObject element, bool value)
        {
            element.SetValue(KeepHiddenProperty, value);
        }

        /// <summary>
        /// Gets the keep hidden property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetKeepHidden(DependencyObject element)
        {
            return (bool) element.GetValue(KeepHiddenProperty);
        }
    }
}
