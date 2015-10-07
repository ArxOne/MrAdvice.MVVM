#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace MrAdvice.MVVM.MVVM.ViewModel
{
    using System.ComponentModel;

    /// <summary>
    /// View-model implementing data error notifications
    /// </summary>
    public interface INotifyDataErrorViewModel: INotifyDataErrorInfo
    {
    }
}
