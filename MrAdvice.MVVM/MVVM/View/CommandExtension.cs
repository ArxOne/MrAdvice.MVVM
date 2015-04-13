#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Utility;

    /// <summary>
    /// Allows to bind commands directly to view-model methods
    /// Syntax: Command="{controls:Command {Binding methodName}}"
    /// </summary>
    public class CommandExtension : MarkupExtension
    {
        private readonly object _name;

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public object Parameter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExtension"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CommandExtension(object name)
        {
            _name = name;
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
            var element = provideValueTarget.TargetObject as FrameworkElement;
            if (element == null)
                return null;

            // no need to go further in design mode
            if (DesignerProperties.GetIsInDesignMode(element))
                return null;

            var targetProperty = provideValueTarget.TargetProperty;
            element.DataContextChanged += delegate
            {
                var viewModel = element.DataContext;
                if (viewModel == null)
                    return;

                var name = _name;
                var bindingParameter = name as Binding;
                // because we bind to a method, this allows us to have a syntax control in XAML editor
                if (bindingParameter != null)
                    name = viewModel.GetType().GetMember(bindingParameter.Path.Path).FirstOrDefault();

                var command = new Command(viewModel, name);
                element.SetCommand(targetProperty, command, Parameter);
            };

            return null;
        }
    }
}
