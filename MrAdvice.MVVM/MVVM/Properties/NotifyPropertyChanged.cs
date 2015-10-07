#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Properties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using Advice;
    using Annotation;
    using global::MrAdvice.MVVM.MVVM.ViewModel;
    using Utility;
    using ViewModel;

    /// <summary>
    /// Invokes notify property changed... If the property has actually changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Priority(AspectPriority.Notification)]
    public class NotifyPropertyChanged : Attribute, IPropertyAdvice
    {
        // this field is related to each marked property
        private bool _validated;

        /// <summary>
        /// Implements advice logic.
        /// Usually, advice must invoke context.Proceed()
        /// </summary>
        /// <param name="context">The method advice context.</param>
        /// <exception cref="InvalidOperationException">ViewModel must implement INotifyPropertyChangedViewModel</exception>
        public void Advise(PropertyAdviceContext context)
        {
            if (context.IsGetter)
            {
                context.Proceed();
                if (!_validated)
                    Validate(context, context.ReturnValue);
            }
            else
            {
                var newValue = context.Value;
                var oldValue = context.TargetProperty.GetValue(context.Target, context.Index.ToArray());

                // first, set the value
                context.Proceed();

                // then, notify, if it has changed
                if (!oldValue.SafeEquals(newValue))
                {
                    var viewModel = context.Target as INotifyPropertyChangedViewModel;
                    if (viewModel == null)
                        throw new InvalidOperationException("ViewModel must implement INotifyPropertyChangedViewModel");
                    viewModel.OnPropertyChanged(context.TargetProperty, this);
                }

                // validation comes after actual setter
                // this is not convenient, but otherwise it fails the display
                Validate(context, newValue);
            }
        }

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        private void Validate(PropertyAdviceContext context, object value)
        {
            _validated = true;
            var notifyDataErrorInfoViewModel = context.Target as INotifyDataErrorViewModel;
            if (notifyDataErrorInfoViewModel != null)
                Validate(notifyDataErrorInfoViewModel, context.Target, context.TargetProperty, value);
        }

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="notifyDataErrorViewModel">The notify data error view model.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool Validate(INotifyDataErrorViewModel notifyDataErrorViewModel, object target, PropertyInfo targetProperty, object value)
        {
            var validationAttributes = targetProperty.GetCustomAttributes(typeof (ValidationAttribute), true).Cast<ValidationAttribute>().ToArray();
            if (validationAttributes.Length == 0)
                return true;

            var context = new ValidationContext(target) { DisplayName = targetProperty.Name, MemberName = targetProperty.Name };
            var validationResults = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty(value, context, validationResults);
            notifyDataErrorViewModel.SetErrors(targetProperty.Name, validationResults);
            return valid;
        }
    }
}
