#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    /// <summary>
    /// Priorities for aspects
    /// Because I hate hard-coded numeric values
    /// </summary>
    public static class AspectPriority
    {
        /// <summary>
        /// Notification level: the property changes, and we take good note of it
        /// </summary>
        public const int Notification = 100;
        /// <summary>
        /// Lowest level, if data is stored virtually (not in the property's backing field)
        /// </summary>
        public const int DataHolder = 10;
    }
}
