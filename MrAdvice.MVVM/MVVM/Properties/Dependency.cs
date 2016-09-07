#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif
    using Advice;
    using Annotation;
    using Utility;

    /// <summary>
    /// Marks a simple auto property to be bound to a DependencyProperty
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Priority(AspectPriority.DataHolder)]
    public partial class Dependency : Attribute, IPropertyAdvice, IPropertyInfoAdvice
    {
        /// <summary>
        /// Gets or sets the notification type.
        /// </summary>
        /// <value>
        /// The notification.
        /// </value>
        public DependencyPropertyNotification Notification { get; set; }

        /// <summary>
        /// Gets or sets the default value for the dependency property.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public object DefaultValue { get; set; } = System.Windows.DependencyProperty.UnsetValue;

        /// <summary>
        /// Invoked once per property, when assembly is loaded
        /// </summary>
        /// <param name="context">The property info advice context</param>
        public void Advise(PropertyInfoAdviceContext context)
        {
            var propertyInfo = context.TargetProperty;
            propertyInfo.CreateDependencyProperty(DefaultValue, Notification);
        }

        /// <summary>
        /// Implements advice logic.
        /// Usually, advice must invoke context.Proceed()
        /// </summary>
        /// <param name="context">The method advice context.</param>
        public void Advise(PropertyAdviceContext context)
        {
            var dependencyProperty = context.TargetProperty.GetDependencyProperty();
            var dependencyObject = (DependencyObject)context.Target;
            if (context.IsGetter)
            {
                // yes, in the end, it is a GetValue()
                context.ReturnValue = dependencyObject.GetValue(dependencyProperty);
            }
            else
            {
                var oldValue = dependencyObject.GetValue(dependencyProperty);
                var newValue = context.Value;
                // not sure it is necessary to check for a change
                if (!oldValue.SafeEquals(newValue))
                    dependencyObject.SetValue(dependencyProperty, newValue);
            }
        }
    }
}
