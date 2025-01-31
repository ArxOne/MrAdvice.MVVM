#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

#pragma warning disable CS1998

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Annotations;
    using Utility;
    using ViewModel;
    using System.Windows;
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

        public event EventHandler<CreatingInstanceEventArgs> CreatingInstance;
        public event EventHandler<CreatedInstanceEventArgs> CreatedInstance;

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
        /// <param name="viewModelInitializer"></param>
        /// <returns>
        /// The view model if dialog is OK, null if cancelled
        /// </returns>
        public async Task<object> Show(Type viewModelType, Func<object, Task> viewModelInitializer = null)
        {
            var viewModel = await CreateViewModel(viewModelType, viewModelInitializer);
            var viewType = GetViewType(viewModelType);
            var view = (FrameworkElement)CreateInstance(viewType, InstanceKind.View);
            view.DataContext = viewModel;
            if (_views.Count == 0)
                return ShowMain(view, viewModel);
            return ShowDialog(view, viewModel);
        }

        public async Task<object> CreateViewModel(Type viewModelType, Func<object, Task> viewModelInitializer)
        {
            var viewModel = CreateInstance(viewModelType, InstanceKind.ViewModel);
            // initializer comes first
            if (viewModelInitializer != null)
                await viewModelInitializer(viewModel);
            // load comes second
            if (viewModel is ILoadViewModel loadViewModel)
                await loadViewModel.Load();
            return viewModel;
        }

        /// <summary>
        /// Gets (usually creates) a view related to an existing view-model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        public async Task<FrameworkElement> CreateView(ViewModel viewModel)
        {
            var viewType = GetViewType(viewModel.GetType());
            var view = (FrameworkElement)CreateInstance(viewType, InstanceKind.View);
            view.DataContext = viewModel;
            return view;
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instanceKind">Type of the instance.</param>
        /// <returns></returns>
        private object CreateInstance(Type type, InstanceKind instanceKind)
        {
            var onCreatingInstance = CreatingInstance;
            if (onCreatingInstance is not null)
            {
                var e = new CreatingInstanceEventArgs(instanceKind, type);
                onCreatingInstance.Invoke(this, e);
                if (e.Instance is not null)
                    return e.Instance;
            }
            var instance = Activator.CreateInstance(type);
            CreatedInstance?.Invoke(this, new CreatedInstanceEventArgs(instance, instanceKind));
            return instance;
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
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => FindType(assembly, typeName)).FirstOrDefault(viewType => viewType != null);
        }

        /// <summary>
        /// Gets the view type from registration.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        private Type GetViewTypeFromRegistration(Type viewModelType)
        {
            _viewByViewModel.TryGetValue(viewModelType, out var viewType);
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
