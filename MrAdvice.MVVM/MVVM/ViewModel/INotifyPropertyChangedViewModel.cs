#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.ViewModel
{
    using System.ComponentModel;
    using System.Reflection;
    using Properties;

    /// <summary>
    /// In order to raise property changed events, view-models must implement this interface.
    /// Lazy dudes can also use the <see cref="ViewModel"/> class which implements this already.
    /// </summary>
    public interface INotifyPropertyChangedViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Called to raise the PropertyChanged event.
        /// </summary>
        /// <param name="propertyInfo">The property information whose value changed.</param>
        /// <param name="sender">The sender advice (right, you probably won't need it).</param>
        void OnPropertyChanged(PropertyInfo propertyInfo, NotifyPropertyChanged sender);
    }
}
