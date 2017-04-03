#region Mr. Advice MVVM

// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php

#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Documents;
    using Utility;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using SystemDependencyProperty = Windows.UI.Xaml.DependencyProperty;
#else
    using System.Windows;
    using SystemDependencyProperty = System.Windows.DependencyProperty;

#endif

    /// <summary>
    /// This class holds all auto DependencyProperties, grouped by control type
    /// </summary>
    public static class DependencyProperties
    {
        private static readonly IDictionary<Type, IDictionary<string, SystemDependencyProperty>> RegisteredTypes
            = new Dictionary<Type, IDictionary<string, SystemDependencyProperty>>();

        /// <summary>
        /// Gets the dependency property matching the given PropertyInfo.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns></returns>
        public static SystemDependencyProperty GetDependencyProperty(this PropertyInfo propertyInfo)
        {
            var dependencyProperties = GetDependencyProperties(propertyInfo);
            SystemDependencyProperty property;
            dependencyProperties.TryGetValue(propertyInfo.Name, out property);
            return property;
        }

        /// <summary>
        /// Creates the dependency property related to a property.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="defaultValue">The default value (null if none).</param>
        /// <param name="notification">The notification type, in order to have a callback.</param>
        /// <param name="callbackName">Name of the callback.</param>
        /// <param name="bindsTwoWayByDefault">if set to <c>true</c> [binds two way by default].</param>
        /// <param name="defaultUpdateSourceTrigger">The default update source trigger.</param>
        public static void CreateDependencyProperty(this PropertyInfo propertyInfo, object defaultValue,
            DependencyPropertyNotification notification = DependencyPropertyNotification.None, string callbackName = null,
            bool bindsTwoWayByDefault = false, UpdateSourceTrigger defaultUpdateSourceTrigger = UpdateSourceTrigger.Default)
        {
            var dependencyProperties = GetDependencyProperties(propertyInfo);
            var ownerType = propertyInfo.DeclaringType;
            var propertyName = propertyInfo.Name;
            var defaultPropertyValue = defaultValue == SystemDependencyProperty.UnsetValue ? propertyInfo.PropertyType.Default() : defaultValue;
            var onPropertyChanged = GetPropertyChangedCallback(propertyName, ownerType, notification, callbackName);
            if (propertyInfo.IsStatic())
            {
                // property type is very specific here, because it comes from the second argument of the generic
                var propertyType = propertyInfo.PropertyType.GetGenericArguments()[1];
                dependencyProperties[propertyName] = SystemDependencyProperty.RegisterAttached(propertyName, propertyType, ownerType,
                    CreatePropertyMetadata(defaultPropertyValue ?? propertyType.Default(), onPropertyChanged, bindsTwoWayByDefault, defaultUpdateSourceTrigger));
            }
            else
            {
                dependencyProperties[propertyName] = SystemDependencyProperty.Register(propertyName, propertyInfo.PropertyType, ownerType,
                    CreatePropertyMetadata(defaultPropertyValue ?? propertyInfo.PropertyType.Default(), onPropertyChanged, bindsTwoWayByDefault, defaultUpdateSourceTrigger));
            }
        }

        private static PropertyMetadata CreatePropertyMetadata(object defaultValue, PropertyChangedCallback onPropertyChangedCallback,
            bool bindsTwoWayByDefault, UpdateSourceTrigger updateSourceTrigger)
        {
#if !SILVERLIGHT
            if (bindsTwoWayByDefault || updateSourceTrigger != UpdateSourceTrigger.Default)
            {
                var propertyMetadata = new FrameworkPropertyMetadata(defaultValue, onPropertyChangedCallback);
                propertyMetadata.BindsTwoWayByDefault = bindsTwoWayByDefault;
                if (updateSourceTrigger != UpdateSourceTrigger.Default)
                    propertyMetadata.DefaultUpdateSourceTrigger = updateSourceTrigger;
                return propertyMetadata;
            }
#endif
            return new PropertyMetadata(defaultValue, onPropertyChangedCallback);
        }

        /// <summary>
        /// Gets the property changed callback, based on notification type.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="ownerType">Type of the owner.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="callbackName">Name of the callback.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">notification</exception>
        private static PropertyChangedCallback GetPropertyChangedCallback(string propertyName, Type ownerType, DependencyPropertyNotification notification, string callbackName)
        {
            if (callbackName != null && notification == DependencyPropertyNotification.None)
                notification = DependencyPropertyNotification.OnPropertyNameChanged;
            switch (notification)
            {
                case DependencyPropertyNotification.None:
                    return null;
                case DependencyPropertyNotification.OnPropertyNameChanged:
                    return GetOnPropertyNameChangedCallback(propertyName, ownerType, callbackName);
                default:
                    throw new ArgumentOutOfRangeException(nameof(notification));
            }
        }

        private static PropertyChangedCallback GetOnPropertyNameChangedCallback(string propertyName, Type ownerType, string callbackName)
        {
            PropertyChangedCallback onPropertyChanged;
            var methodName = callbackName ?? $"On{propertyName}Changed";
            var method = ownerType.GetMethod(methodName);
            if (method == null)
                throw new InvalidOperationException("Callback method not found (WTF?)");
            var parameters = method.GetParameters();
            if (method.IsStatic)
            {
                if (parameters.Length == 0)
                {
                    onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(null, new object[0]); };
                }
                else if (parameters.Length == 1)
                {
                    onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(null, new object[] {d}); };
                }
                else if (parameters.Length == 2)
                {
                    if (parameters[1].ParameterType.IsAssignableFrom(typeof(DependencyPropertyChangedEventArgs)))
                    {
                        onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(null, new object[] {d, e}); };
                    }
                    else
                    {
                        onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(null, new[] {d, e.OldValue}); };
                    }
                }
                else if (parameters.Length == 3)
                {
                    onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(null, new[] {d, e.OldValue, e.NewValue}); };
                }
                else
                    throw new InvalidOperationException("Unhandled method overload");
            }
            else
            {
                if (parameters.Length == 0)
                {
                    onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(d, new object[0]); };
                }
                else if (parameters.Length == 1)
                {
                    if (parameters[0].ParameterType.IsAssignableFrom(typeof(DependencyPropertyChangedEventArgs)))
                    {
                        onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(d, new object[] {e}); };
                    }
                    else
                    {
                        onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(d, new[] {e.OldValue}); };
                    }
                }
                else if (parameters.Length == 2)
                {
                    onPropertyChanged = delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) { method.Invoke(d, new[] {e.OldValue, e.NewValue}); };
                }
                else
                    throw new InvalidOperationException("Unhandled method overload");
            }
            return onPropertyChanged;
        }

        /// <summary>
        /// Gets the dependency properties group, based on property.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <returns></returns>
        private static IDictionary<string, SystemDependencyProperty> GetDependencyProperties(PropertyInfo propertyInfo)
        {
            IDictionary<string, SystemDependencyProperty> dependencyProperties;
            var ownerType = propertyInfo.DeclaringType;
            if (!RegisteredTypes.TryGetValue(ownerType, out dependencyProperties))
                RegisteredTypes[ownerType] = dependencyProperties = new Dictionary<string, SystemDependencyProperty>();
            return dependencyProperties;
        }
    }
}