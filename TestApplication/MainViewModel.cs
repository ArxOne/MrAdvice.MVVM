#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace TestApplication
{
    using System.ComponentModel;
    using System.Reflection;
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.ViewModel;

    /// <summary>
    /// View-model for MainView
    /// </summary>
    public class MainViewModel : INotifyPropertyChangedViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChanged]
        public int ButtonActionCount { get; set; }

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
    }
}
