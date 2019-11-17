#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System;

    /// <summary>
    /// Items to be collected
    /// </summary>
    [Flags]
    public enum ItemCollectionType
    {
        /// <summary>
        /// The key bindings are collected here
        /// </summary>
        KeyBindings = 0x01,
    }
}