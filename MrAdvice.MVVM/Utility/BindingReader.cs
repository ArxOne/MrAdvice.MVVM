#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Allows to read bindings
    /// </summary>
    public class BindingReader : DependencyObject
    {
        /// <summary>
        /// The property property
        /// </summary>
        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
            "Property", typeof(object), typeof(BindingReader), new PropertyMetadata(default(object)));

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public object Property
        {
            get { return (object)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        private static BindingReader Instance { get; } = new BindingReader();

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="byReflectionPath"></param>
        /// <returns></returns>
        public static object GetValue(Binding binding, bool byReflectionPath)
        {
            lock (Instance)
            {
                BindingOperations.SetBinding(Instance, PropertyProperty, binding);
                var value = Instance.Property;
#if !SILVERLIGHT
                BindingOperations.ClearBinding(Instance, PropertyProperty);
#endif
                return value;
            }
        }

        //private static object GetValueByReflectionPath(Binding binding)
    }
}
