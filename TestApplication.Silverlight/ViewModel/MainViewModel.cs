#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.Silverlight.ViewModel
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.Threading;
    using ArxOne.MrAdvice.MVVM.ViewModel;
    using ArxOne.MrAdvice.Utility;
    using MrAdvice.MVVM.MVVM.ViewModel;

    public class MainViewModel : ViewModel
    {
        public INavigator Navigator => Application.Current.GetNavigator();

        // DEMO: the NotifyPropertyChanged aspect
        [NotifyPropertyChanged]
        public int ButtonActionCount { get; set; }

        [NotifyPropertyChanged]
        public int AutomaticCounter { get; set; }

        [NotifyPropertyChanged]
        public string ImportantAnswer { get; set; } = "no answer until here";

        [NotifyPropertyChanged]
        [Required(ErrorMessage = @"Type something!")]
        [RegularExpression(@"\d*", ErrorMessage = @"Only digits")]
        public string ValidatedValue { get; set; }

        /// <summary>
        /// This method is called by the navigator once the view-model is initialized.
        /// </summary>
        public override async Task Load()
        {
            // This method is called when the navigator creates the view-model
            UpdateAutomaticCounter();
        }

        [Async]
        private void UpdateAutomaticCounter()
        {
            for (;;)
            {
                ++AutomaticCounter;
                Thread.Sleep(1000);
            }
        }

        [Command]
        public void ButtonAction()
        {
            ++ButtonActionCount;
        }

        [Command]
        public async void BigQuestion()
        {
            var viewModel = await Navigator.Show<PopupViewModel>();
            ImportantAnswer = viewModel?.Answer ?? "you escaped the answer :'(";
        }
    }
}
