#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
#if !WINDOWS_UWP
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Utility;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Allows to bind commands directly to view-model methods
    /// Syntax: Command="{controls:Command {Binding methodName}}"
    /// </summary>
    public class CommandExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public object Name { get; set; }

        private object _parameter;
        private bool _parameterSet;

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public object Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                _parameterSet = true;
            }
        }

        internal FrameworkElement Element { get; private set; }

        /// <summary>
        /// Occurs when [command].
        /// </summary>
        public event EventHandler Command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExtension"/> class.
        /// </summary>
        public CommandExtension()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExtension"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CommandExtension(object name)
        {
            Name = name;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>
        /// Requested service or null.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var targetObject = provideValueTarget.TargetObject;
            var element = targetObject as FrameworkElement;
            if (element == null)
                return this;

            // no need to go further in design mode
#if WINDOWS_UWP
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return null;
#else
            if (DesignerProperties.GetIsInDesignMode(element))
                return null;
#endif

            Element = element;
            var targetProperty = provideValueTarget.TargetProperty;
            element.DataContextChanged += delegate
            {
                var viewModel = element.DataContext;
                if (viewModel == null)
                    return;

                var name = Name;
                var bindingParameter = name as Binding;
                // because we bind to a method, this allows us to have a syntax control in XAML editor
                if (bindingParameter != null)
                    name = viewModel.GetType().GetMember(bindingParameter.Path.Path).FirstOrDefault();

                var command = new RelayCommand(viewModel, name, _parameterSet ? () => Parameter : (Func<object>)null);
                command.Command += (sender, e) => Command?.Invoke(sender, e);
                // TODO: the SetCommand extension method could be much better, especially use directly dependency property
                // kept here for compatibility, but should be removed
                if (!element.SetCommand(targetProperty, command, Parameter))
                {
                    var targetDependencyProperty = targetProperty as DependencyProperty;
                    if (targetDependencyProperty != null)
                        element.SetValue(targetDependencyProperty, command);
                }
            };

            return null;
        }
    }
#endif
}
