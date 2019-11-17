#region Mr. Advice MVVM

// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php

#endregion

namespace ArxOne.MrAdvice.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;
    using DependencyProperty = System.Windows.DependencyProperty;

    /// <summary>
    /// Simple wrapper to watch dependency property
    /// </summary>
    internal static class DependencyPropertyWatcher
    {
        public class Watcher : DependencyObject
        {
            public static readonly DependencyProperty WatchedPropertyProperty = DependencyProperty.Register(
                "WatchedProperty", typeof(object), typeof(Watcher), new PropertyMetadata(default(object), (o, args) => ((Watcher)o).Callback(args)));

            public object WatchedProperty
            {
                get { return GetValue(WatchedPropertyProperty); }
                set { SetValue(WatchedPropertyProperty, value); }
            }

            private readonly DependencyObject _source;
            private readonly Action<DependencyObject> _callback;

            public Watcher(DependencyObject source, Action<DependencyObject> callback)
            {
                _source = source;
                _callback = callback;
            }
            private void Callback(DependencyPropertyChangedEventArgs args)
            {
                _callback(_source);
            }
        }

        /// <summary>
        /// This is to keep a reference to watchers, otherwise, they are destroyed
        /// </summary>
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly IList<Watcher> Watchers = new List<Watcher>();

        /// <summary>
        /// Registers the change callback.
        /// The callback method is called when target dependency property is updated
        /// </summary>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterChangeCallback(this DependencyProperty dependencyProperty, DependencyObject source, string sourcePath, Action<DependencyObject> callback)
        {
            var watcher = new Watcher(source, callback);
            Watchers.Add(watcher);
            BindingOperations.SetBinding(watcher, Watcher.WatchedPropertyProperty, new Binding { Source = source, Path = new PropertyPath(sourcePath), Mode = BindingMode.TwoWay });
        }
    }
}
