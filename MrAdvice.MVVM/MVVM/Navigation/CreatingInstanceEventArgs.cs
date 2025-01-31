namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;

    /// <summary>
    /// Used when trying to create a new instance
    /// </summary>
    public class CreatingInstanceEventArgs: EventArgs
    {
        /// <summary>
        /// Gets or set the instance.
        /// By defaults the value is null. If an instance is not provided here, 
        /// the navigator will create one and invoke CreatedInstance event
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public InstanceKind InstanceKind { get; }

        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedInstanceEventArgs" /> class.
        /// </summary>
        /// <param name="instanceKind">Kind of the instance.</param>
        /// <param name="type">The type.</param>
        public CreatingInstanceEventArgs(InstanceKind instanceKind, Type type)
        {
            InstanceKind = instanceKind;
            Type = type;
        }
    }
}