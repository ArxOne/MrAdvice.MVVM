#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;

    /// <summary>
    /// Call when a new instance is created
    /// </summary>
    public class CreatedInstanceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public InstanceKind InstanceKind { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedInstanceEventArgs" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="instanceKind">Type of the instance.</param>
        /// <param name="type">The type.</param>
        public CreatedInstanceEventArgs(object instance, InstanceKind instanceKind)
        {
            Instance = instance;
            InstanceKind = instanceKind;
        }
    }
}