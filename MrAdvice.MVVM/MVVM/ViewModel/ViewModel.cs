#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Properties;
    using Threading;

    /// <summary>
    /// View-model base
    /// </summary>
    public class ViewModel : ILoadViewModel, INotifyPropertyChangedViewModel, INotifyDataErrorViewModel
    {
        /// <summary>
        /// Invoked when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called to raise the PropertyChanged event.
        /// </summary>
        /// <param name="propertyInfo">The property information whose value changed.</param>
        /// <param name="sender">The sender advice (right, you probably won't need it).</param>
        [UISync]
        public virtual void OnPropertyChanged(PropertyInfo propertyInfo, NotifyPropertyChanged sender)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyInfo.Name));
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var onPropertyChanged = PropertyChanged;
            if (onPropertyChanged != null)
                onPropertyChanged(this, args);
        }

        /// <summary>
        /// Loads data related to this view-model.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task Load()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        { }

        private readonly IDictionary<string, object[]> _errors = new Dictionary<string, object[]>();
        private readonly object[] _noError = new object[0];

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return GetErrors(propertyName);
        }

        private object[] GetErrors(string propertyName)
        {
            object[] errors;
            if (_errors.TryGetValue(propertyName, out errors))
                return errors;
            return _noError;
        }

        /// <summary>
        /// Determines whether the specified property name has errors.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool HasErrors(string propertyName)
        {
            return GetErrors(propertyName).Length > 0;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool INotifyDataErrorInfo.HasErrors => _errors.Any(kv => kv.Value.Any());

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Sets the errors for the given property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errors">The errors.</param>
        public void SetErrors(string propertyName, IEnumerable errors)
        {
            _errors[propertyName] = errors.Cast<object>().ToArray();

            var errorsChanged = ErrorsChanged;
            if (errorsChanged != null)
                errorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
