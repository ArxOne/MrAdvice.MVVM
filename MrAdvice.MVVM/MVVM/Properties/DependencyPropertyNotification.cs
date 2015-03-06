#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    /// <summary>
    /// Notification type for DependencyPropertyChanged
    /// </summary>
    public enum DependencyPropertyNotification
    {
        /// <summary>
        /// No notification (this is the default)
        /// </summary>
        None,

        /// <summary>
        /// Invokes a notification method named "On&lt;PropertyName>Changed"
        /// </summary>
        OnPropertyNameChanged,
    }
}