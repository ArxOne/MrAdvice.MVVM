#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Windows;
    using ViewModel = System.Object;

    partial class Navigator
    {

        /// <summary>
        /// Shows the view/view-model as dialog.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private object ShowDialog(UIElement view, ViewModel viewModel)
        {
            var window = view as Window;
            window.Owner = (Window)_views.Peek();
            // the Exit() method is called only if the window is still present
            window.Closed += delegate { if (_views.Contains(window)) Exit(false); };
            _views.Push(window);
            var ok = window.ShowDialog();
            return ok ?? (false) ? viewModel : null;
        }

        /// <summary>
        /// Shows the view/view-model as main window (first window).
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private object ShowMain(UIElement view, ViewModel viewModel)
        {
            var window = view as Window;
            _views.Push(view);
            // This is a very dirty hack. I'm not proud of it.
            if (!View.Navigator.GetKeepHidden(view))
            {
                if (window != null)
                {
                    window.Show();
                    if (window.ShowActivated)
                        window.Activate();
                }
            }
            view.IsVisibleChanged += OnVisibleChanged;
            return viewModel;
        }

        /// <summary>
        /// Called when the visible property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window)sender;
            if (!WasShown[window] && window.IsVisible)
            {
                WasShown[window] = true;
                window.Show();
                window.Activate();
            }
        }

        /// <summary>
        /// Exits the view.
        /// </summary>
        /// <param name="validate">for a dialog, true if the result has to be used</param>
        public void Exit(bool validate)
        {
            var view = (Window)_views.Pop();
            if (_views.Count == 0)
            {
                var onExiting = Exiting;
                if (onExiting != null)
                    onExiting(this, EventArgs.Empty);
                // this is not something I'm very proud of
                // TODO: have a nice exit
                Application.Current.DispatcherUnhandledException += (sender, e) => e.Handled = true;
                Application.Current.Shutdown();
            }
            else
            {
                view.DialogResult = validate;
                view.Close();
            }
        }
    }
}
