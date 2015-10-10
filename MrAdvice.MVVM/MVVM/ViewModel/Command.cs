#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.ViewModel
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Advice;
    using View;

    /// <summary>
    /// Command marker.
    /// This generates a property with the same name as target command
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Command : Attribute, IMethodWeavingAdvice
    {
        private const string Suffix = "'";
        /// <summary>
        /// Advises the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Advise(MethodWeavingContext context)
        {
            context.AddPublicAutoProperty(context.TargetMethodName, typeof(object));
            context.TargetMethodName += Suffix;
            context.AddInitializerOnce(InitializeCommands);
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void InitializeCommands(object viewModel)
        {
            var viewModelType = viewModel.GetType();
            foreach (var method in viewModelType.GetMethods())
            {
                var commandName = GetCommandName(method);
                if (commandName == null)
                    continue;
                var commandProperty = viewModelType.GetProperty(commandName);
                var canCommandProperty = viewModelType.GetProperty("Can" + commandName);
                var command = new RelayCommand(viewModel, method, canCommandProperty);
                commandProperty.SetValue(viewModel, command, new object[0]);
            }
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns></returns>
        private static string GetCommandName(MethodInfo methodInfo)
        {
            if (!methodInfo.Name.EndsWith(Suffix))
                return null;
            if (!methodInfo.GetCustomAttributes(typeof(Command), false).Any())
                return null;
            var commandPropertyName = methodInfo.Name.Substring(0, methodInfo.Name.Length - Suffix.Length);
            return commandPropertyName;
        }
    }
}
