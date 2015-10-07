#region Mr. Advice MVVM

// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php

#endregion

namespace TestApplication.ModernUI.ViewModel
{
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.Utility;

    public class HomeViewModel
    {
        public INavigator Navigator => Application.Current.GetNavigator();

        public async void OpenSettings()
        {
            await Navigator.Show<SettingsViewModel>();
        }
    }
}
