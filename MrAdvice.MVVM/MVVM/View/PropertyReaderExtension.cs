﻿#region Mr. Advice MVVM

// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php

#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Utility;

    /// <summary>
    /// Allows to read non-readable property
    /// </summary>
    public class PropertyReaderExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public object Property { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyReaderExtension" /> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public PropertyReaderExtension(object property)
        {
            Property = property;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var targetObject = provideValueTarget.TargetObject;
            var element = targetObject as FrameworkElement;

            // no need to go further in design mode
            if (element is not null && DesignerProperties.GetIsInDesignMode(element))
                return null;

            var targetProperty = (PropertyInfo)provideValueTarget.TargetProperty;

            void SetTarget()
            {
                var property = Property;
                // because we bind to a method, this allows us to have a syntax control in XAML editor
                if (property is Binding bindingParameter) property = GetTargetFrameworkElement(targetObject).ReadFromBinding(bindingParameter);
                targetProperty.SetValue(targetObject, property, new object[0]);
            }

            if (element is not null)
                element.DataContextChanged += delegate { SetTarget(); };
            else
            {
                if (targetObject is CommandExtension commandExtension)
                    commandExtension.Command += delegate { SetTarget(); };
            }

            return null;
        }

        private static FrameworkElement GetTargetFrameworkElement(object targetObject)
        {
            if (targetObject is FrameworkElement element)
                return element;
            if (targetObject is CommandExtension commandExtension)
                return commandExtension.Element;
            return null;
        }
    }
}