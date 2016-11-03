#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.Silverlight.ViewModel
{
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.MVVM.ViewModel;
    using ArxOne.MrAdvice.Utility;

    public class PopupViewModel
    {
        public INavigator Navigator { get { return Application.Current.GetNavigator(); } }

        public string Answer;

        [Command]
        public void FortyTwo()
        {
            Answer = "42";
            Navigator.Exit(true);
        }

        [Command]
        public void OutThere()
        {
            Answer = "The truth is out there";
            Navigator.Exit(true);
        }
    }
}
