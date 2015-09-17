#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using ViewModel = System.Object;

    partial class Navigator
    {
        public event EventHandler Exiting;

        /// <summary>
        /// Shows the view/view-model as dialog.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private async Task<ViewModel> ShowDialog(UIElement view, ViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shows the view/view-model as main window (first window).
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private async Task<ViewModel> ShowMain(UIElement view, ViewModel viewModel)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //rootFrame.NavigationFailed += OnNavigationFailed;

                //if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Content = view;
               //rootFrame.Navigate(view.GetType(), e.Arguments);
                //rootFrame.Navigate(view.GetType(), null);
            }
            // Ensure the current window is active
            Window.Current.Activate();
            return viewModel;
        }

        /// <summary>
        /// Called when the visible property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exits the view.
        /// </summary>
        /// <param name="validate">for a dialog, true if the result has to be used</param>
        public void Exit(bool validate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        private void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
