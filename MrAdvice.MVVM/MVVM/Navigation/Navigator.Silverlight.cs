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
    using System.Windows.Controls;
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
            var childWindow = (ChildWindow)view;
            // the Exit() method is called only if the window is still present
            childWindow.Closed += delegate { if (_views.Contains(childWindow)) Exit(false); };
            _views.Push(childWindow);
            childWindow.Show();
            return null;
        }

        /// <summary>
        /// Shows the view/view-model as main window (first window).
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private object ShowMain(UIElement view, ViewModel viewModel)
        {
            _views.Push(view);
            if (!View.Navigator.GetKeepHidden(view))
                Application.Current.RootVisual = view;
            return viewModel;
        }

        /// <summary>
        /// Exits the view.
        /// </summary>
        /// <param name="validate">for a dialog, true if the result has to be used</param>
        public void Exit(bool validate)
        {
            var view = _views.Pop();
            if (_views.Count == 0)
            {
                var onExiting = Exiting;
                if (onExiting != null)
                    onExiting(this, EventArgs.Empty);
            }
            else
            {
                var childWindow = (ChildWindow) view;
                childWindow.DialogResult = validate;
                childWindow.Close();
            }
        }
    }
}
