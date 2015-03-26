#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System.Windows;
    using DependencyProperty = System.Windows.DependencyProperty;

    public static class Navigator
    {
        public static readonly DependencyProperty KeepHiddenProperty = DependencyProperty.RegisterAttached(
            "KeepHidden", typeof (bool), typeof (Navigator), new PropertyMetadata(default(bool)));

        public static void SetKeepHidden(DependencyObject element, bool value)
        {
            element.SetValue(KeepHiddenProperty, value);
        }

        public static bool GetKeepHidden(DependencyObject element)
        {
            return (bool) element.GetValue(KeepHiddenProperty);
        }
    }
}
