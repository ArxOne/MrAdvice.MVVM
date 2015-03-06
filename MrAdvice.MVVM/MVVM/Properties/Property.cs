#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    using System.Windows;
    using Annotations;
    using Utility;

    /// <summary>
    /// This is the property part for attached properties.
    /// 
    /// Declaration:
    /// 
    /// [Attached]
    /// public static Property&lt;TDependencyObject,TValue> MyProperty { get; set; }
    /// 
    /// then use it as follows:
    /// var someValue = MyProperty[dependencyObject]
    /// MyProperty[dependencyObject] = someOtherValue
    /// 
    /// Nice, isn't it?
    /// </summary>
    /// <typeparam name="TDependencyObject">The type of the target.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class Property<TDependencyObject, TValue>
        where TDependencyObject : DependencyObject
    {
        public TValue this[TDependencyObject target]
        {
            get { return ObjectTypeConverter.Convert<TValue>(target.GetValue(Attached.CurrentProperty)); }
            set { target.SetValue(Attached.CurrentProperty, value); }
        }
    }
}
