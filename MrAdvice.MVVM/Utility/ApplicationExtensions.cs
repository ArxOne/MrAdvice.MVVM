﻿#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System.Windows;
    using System.Windows.Threading;
    using MVVM.Navigation;

    /// <summary>
    /// Extensions to Application
    /// </summary>
    public static class ApplicationExtensions
    {
#if SILVERLIGHT
        private static INavigator _navigator;
#endif
        /// <summary>
        /// Gets the navigator related to this application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        public static INavigator GetNavigator(this Application application)
        {
#if SILVERLIGHT
            if (_navigator == null)
                _navigator = new Navigator();
            return _navigator;
#else
            var key = typeof(Navigator).FullName;
            var navigator = application.Properties[key] as INavigator;
            if (navigator == null)
                application.Properties[key] = navigator = new Navigator();
            return navigator;
#endif
        }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        public static Dispatcher GetDispatcher(this Application application)
        {
#if SILVERLIGHT
            var dispatcher = Deployment.Current.Dispatcher;
#else
            var dispatcher = application.Dispatcher;
#endif
            return dispatcher;
        }
    }
}
