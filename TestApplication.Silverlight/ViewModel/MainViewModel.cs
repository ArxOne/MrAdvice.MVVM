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
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.Threading;
    using ArxOne.MrAdvice.MVVM.ViewModel;

    public class MainViewModel : INotifyPropertyChangedViewModel, ILoadViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // DEMO: the NotifyPropertyChanged aspect
        [NotifyPropertyChanged]
        public int ButtonActionCount { get; set; }

        [NotifyPropertyChanged]
        public int AutomaticCounter { get; set; }

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
        public void Load()
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
    }
}
