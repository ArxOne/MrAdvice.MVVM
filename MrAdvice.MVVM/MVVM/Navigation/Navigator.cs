#region Mr. Advice MVVM
// // Mr. Advice MVVM
// // A simple MVVM package using Mr. Advice aspect weaver
// // https://github.com/ArxOne/MrAdvice.MVVM
// // Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif
    using Annotations;
    using global::MrAdvice.MVVM.Utility;
    using Properties;
    using Utility;
    using ViewModel;
    using ViewModel = System.Object;

    /// <summary>
    /// The navigator implements navigation logic
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    internal partial class Navigator : INavigator
    {
        private const string ViewModelTypeSuffix = "ViewModel";
        private const string ViewTypeSuffix = "View";

        private readonly IDictionary<Type, Type> _viewByViewModel = new Dictionary<Type, Type>();

        private readonly Stack<UIElement> _views = new Stack<UIElement>();

        /// <summary>
        /// Configures the specified view model type to be used with view type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewType">Type of the view.</param>
        public void Associate(Type viewModelType, Type viewType)
        {
            _viewByViewModel[viewModelType] = viewType;
        }

        /// <summary>
        /// Shows the specified view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="initializer"></param>
        /// <returns>
        /// The view model if dialog is OK, null if cancelled
        /// </returns>
        public async Task<object> Show(Type viewModelType, Func<object, Task> initializer = null)
        {
            var viewModel = (ViewModel)await GetOrCreateInstance(viewModelType);
            // initializer comes first
            if (initializer != null)
                await initializer(viewModel);
            // load comes second
            var loadViewModel = viewModel as ILoadViewModel;
            if (loadViewModel != null)
                await loadViewModel.Load();
            var viewType = GetViewType(viewModelType);
            var view = (FrameworkElement)await GetOrCreateInstance(viewType);
            view.DataContext = viewModel;
            if (_views.Count == 0)
                return await ShowMain(view, viewModel);
            return await ShowDialog(view, viewModel);
        }

        /// <summary>
        /// Gets or create an instance of given type.
        /// This may use the CommonServiceLocator, assuming it's been correctly configured
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static async Task<object> GetOrCreateInstance(Type type)
        {
            return await GetServiceLocatorInstance(type) ?? Activator.CreateInstance(type);
        }

        private static async Task<object> GetServiceLocatorInstance(Type type)
        {
#if WINDOWS_UWP
            return null;
#else
            // polymorphic approach: if the result is a task, we await for it
            // otherwise, this is straightforward
            var result = ServiceLocatorAccessor.Activate(type);
            var awaitableResult = result as Task<object>;
            if (awaitableResult != null)
                return await awaitableResult;
            return result;
#endif
        }

        /// <summary>
        /// Gets the type of the view matching the view-model.
        /// There are two ways of doing so: by dictionary or convention
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        private Type GetViewType(Type viewModelType)
        {
            return GetViewTypeFromRegistration(viewModelType) ?? GetViewTypeFromConvention(viewModelType);
        }

        /// <summary>
        /// Gets the view type from naming convention.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        private Type GetViewTypeFromConvention(Type viewModelType)
        {
            var viewType = GetTypeFromConvention(viewModelType, ViewModelTypeSuffix, ViewTypeSuffix);
            if (viewType == null)
                return null;
            _viewByViewModel[viewModelType] = viewType;
            return viewType;
        }

        /// <summary>
        /// Gets the view type from naming convention.
        /// </summary>
        /// <param name="referenceType">Type of the view model.</param>
        /// <param name="referenceTypeSuffix">The search type suffix.</param>
        /// <param name="relatedTypeSuffix">The replace type suffix.</param>
        /// <returns></returns>
        private static Type GetTypeFromConvention(Type referenceType, string referenceTypeSuffix, string relatedTypeSuffix)
        {
            if (!referenceType.Name.EndsWith(referenceTypeSuffix))
                return null;
            var referenceBase = referenceType.Name.Substring(0, referenceType.Name.Length - referenceTypeSuffix.Length);
            var relatedTypeName = referenceBase + relatedTypeSuffix;
            return FindType(referenceType.Namespace, relatedTypeName) ?? FindType(referenceType.TypeInfo().Assembly, relatedTypeName) ?? FindType(relatedTypeName);
        }

        /// <summary>
        /// Finds the view type given a name and namespace.
        /// </summary>
        /// <param name="ns">The ns.</param>
        /// <param name="relatedTypeName">Name of the view type.</param>
        /// <returns></returns>
        private static Type FindType(string ns, string relatedTypeName)
        {
            return Type.GetType(ns + "." + relatedTypeName);
        }

        private static Type FindType(Assembly assembly, string relatedTypeName)
        {
            return assembly.GetTypes().SingleOrDefault(t => t.Name == relatedTypeName);
        }

        private static Type FindType(string typeName)
        {
#if WINDOWS_UWP
            var assembly = Application.Current.GetType().GetTypeInfo().Assembly;
            return FindType(assembly, typeName);
#else
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => FindType(assembly, typeName)).FirstOrDefault(viewType => viewType != null);
#endif
        }

        /// <summary>
        /// Gets the view type from registration.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        private Type GetViewTypeFromRegistration(Type viewModelType)
        {
            Type viewType;
            _viewByViewModel.TryGetValue(viewModelType, out viewType);
            return viewType;
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        private Type GetViewModelType(Type viewType)
        {
            return GetViewModelTypeFromRegistration(viewType) ?? GetViewModelTypeFromConvention(viewType);
        }

        /// <summary>
        /// Gets the view-model type from registration.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        private Type GetViewModelTypeFromRegistration(Type viewType)
        {
            return (from kv in _viewByViewModel
                    where kv.Value == viewType
                    select kv.Key).SingleOrDefault();
        }

        /// <summary>
        /// Gets the view-model type from naming convention.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        private Type GetViewModelTypeFromConvention(Type viewType)
        {
            var viewModelType = GetTypeFromConvention(viewType, ViewTypeSuffix, ViewModelTypeSuffix);
            if (viewModelType == null)
                return null;
            _viewByViewModel[viewType] = viewModelType;
            return viewModelType;
        }
    }
}
