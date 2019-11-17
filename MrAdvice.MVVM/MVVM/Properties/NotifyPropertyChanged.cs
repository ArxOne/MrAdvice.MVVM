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

        private static readonly ValidationResult[] NoValidationResult = new ValidationResult[0];

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
                bool validated = false;
                var newValue = context.Value;
                var oldValue = context.TargetProperty.GetValue(context.Target, context.Index.ToArray());
                ValidationException validationException = null;

                try
                {
                    // first, set the value
                    context.Proceed();
                }
                catch (ValidationException e)
                {
                    validationException = e;
                }

                // handle ValidationException, if any
                if (context.Target is INotifyDataErrorViewModel notifyDataErrorInfoViewModel)
                {
                    // in all cases, set or clear errors
                    if (validationException != null)
                    {
                        validated = true; // no need to carry on validation. Exceptions are priority
                        notifyDataErrorInfoViewModel.SetErrors(context.TargetProperty.Name, new[] { validationException.ValidationResult });
                    }
                    else // no exception
                        notifyDataErrorInfoViewModel.SetErrors(context.TargetProperty.Name, NoValidationResult);
                }

                // then, notify, if it has changed
                if (!oldValue.SafeEquals(newValue))
                {
                    if (!(context.Target is INotifyPropertyChangedViewModel viewModel))
                        throw new InvalidOperationException("ViewModel must implement INotifyPropertyChangedViewModel");
                    viewModel.OnPropertyChanged(context.TargetProperty, this);
                }

                // validation comes after actual setter
                // this is not convenient, but otherwise it fails the display
                if (!validated)
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
            if (context.Target is INotifyDataErrorViewModel notifyDataErrorInfoViewModel)
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
            var validationAttributes = targetProperty.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>().ToArray();
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
