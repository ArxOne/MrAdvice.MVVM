﻿#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Navigation interface, to be injected in view-models
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Occurs when exiting.
        /// </summary>
        event EventHandler Exiting;

        /// <summary>
        /// Occurs when creating a new instance.
        /// </summary>
        event EventHandler<CreatingInstanceEventArgs> CreatingInstance;

        /// <summary>
        /// Occurs when an instance is created.
        /// </summary>
        event EventHandler<CreatedInstanceEventArgs> CreatedInstance;

        /// <summary>
        /// Configures the specified view model type to be used with view type.
        /// This step is unnecessary if view/view-model pairs types use the suffix "View" and "ViewModel"
        /// (convention over configuration)
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewType">Type of the view.</param>
        void Associate(Type viewModelType, Type viewType);

        /// <summary>
        /// Shows the specified view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModelInitializer"></param>
        /// <returns>The view model if dialog is OK, null if cancelled</returns>
        Task<object> Show(Type viewModelType, Func<object, Task> viewModelInitializer = null);

        /// <summary>
        /// Creates the view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModelInitializer">The view model initializer.</param>
        /// <returns></returns>
        Task<object> CreateViewModel(Type viewModelType, Func<object, Task> viewModelInitializer);

        /// <summary>
        /// Gets (usually creates) a view related to an existing view-model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        Task<FrameworkElement> CreateView(object viewModel);

        /// <summary>
        /// Exits the view.
        /// </summary>
        /// <param name="validate">for a dialog, true if the result has to be used</param>
        void Exit(bool validate);
    }
}
