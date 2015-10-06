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
#else
    using System.Windows;
#endif
    using Utility;

    /// <summary>
    /// Command attached property holder
    /// </summary>
    public static class Command
    {
        /// <summary>
        /// The target property
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached(
                    "Target", typeof(object), typeof(Command), new PropertyMetadata(default(object), BindTarget));

        /// <summary>
        /// Sets the target.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetTarget(DependencyObject element, object value)
        {
            element.SetValue(TargetProperty, value);
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static object GetTarget(DependencyObject element)
        {
            return (object)element.GetValue(TargetProperty);
        }

        private static void BindTarget(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)d;
            if (element.DataContext == null)
                element.DataContextChanged += delegate { SetCommand(element, e.NewValue); };
            else
                SetCommand(element, e.NewValue);
        }

        private static void SetCommand(FrameworkElement element, object value)
        {
            var command = new RelayCommand(element.DataContext, value);
            element.SetCommand(command, null);
        }
    }
}
