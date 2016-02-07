#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Properties;
    using Utility;
    using DependencyProperty = System.Windows.DependencyProperty;
    using ViewModel = System.Object;

    partial class Navigator
    {
        [Attached]
        public static Property<Window, bool> WasShown { get; set; }

        [Attached]
        public static Property<FrameworkElement, bool> IsWatched { get; set; }

        public event EventHandler Exiting;

        private FrameworkFeatures _features;
        private ContentControl _modernUIContentControl;

        /// <summary>
        /// Shows the view/view-model as dialog.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private async Task<ViewModel> ShowDialog(UIElement view, ViewModel viewModel)
        {
            if (_features.HasFlag(FrameworkFeatures.UsesContentFrame))
            {
                if (_modernUIContentControl == null)
                    _modernUIContentControl = Application.Current.MainWindow.GetVisualSelfAndChildren().OfType<ContentControl>().Single(c => c.Name == "ContentFrame");

                _modernUIContentControl.Content = view;
                return viewModel;
            }

            var window = (Window)view;
            HandleModernUIWindowNavigation(window);
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
            var autoClose = _features.HasFlag(FrameworkFeatures.AutoClose);
            // This is a very dirty hack. I'm not proud of it.
            if (!View.Navigator.GetKeepHidden(view))
            {
                if (window != null)
                {
                    _features = GetWindowFrameworkFeatures(window);
                    HandleModernUIWindowNavigation(window);
                    window.Show();
                    if (window.ShowActivated)
                        window.Activate();
                    if (!autoClose)
                        window.Closed += delegate { Shutdown(); };
                }
            }
            // auto-close models requires not to keep track of them
            _views.Push(autoClose ? null : view);
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

        private void HandleModernUIWindowNavigation(Window view)
        {
            if (!IsModernUIWindow(view) || IsWatched[view])
                return;
            IsWatched[view] = true;

            view.ApplyTemplate();
            var contentFrame = view.GetVisualSelfAndChildren().OfType<ContentControl>().Single(c => c.Name == "ContentFrame");
            ContentControl.ContentProperty.RegisterChangeCallback(contentFrame, "Content", OnContentChanged);
        }

        private void HandleModernUIContentNavigation(FrameworkElement parentView)
        {
            parentView.ApplyTemplate();
            foreach (var view in parentView.GetVisualSelfAndChildren().OfType<FrameworkElement>())
            {
                if (!IsModernUIContent(view) || IsWatched[view])
                    continue;
                IsWatched[view] = true;

                view.ApplyTemplate();
                var contentFrame = view.GetVisualSelfAndChildren().OfType<Control>().Single(c => c.GetType().Name == "ModernFrame");
                ContentControl.ContentProperty.RegisterChangeCallback(contentFrame, "Content", OnContentChanged);
            }
        }

        private static FrameworkFeatures GetWindowFrameworkFeatures(Window view)
        {
            if (IsModernUIWindow(view))
                return FrameworkFeatures.ModernUI;
            if (IsMahAppsMetroWindow(view))
                return FrameworkFeatures.MahAppsMetroWindow;
            return FrameworkFeatures.Default;
        }

        private static bool IsMahAppsMetroWindow(Window view)
        {
            return view.GetType().GetSelfAndAncestors().Any(t => t.FullName == "MahApps.Metro.Controls.MetroWindow");
        }

        /// <summary>
        /// Determines whether [is modern UI window] [the specified view].
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        private static bool IsModernUIWindow(Window view)
        {
            return view.GetType().GetSelfAndAncestors().Any(t => t.Name == "ModernWindow");
        }

        private static bool IsModernUIContent(UIElement content)
        {
            return content.GetType().GetSelfAndAncestors().Any(t => t.Name == "ModernTab");
        }

        /// <summary>
        /// Called when content has changed.
        /// </summary>
        /// <param name="d">The d.</param>
        private async void OnContentChanged(DependencyObject d)
        {
            var contentControl = (ContentControl)d;
            var content = contentControl.Content as FrameworkElement;
            if (content == null)
                return;
            HandleModernUIContentNavigation(content);
            var viewType = content.GetType();
            // now find associated view-model
            var viewModelType = GetViewModelType(viewType);
            if (viewModelType == null)
                return;
            // then check for data context
            if (contentControl.DataContext == null || contentControl.DataContext.GetType() != viewModelType)
                contentControl.DataContext = await GetOrCreateInstance(viewModelType, InstanceType.ViewModel);
        }
    }
}
