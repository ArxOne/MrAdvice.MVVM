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
    using System.Windows;
    using ViewModel = System.Object;

    partial class Navigator
    {
        [Attached]
        public static Property<Window, bool> WasShown { get; set; }

        public event EventHandler Exiting;

        /// <summary>
        /// Shows the view/view-model as dialog.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private async Task<ViewModel> ShowDialog(UIElement view, ViewModel viewModel)
        {
            var window = (Window)view;
            window.Owner = (Window)_views.Peek();
            // the Exit() method is called only if the window is still present
            window.Closed += delegate { _views.Pop(); };
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
        private async Task<ViewModel> ShowMain(UIElement view, ViewModel viewModel)
        {
            var window = view as Window;
            // This is a very dirty hack. I'm not proud of it.
            if (!View.Navigator.GetKeepHidden(view))
            {
                if (window != null)
                {
                    window.Show();
                    if (window.ShowActivated)
                        window.Activate();
                    window.Closed += delegate { Shutdown(); };
                }
            }
            _views.Push(view);
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
            var view = _views.Peek();
            // down to first window?
            if (_views.Count == 1)
            {
                // for windows, we invoke their close method, wich then invokes the Shutdown()
                var window = view as Window;
                if (window != null)
                    window.Close();
                else // other it is a direct shutdown
                    Shutdown();
            }
            else // otherwise simple exit from dialog
                ((Window)view).DialogResult = validate;
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        private void Shutdown()
        {
            var onExiting = Exiting;
            if (onExiting != null)
                onExiting(this, EventArgs.Empty);
            // this is not something I'm very proud of
            // TODO: have a nice exit
            Application.Current.DispatcherUnhandledException += (sender, e) => e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}
