﻿#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion
namespace TestApplication
{
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.Utility;
    using ViewModel;

    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            Startup += OnStartup;
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            // DEMO: the INavigator.
            // You can also configure you IoC container with INavigator --> Navigator
            var navigator = this.GetNavigator();
            await navigator.Show<MainViewModel>();
        }
    }
}
