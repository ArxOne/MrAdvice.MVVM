#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.UWP.ViewModel
{
    using System.ComponentModel;
    using System.Reflection;
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.ViewModel;

    public class MainViewModel : INotifyPropertyChangedViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyInfo propertyInfo, NotifyPropertyChanged sender)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyInfo.Name));
        }
    }
}