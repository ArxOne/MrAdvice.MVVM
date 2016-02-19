#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System.Windows;
    /// <summary>
    /// External binding (this is the only way I found)
    /// </summary>
    public class PropertyReaderBinding: DependencyObject
    {
        /// <summary>
        /// The source property
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
            "Source", typeof (object), typeof (PropertyReaderBinding), new PropertyMetadata(default(object)));

        /// <summary>
        /// Sets the source.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetSource(DependencyObject element, object value)
        {
            element.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static object GetSource(DependencyObject element)
        {
            return (object) element.GetValue(SourceProperty);
        }
    }
}