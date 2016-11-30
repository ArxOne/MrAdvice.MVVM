#region Mr. Advice MVVM
// Mr. Advice MVVM
// A simple MVVM package using Mr. Advice aspect weaver
// https://github.com/ArxOne/MrAdvice.MVVM
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice.MVVM.View
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using Utility;

    /// <summary>
    /// Marker for collection
    /// </summary>
    public static class Collect
    {
        /// <summary>
        /// The item type property
        /// </summary>
        public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.RegisterAttached(
            "ItemType", typeof(ItemCollectionType), typeof(Collect), new PropertyMetadata(default(ItemCollectionType)));

        /// <summary>
        /// Sets the type of the item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetItemType(DependencyObject element, ItemCollectionType value)
        {
            element.SetValue(ItemTypeProperty, value);
        }

        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ItemCollectionType GetItemType(DependencyObject element)
        {
            return (ItemCollectionType)element.GetValue(ItemTypeProperty);
        }

        /// <summary>
        /// Finds the collecting item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns></returns>
        public static UIElement FindCollectingItem(this UIElement element, ItemCollectionType itemType)
        {
            var lastParent = element;
            // Window is explicitly concatenated because it is not part of visual tree
            foreach (var parent in element.GetVisualSelfAndParents().Concat(new DependencyObject[] { Window.GetWindow(element) }).OfType<UIElement>())
            {
                if (GetItemType(parent).HasFlag(itemType))
                    return element;

                lastParent = parent;
            }
            return lastParent;
        }
    }
}
