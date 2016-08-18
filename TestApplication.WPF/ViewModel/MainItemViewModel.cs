#region Mr. Advice MVVM

// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php

#endregion

namespace TestApplication.ViewModel
{
    using ArxOne.MrAdvice.MVVM.Properties;
    using ArxOne.MrAdvice.MVVM.ViewModel;

    public class MainItemViewModel: ViewModel
    {
        [NotifyPropertyChanged]
        public int Value { get; set; }

        public void AddOne()
        {
            ++Value;
        }
    }
}