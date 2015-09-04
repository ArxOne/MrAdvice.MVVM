#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.ViewModel
{
    using System.Threading.Tasks;

    /// <summary>
    /// Loadable view-model
    /// </summary>
    public interface ILoadViewModel
    {
        /// <summary>
        /// This method is called by the navigator once the view-model is initialized.
        /// </summary>
        Task Load();
    }
}
