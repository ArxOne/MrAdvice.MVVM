#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.ModernUI
{
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.Utility;
    using ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class TestApp : Application
    {
        public TestApp()
        {
            Startup += OnStartup;
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var navigator = this.GetNavigator();
            await navigator.Show<MainViewModel>();
        }
    }
}