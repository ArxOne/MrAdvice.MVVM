#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System.Windows;
    using Properties;

    public static class Navigator
    {
        [Attached]
        public static Property<Window, bool> KeepHidden { get; set; }
    }
}
