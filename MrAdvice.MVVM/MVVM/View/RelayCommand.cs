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
    using System.Windows.Input;

    internal partial class RelayCommand : ICommand
    {
        private readonly object _viewModel;
        private MethodBase _commandMethod;
        private string _canCommandPropertyName;
        private bool _canExecute;
        private PropertyInfo _canExecuteProperty;

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="parameter">The parameter.</param>
        public RelayCommand(object viewModel, object parameter)
        {
            _viewModel = viewModel;
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
            _commandMethod = parameter as MethodBase;
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
                _canExecute = true;
                return;
            }

            // check the property initial value
            GetCanExecute();

            var notifyPropertyChanged = _viewModel as INotifyPropertyChanged;
            if (notifyPropertyChanged == null)
                return;

            // and stay tuned
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }

        private void GetCanExecute()
        {
            _canExecute = (bool)_canExecuteProperty.GetValue(_viewModel, new object[0]);
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
                GetCanExecute();
                var canExecuteChanged = CanExecuteChanged;
                if (canExecuteChanged != null)
                    canExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter) => _canExecute;

        public void Execute(object parameter)
        {
            var parameters = new List<object>();
            if (_commandMethod.GetParameters().Length > 0)
                parameters.Add(parameter);
            _commandMethod.Invoke(_viewModel, parameters.ToArray());
        }
    }
}
