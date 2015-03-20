#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion
namespace TestApplication.ViewModel
{
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.Threading;
    using ArxOne.MrAdvice.MVVM.ViewModel;
    using ArxOne.MrAdvice.Utility;

    /// <summary>
    /// View-model for MainView
    /// </summary>
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
        public string PopupSaid { get; set; }


        /// <summary>
        /// This method is called by the navigator once the view-model is initialized.
        /// </summary>
        public void Load()
        {
            PopupSaid = "nothing yet, you have to use it";
            // This method is called when the navigator creates the view-model
            UpdateAutomaticCounter();
        }

        [Async]
        private void UpdateAutomaticCounter()
        {
            for (; ; )
            {
                ++AutomaticCounter;
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Called to raise the PropertyChanged event.
        /// </summary>
        /// <param name="propertyInfo">The property information whose value changed.</param>
        /// <param name="sender">The sender advice (right, you probably won't need it).</param>
        public void OnPropertyChanged(PropertyInfo propertyInfo, NotifyPropertyChanged sender)
        {
            var onPropertyChanged = PropertyChanged;
            if (onPropertyChanged != null)
                onPropertyChanged(this, new PropertyChangedEventArgs(propertyInfo.Name));
        }

        public void ButtonAction()
        {
            ++ButtonActionCount;
        }

        public void OpenPopup()
        {
            // DEMO: navigation opens a window
            var popupViewModel = Navigator.Show<PopupViewModel>();
            // when the popup exits with validatoin it returns itself
            if (popupViewModel != null)
                PopupSaid = "\"" + popupViewModel.Said + "\"";
            else // otherwise, it is null
                PopupSaid = "nothing, user escaped cowardly.";
        }
    }
}
