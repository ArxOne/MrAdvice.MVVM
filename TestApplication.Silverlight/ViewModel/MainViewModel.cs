#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.Silverlight.ViewModel
{
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.Threading;
    using ArxOne.MrAdvice.MVVM.ViewModel;
    using ArxOne.MrAdvice.Utility;

    public class MainViewModel : INotifyPropertyChangedViewModel, ILoadViewModel
    {
        public INavigator Navigator { get { return Application.Current.GetNavigator(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        // DEMO: the NotifyPropertyChanged aspect
        [NotifyPropertyChanged]
        public int ButtonActionCount { get; set; }

        [NotifyPropertyChanged]
        public int AutomaticCounter { get; set; }

        [NotifyPropertyChanged]
        public string ImportantAnswer { get; set; } = "no answer until here";

        [UISync]
        public void OnPropertyChanged(PropertyInfo propertyInfo, NotifyPropertyChanged sender)
        {
            var onPropertyChanged = PropertyChanged;
            if (onPropertyChanged != null)
                onPropertyChanged(this, new PropertyChangedEventArgs(propertyInfo.Name));
        }

        /// <summary>
        /// This method is called by the navigator once the view-model is initialized.
        /// </summary>
        public async Task Load()
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

        public void ButtonAction()
        {
            ++ButtonActionCount;
        }

        public async void BigQuestion()
        {
            var viewModel = await Navigator.Show<PopupViewModel>();
            ImportantAnswer = viewModel?.Answer ?? "you escaped the answer :'(";
        }
    }
}
