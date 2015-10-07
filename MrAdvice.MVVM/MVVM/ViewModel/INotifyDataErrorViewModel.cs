#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace MrAdvice.MVVM.MVVM.ViewModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// View-model implementing data error notifications
    /// </summary>
    public interface INotifyDataErrorViewModel : INotifyDataErrorInfo
    {
        /// <summary>
        /// Sets the errors for the given property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errors">The errors.</param>
        void SetErrors(string propertyName, IEnumerable errors);
    }
}
