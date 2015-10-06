#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion
namespace ArxOne.MrAdvice.Utility
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Extensions to types
    /// </summary>
    internal static class TypeExtensions
    {
#if WINDOWS_UWP
        public static TypeInfo TypeInfo(this Type type)
        {
            return type.GetTypeInfo();
        }
#else
        public static Type TypeInfo(this Type type)
        {
            return type;
        }
#endif

        /// <summary>
        /// Creates a default instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object Default(this Type type)
        {
            if (type.TypeInfo().IsClass)
                return null;
            return Activator.CreateInstance(type);
        }

    }
}