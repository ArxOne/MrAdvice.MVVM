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
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Xaml;

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

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Key Key { get; set; }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>
        /// The modifiers.
        /// </value>
        public ModifierKeys Modifiers { get; set; }
#endif

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

#if !SILVERLIGHT
            // InputBinding have a special case
            var inputBinding = targetObject as InputBinding;
            if (inputBinding != null)
            {
                var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
                var root = (FrameworkElement)rootObjectProvider.RootObject;
                // once it is bound
                root.DataContextChanged += delegate
                {
                    // find owner
                    var owner = GetOwner(root, inputBinding);
                    if (owner == null)
                        return;
                    var ownerViewModel = owner.DataContext;
                    if (ownerViewModel != null)
                    {
                        // and set command
                        var command = GetCommand(ownerViewModel);
                        inputBinding.Command = command;
                    }
                };
                return null;
            }
#endif

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
                var elementViewModel = element.DataContext;
                if (elementViewModel == null)
                    return;

                var command1 = SetCommand(element, elementViewModel, targetProperty);

#if !SILVERLIGHT
                // keyboard shortcut
                if (Key != Key.None)
                {
                    var keyBinding = new KeyBinding(command1, Key, Modifiers);
                    var collectingItem = element.FindCollectingItem(ItemCollectionType.KeyBindings);
                    if (collectingItem != null)
                    {
                        collectingItem.InputBindings.Add(keyBinding);
                        element.Unloaded += delegate { collectingItem.InputBindings.Remove(keyBinding); };
                    }
                }
#endif
            };
            return null;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets the owner for given <see cref="InputBinding"/>.
        /// Currently this method is very poor, and works only for <see cref="Window"/>
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="inputBinding">The input binding.</param>
        /// <returns></returns>
        private static FrameworkElement GetOwner(DependencyObject root, InputBinding inputBinding)
        {
            var descendants = root.GetVisualSelfAndChildren().OfType<FrameworkElement>();
            var owner = descendants.FirstOrDefault(o => o.InputBindings.Contains(inputBinding));
            return owner;
        }
#endif 

        /// <summary>
        /// Sets the command to target element and property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns></returns>
        private RelayCommand SetCommand(UIElement element, object viewModel, object targetProperty)
        {
            var command = GetCommand(viewModel);
            // TODO: the SetCommand extension method could be much better, especially use directly dependency property
            // kept here for compatibility, but should be removed
            if (!element.SetCommand(targetProperty, command, Parameter))
            {
                var targetDependencyProperty = targetProperty as DependencyProperty;
                if (targetDependencyProperty != null)
                    element.SetValue(targetDependencyProperty, command);
            }
            return command;
        }

        /// <summary>
        /// Gets the bound command, given the view-model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private RelayCommand GetCommand(object viewModel)
        {
            var name = Name;
            var bindingParameter = name as Binding;
            // because we bind to a method, this allows us to have a syntax control in XAML editor
            if (bindingParameter != null)
                name = viewModel.GetType().GetMember(bindingParameter.Path.Path).FirstOrDefault();

            var command = new RelayCommand(viewModel, name, _parameterSet ? () => Parameter : (Func<object>)null);
            command.Command += (sender, e) => Command?.Invoke(sender, e);
            return command;
        }
    }
#endif
}
