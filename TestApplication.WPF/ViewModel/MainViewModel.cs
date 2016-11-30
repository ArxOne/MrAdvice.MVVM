#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication.ViewModel
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ArxOne.MrAdvice.MVVM.Navigation;
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.Threading;
    using ArxOne.MrAdvice.MVVM.ViewModel;
    using ArxOne.MrAdvice.Utility;

    /// <summary>
    /// View-model for MainView
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private string _validatedValue;
        public INavigator Navigator => Application.Current.GetNavigator();

        // DEMO: the NotifyPropertyChanged aspect
        [NotifyPropertyChanged]
        public int ButtonActionCount { get; set; }

        [NotifyPropertyChanged]
        public int AutomaticCounter { get; set; }

        [NotifyPropertyChanged]
        public string PopupSaid { get; set; }

        [NotifyPropertyChanged]
        [Required(ErrorMessage = @"Type something!")]
        [RegularExpression(@"\d*", ErrorMessage = @"Only digits")]
        [MaxLength(5, ErrorMessage = @"Only 5 digits at most")]
        public string ValidatedValue
        {
            get { return _validatedValue; }
            set
            {
                _validatedValue = value;
                if (_validatedValue == "42")
                    throw new ValidationException("Not this answer!");
            }
        }

        public MainItemViewModel[] Items { get; } =
            {
                new MainItemViewModel(),
                new MainItemViewModel(),
            };

        /// <summary>
        /// This method is called by the navigator once the view-model is initialized.
        /// </summary>
        public override async Task Load()
        {
            PopupSaid = "nothing yet, you have to use it";
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

        [NotifyPropertyChanged]
        public bool CanButtonAction { get; set; } = true;

        [Command]
        public void ButtonAction()
        {
            ++ButtonActionCount;
        }

        public async void OpenPopup()
        {
            // DEMO: navigation opens a window
            var popupViewModel = await Navigator.Show<PopupViewModel>();
            // when the popup exits with validatoin it returns itself
            if (popupViewModel != null)
                PopupSaid = "\"" + popupViewModel.Said + "\"";
            else // otherwise, it is null
                PopupSaid = "nothing, user escaped cowardly.";
        }

        public async void Exit()
        {
            Navigator.Exit(false);
        }

        [NotifyPropertyChanged]
        public string EnteredPassword { get; set; }

        public void ShowPassword(string enteredPassword)
        {
            EnteredPassword = enteredPassword;
        }

        [NotifyPropertyChanged]
        public string LongCommandStatus { get; set; }

        public async Task LongCommand()
        {
            for (int s = 5; s > 0; s--)
            {
                LongCommandStatus = $"Hold on... {s}s";
                await Task.Delay(1000);
            }
            LongCommandStatus = "";
        }
    }
}
