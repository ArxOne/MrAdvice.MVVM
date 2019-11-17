#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    using System;
    using Advice;
    using Annotation;
    using SystemDependencyProperty = System.Windows.DependencyProperty;

    /// <summary>
    /// Aspect for attached properties
    /// See Property for more information
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Priority(AspectPriority.DataHolder)]
    public class Attached : Attribute, IPropertyAdvice, IPropertyInfoAdvice
    {
        /// <summary>
        /// Gets or sets the default value for the dependency property.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the notification type.
        /// </summary>
        /// <value>
        /// The notification.
        /// </value>
        public DependencyPropertyNotification Notification { get; set; }

        /// <summary>
        /// Gets or sets the property changed callback.
        /// </summary>
        /// <value>
        /// The property changed callback.
        /// </value>
        public string CallbackName { get; set; }

        /// <summary>
        /// The CurrentProperty is used by Property
        /// The syntax for using all of this is elegant, much more elegant than the implementation
        /// </summary>
        [ThreadStatic]
        internal static SystemDependencyProperty CurrentProperty;

        private static readonly object[] NoParameter = new object[0];

        /// <summary>
        /// Invoked once per property, when assembly is loaded
        /// </summary>
        /// <param name="context">The property info advice context</param>
        public void Advise(PropertyInfoAdviceContext context)
        {
            var propertyInfo = context.TargetProperty;
            propertyInfo.CreateDependencyProperty(DefaultValue, Notification, CallbackName);
            if (propertyInfo.GetValue(null, NoParameter) == null)
                propertyInfo.SetValue(null, Activator.CreateInstance(propertyInfo.PropertyType), NoParameter);
        }

        /// <summary>
        /// Implements advice logic.
        /// Usually, advice must invoke context.Proceed()
        /// </summary>
        /// <param name="context">The method advice context.</param>
        public void Advise(PropertyAdviceContext context)
        {
            CurrentProperty = context.TargetProperty.GetDependencyProperty();
            context.Proceed();
        }
    }
}