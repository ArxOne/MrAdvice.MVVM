namespace ArxOne.MrAdvice.MVVM.Navigation
{
    using System;

    /// <summary>
    /// When using a framework, identify some features
    /// </summary>
    [Flags]
    internal enum FrameworkFeatures
    {
        /// <summary>
        /// Uses a content frame
        /// </summary>
        UsesContentFrame = 0x0001,

        /// <summary>
        /// The framework manages closes by itself
        /// </summary>
        AutoClose = 0x0002,

        /// <summary>
        /// The default type
        /// </summary>
        Default = 1 << 32,
        /// <summary>
        /// ModernUI
        /// </summary>
        ModernUI = (2 << 32) | UsesContentFrame,
        /// <summary>
        /// The mahapps metro window
        /// </summary>
        MahAppsMetroWindow = (3 << 32) | AutoClose,
    }
}