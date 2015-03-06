#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System.Windows;
    using MVVM.Navigation;

    /// <summary>
    /// Extensions to Application
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Gets the navigator related to this application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        public static INavigator GetNavigator(this Application application)
        {
            var key = typeof(Navigator).FullName;
            var navigator = application.Properties[key] as INavigator;
            if (navigator == null)
                application.Properties[key] = navigator = new Navigator();
            return navigator;
        }
    }
}
