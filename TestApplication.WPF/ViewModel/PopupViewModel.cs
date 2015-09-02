#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.ViewModel
{
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.Utility;

    public class PopupViewModel
    {
         public INavigator Navigator { get { return Application.Current.GetNavigator(); } }

       public string Said { get; set; }

        public void SayYes()
        {
            Said = "Yes";
            Navigator.Exit(true);
        }

        public void SayMaybe()
        {
            Said = "Maybe";
            Navigator.Exit(true);
        }
    }
}
