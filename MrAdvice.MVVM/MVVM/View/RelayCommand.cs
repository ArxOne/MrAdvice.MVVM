#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion
namespace ArxOne.MrAdvice.MVVM.View
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Input;

    internal partial class RelayCommand : ICommand
    {
        private readonly object _viewModel;
        private MethodBase _commandMethod;

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

        private void SetCommand(object parameter)
        {
            // a method is bound directly
            _commandMethod = parameter as MethodBase;
            if (_commandMethod != null)
                return;

            // otherwise, search by its name
            var commandString = (string)parameter;
            _commandMethod = _viewModel.GetType().GetMethod(commandString);
            if (_commandMethod == null)
                throw new InvalidOperationException($"Command '{commandString}' not found");
        }

        public bool CanExecute(object parameter)
        {
            // TODO :)
            return true;
        }

        public void Execute(object parameter)
        {
            var parameters = new List<object>();
            if (_commandMethod.GetParameters().Length > 0)
                parameters.Add(parameter);
            _commandMethod.Invoke(_viewModel, parameters.ToArray());
        }
    }
}
