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
    using System.Windows;
    using Annotations;
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
        [Attached]
        public static Property<Window, bool> WasShown { get; set; }

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
            var viewModel = (ViewModel)GetOrCreateInstance(viewModelType);
            // initializer comes first
            if (initializer != null)
                await initializer(viewModel);
            // load comes second
            var loadViewModel = viewModel as ILoadViewModel;
            if (loadViewModel != null)
                await loadViewModel.Load();
            var viewType = GetViewType(viewModelType);
            var view = (FrameworkElement)GetOrCreateInstance(viewType);
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
        private static object GetOrCreateInstance(Type type)
        {
            return ServiceLocatorAccessor.Activate(type) ?? Activator.CreateInstance(type);
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
            var viewType = GetViewTypeFromConvention(viewModelType, "ViewModel", "View");
            if (viewType == null)
                return null;
            _viewByViewModel[viewModelType] = viewType;
            return viewType;
        }

        /// <summary>
        /// Gets the view type from naming convention.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModelSuffix">The view model suffix.</param>
        /// <param name="viewSuffix">The view suffix.</param>
        /// <returns></returns>
        private static Type GetViewTypeFromConvention(Type viewModelType, string viewModelSuffix, string viewSuffix)
        {
            if (!viewModelType.Name.EndsWith(viewModelSuffix))
                return null;
            var rawName = viewModelType.Name.Substring(0, viewModelType.Name.Length - viewModelSuffix.Length);
            var viewTypeName = rawName + viewSuffix;
            return FindViewType(viewModelType.Namespace, viewTypeName) ?? FindViewType(viewModelType.Assembly, viewTypeName) ?? FindViewType(viewTypeName);
        }

        /// <summary>
        /// Finds the view type given a name and namespace.
        /// </summary>
        /// <param name="ns">The ns.</param>
        /// <param name="viewTypeName">Name of the view type.</param>
        /// <returns></returns>
        private static Type FindViewType(string ns, string viewTypeName)
        {
            return Type.GetType(ns + "." + viewTypeName);
        }

        private static Type FindViewType(Assembly assembly, string viewTypeName)
        {
            return assembly.GetTypes().SingleOrDefault(t => t.Name == viewTypeName);
        }

        private static Type FindViewType(string viewTypeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => FindViewType(assembly, viewTypeName)).FirstOrDefault(viewType => viewType != null);
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
    }
}
