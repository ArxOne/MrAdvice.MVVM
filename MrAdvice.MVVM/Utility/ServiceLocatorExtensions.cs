#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Extensions to ServiceLocator
    /// </summary>
    internal static class ServiceLocatorExtensions
    {
        public static object GetOrCreateInstance(this IServiceLocator serviceLocator, Type instanceType)
        {
            try
            {
                return serviceLocator.GetInstance(instanceType);
            }
            catch (ActivationException)
            {
                return Activator.CreateInstance(instanceType);
            }
        }
    }
}
