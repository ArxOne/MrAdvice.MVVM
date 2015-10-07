#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
#if !WINDOWS_UWP
    using System;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Extensions to ServiceLocator
    /// </summary>
    internal static class ServiceLocatorAccessor
    {
        /// <summary>
        /// Activates the specified instance type.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <returns></returns>
        public static object Activate(Type instanceType)
        {
            if (ServiceLocator.IsLocationProviderSet)
            {
                try { return ServiceLocator.Current.GetInstance(instanceType); }
                catch (ActivationException) { }
            }
            return null;
        }
    }
#endif
}
