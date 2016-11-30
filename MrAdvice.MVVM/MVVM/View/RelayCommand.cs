#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion
namespace ArxOne.MrAdvice.MVVM.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Threading;

    internal partial class RelayCommand : ICommand
    {
        private readonly object _viewModel;
        private readonly Func<object> _commandParameterGetter;
        private MethodInfo _commandMethod;
        private string _canCommandPropertyName;
        private bool _canExecuteCommand;
        private bool? _canExecuteOverride;
        private PropertyInfo _canExecuteProperty;

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        /// <summary>
        /// Occurs when [command].
        /// </summary>
        public event EventHandler Command;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="commandParameterGetter">The command parameter getter.</param>
        public RelayCommand(object viewModel, object parameter, Func<object> commandParameterGetter)
        {
            _viewModel = viewModel;
            _commandParameterGetter = commandParameterGetter;
            SetCommand(parameter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="command">The command.</param>
        /// <param name="canCommand">The can command.</param>
        public RelayCommand(object viewModel, MethodInfo command, PropertyInfo canCommand)
        {
            _viewModel = viewModel;
            _commandMethod = command;
            _canExecuteProperty = canCommand;
            _canCommandPropertyName = canCommand?.Name;
            SetCanExecute();
        }

        private void SetCommand(object parameter)
        {
            if (parameter == null)
                return;

            // a method is bound directly
            _commandMethod = parameter as MethodInfo;
            if (_commandMethod != null)
            {
                SetCanExecute(_commandMethod.Name);
                return;
            }

            // otherwise, search by its name
            var commandString = (string)parameter;
            _commandMethod = _viewModel.GetType().GetMethod(commandString);
            if (_commandMethod == null)
                throw new InvalidOperationException($"Command '{commandString}' not found");
            SetCanExecute(commandString);
        }

        private void SetCanExecute(string commandName)
        {
            _canCommandPropertyName = "Can" + commandName;
            _canExecuteProperty = _viewModel.GetType().GetProperty(_canCommandPropertyName);
            SetCanExecute();
        }

        private void SetCanExecute()
        {
            if (_canExecuteProperty == null)
            {
                _canExecuteCommand = true;
                return;
            }

            // check the property initial value
            ReadCanExecute();

            var notifyPropertyChanged = _viewModel as INotifyPropertyChanged;
            if (notifyPropertyChanged == null)
                return;

            // and stay tuned
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }

        private void ReadCanExecute()
        {
            _canExecuteCommand = (bool)_canExecuteProperty.GetValue(_viewModel, new object[0]);
        }

        /// <summary>
        /// Called when the can execute property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _canCommandPropertyName)
            {
                ReadCanExecute();
                var canExecuteChanged = CanExecuteChanged;
                if (canExecuteChanged != null)
                    canExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter) => _canExecuteOverride ?? _canExecuteCommand;

        public void Execute(object parameter)
        {
            Command?.Invoke(this, EventArgs.Empty);
            var parameters = new List<object>();
            if (_commandMethod.GetParameters().Length > 0)
                parameters.Add(GetParameter(parameter));
            var result = _commandMethod.Invoke(_viewModel, parameters.ToArray());
            // once the command returns, if it is a task and still not complete,
            // we disable the command until the end of task
            var taskResult = result as Task;
            if (taskResult != null && !taskResult.IsCompleted)
            {
                OverrideCanExecute(false);
                taskResult.ContinueWith(t => OverrideCanExecute(null));
            }
        }

        [UISync]
        private void OverrideCanExecute(bool? canExecute)
        {
            _canExecuteOverride = canExecute;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        private object GetParameter(object parameter)
        {
            if (_commandParameterGetter != null)
                return _commandParameterGetter();
            return parameter;
        }
    }
}
