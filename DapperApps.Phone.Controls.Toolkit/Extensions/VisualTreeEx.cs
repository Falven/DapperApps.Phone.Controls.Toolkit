﻿/*
 * Copyright (c) Dapper Apps.  All rights reserved.
 * Use of this sample source code is subject to the terms of the Dapper Apps license 
 * agreement under which you licensed this sample source code and is provided AS-IS.
 * If you did not accept the terms of the license agreement, you are not authorized 
 * to use this sample source code.  For the terms of the license, please see the 
 * license agreement between you and Dapper Apps.
 *
 * To see the article about this app, visit http://www.dapper-apps.com/DapperToolkit
 */

namespace DapperApps.Phone.Extensions
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// A static class providing methods for working with the visual tree.  
    /// </summary>
    internal static class VisualTreeEx
    {
        /// <summary>
        /// Retrieves all the visual children of a framework element.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The visual children of the framework element.</returns>
        internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }

        /// <summary>
        /// Retrieves all the logical children of a framework element using a 
        /// breadth-first search.  A visual element is assumed to be a logical 
        /// child of another visual element if they are in the same namescope.
        /// For performance reasons this method manually manages the queue 
        /// instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        internal static IEnumerable<FrameworkElement> GetLogicalChildrenBreadthFirst(this FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            Queue<FrameworkElement> queue =
                new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                yield return element;

                foreach (FrameworkElement visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }
        }

        /// <summary>
        /// Gets the ancestors of the element, up to the root, limiting the 
        /// ancestors by FrameworkElement.
        /// </summary>
        /// <param name="node">The element to start from.</param>
        /// <returns>An enumerator of the ancestors.</returns>
        internal static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            FrameworkElement parent = node.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        /// <summary>
        /// Gets the visual parent of the element.
        /// </summary>
        /// <param name="node">The element to check.</param>
        /// <returns>The visual parent.</returns>
        internal static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            return VisualTreeHelper.GetParent(node) as FrameworkElement;
        }

        /// <summary>
        /// The first parent of the framework element of the specified type 
        /// that is found while traversing the visual tree upwards.
        /// </summary>
        /// <typeparam name="T">
        /// The element type of the dependency object.
        /// </typeparam>
        /// <param name="element">The dependency object element.</param>
        /// <returns>
        /// The first parent of the framework element of the specified type.
        /// </returns>
        internal static T GetParentByType<T>(this DependencyObject element)
            where T : FrameworkElement
        {
            Debug.Assert(element != null, "The element cannot be null.");

            T result = null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null)
            {
                result = parent as T;

                if (result != null)
                {
                    return result;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}