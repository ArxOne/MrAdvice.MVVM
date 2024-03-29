﻿#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

using System.Collections;
using System.ComponentModel;

namespace ArxOne.MrAdvice.MVVM.ViewModel
{
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
