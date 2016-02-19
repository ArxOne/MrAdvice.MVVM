#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    /// <summary>
    /// Object extensions 
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Equals, with null support.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool SafeEquals(this object a, object b)
        {
            if (a == null || b == null)
                return (a == null) == (b == null);
            return a.Equals(b);
        }

        /// <summary>
        /// Reads the property.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        public static object ReadPropertyFromPath(this object source, string propertyPath)
        {
            var property = source.GetType().GetProperty(propertyPath);
            if (property == null)
                return null;
            return property.GetValue(source, new object[0]);
        }
    }
}
