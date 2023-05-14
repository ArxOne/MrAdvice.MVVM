#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System;

    /// <summary>
    /// Type converter, with more conversion than System.Convert
    /// </summary>
    internal static class ObjectTypeConverter
    {
        /// <summary>
        /// Converts the specified object to target type.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">Can not create URI</exception>
        public static object Convert(object o, Type targetType)
        {
            if (o is null)
                return targetType.Default();

            if (targetType == typeof(Uri))
                return ConvertToUri(o);

            var sourceType = o.GetType();
            if (sourceType == typeof(Uri))
                return Convert(o.ToString(), targetType);

            var c = System.Convert.ChangeType(o, targetType, null);
            return c;
        }

        private static Uri ConvertToUri(object o)
        {
            try
            {
                return new Uri(Convert<string>(o));
            }
            catch (UriFormatException)
            { }
            try
            {
                return new Uri(Convert<string>(o), UriKind.Relative);
            }
            catch (UriFormatException)
            { }
            throw new InvalidCastException("Can not create URI");
        }

        /// <summary>
        /// Converts the specified object to target type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static T Convert<T>(object o)
        {
            return (T)Convert(o, typeof(T));
        }
    }
}
