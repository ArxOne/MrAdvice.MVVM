#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Media;
#endif
    using System.Windows.Input;

    /// <summary>
    /// Extensions to UI elements
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Gets the object and parents from visual tree.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisualSelfAndParents(this DependencyObject dependencyObject)
        {
            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
        }

#if !SILVERLIGHT && !WINDOWS_UWP
        /// <summary>
        /// Gets the object and parents from logical tree.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetLogicalSelfAndParents(this DependencyObject dependencyObject)
        {
            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
            }
        }
#endif

        /// <summary>
        /// Sets the command to given <see cref="UIElement"/>.
        /// </summary>
        /// <param name="uiElement">The UI element to be inject.</param>
        /// <param name="targetProperty">The target property (where the command is bound).</param>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns></returns>
        public static bool SetCommand(this UIElement uiElement, object targetProperty, ICommand command, object commandParameter)
        {
            return SetCommandAndParameter(uiElement, targetProperty, () => command, () => commandParameter);
        }

        private static bool SetCommandAndParameter(this UIElement uiElement, object targetProperty, Func<ICommand> commandSetter, Func<object> commandParameterSetter)
        {
            string propertyName = null;
#if !SILVERLIGHT && !WINDOWS_UWP
            var dependencyProperty = targetProperty as DependencyProperty;
            if (dependencyProperty != null)
                propertyName = dependencyProperty.Name;
#endif
            var propertyInfo = targetProperty as PropertyInfo;
            if (propertyInfo != null)
                propertyName = propertyInfo.Name;

            if (propertyName == null)
                return false;

            if (commandSetter != null)
            {
                var commandProperty = uiElement.GetType().GetProperty(propertyName);
                if (commandProperty == null)
                    return false;
                commandProperty.SetValue(uiElement, commandSetter(), new object[0]);
            }

            if (commandParameterSetter != null)
            {
                var commandParameterProperty = uiElement.GetType().GetProperty(propertyName + "Parameter");
                if (commandParameterProperty == null)
                    return false;
                commandParameterProperty.SetValue(uiElement, commandParameterSetter(), new object[0]);
            }

            return true;
        }
    }
}
