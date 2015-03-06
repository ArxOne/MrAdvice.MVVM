#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.ViewModel
{
    using System.ComponentModel;
    using System.Reflection;
    using Properties;

    public interface INotifyPropertyChangedViewModel : INotifyPropertyChanged
    {
        void OnPropertyChanged(PropertyInfo propertyInfo, NotifyPropertyChanged sender);
    }
}
