#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Extensions to Binding
    /// </summary>
    public static class BindingExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        public class BindingEvaluator : FrameworkElement
        {
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public object Value
            {
                get
                {
                    Value = DependencyProperty.UnsetValue;
                    SetBinding(BindingEvaluator.ValueProperty, _binding);
                    return (object)GetValue(ValueProperty);
                }
                private set { SetValue(ValueProperty, value); }
            }
            static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(BindingEvaluator), null);

            Binding _binding;
            /// <summary>
            /// Initializes a new instance of the <see cref="BindingEvaluator"/> class.
            /// </summary>
            /// <param name="binding">The binding.</param>
            public BindingEvaluator(Binding binding)
            {
                _binding = binding;
            }
        }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="byReflectionPath">if set to <c>true</c> [by reflection path].</param>
        /// <returns></returns>
        //public static object GetValue(this Binding binding, bool byReflectionPath = false) => BindingReader.GetValue(binding, byReflectionPath);
        public static object GetValue(this Binding binding, bool byReflectionPath = false)
        {
            var bindingEvaluator = new BindingEvaluator(binding);
            return bindingEvaluator.Value;
        }

        /// <summary>
        /// Clones the specified binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public static Binding Clone(this Binding binding)
        {
            var newBinding = new Binding
            {
                BindsDirectlyToSource = binding.BindsDirectlyToSource,
                Converter = binding.Converter,
                ConverterCulture = binding.ConverterCulture,
                ConverterParameter = binding.ConverterParameter,
                FallbackValue = binding.FallbackValue,
                Mode = binding.Mode,
                NotifyOnValidationError = binding.NotifyOnValidationError,
                Path = binding.Path,
                StringFormat = binding.StringFormat,
                TargetNullValue = binding.TargetNullValue,
                UpdateSourceTrigger = binding.UpdateSourceTrigger,
                ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                ValidatesOnExceptions = binding.ValidatesOnExceptions,
                ValidatesOnNotifyDataErrors = binding.ValidatesOnNotifyDataErrors,
#if !SILVERLIGHT
                BindingGroupName = binding.BindingGroupName,
                AsyncState = binding.AsyncState,
                IsAsync = binding.IsAsync,
                Delay = binding.Delay,
                NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                XPath = binding.XPath,
#endif
            };
            if (binding.Source != null)
                newBinding.Source = binding.Source;
            else if (binding.RelativeSource != null)
                newBinding.RelativeSource = binding.RelativeSource;
            else if (binding.ElementName != null)
                newBinding.ElementName = binding.ElementName;
#if !SILVERLIGHT
            foreach (var r in binding.ValidationRules)
                newBinding.ValidationRules.Add(r);
#endif
            return newBinding;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public static object GetSource(this Binding binding)
        {
            var p = binding.Path;
            var m = binding.Mode;
            binding.Path = null;
            binding.Mode=BindingMode.OneWay;
            var s = binding.GetValue();

            binding.Path = p;
            binding.Mode = m;

            return s;

            //var newBinding = binding.Clone();
            //newBinding.Path = new PropertyPath(""); // we need the source itself
            //newBinding.Mode=BindingMode.OneWay;
            //return newBinding.GetValue();
        }
    }
}