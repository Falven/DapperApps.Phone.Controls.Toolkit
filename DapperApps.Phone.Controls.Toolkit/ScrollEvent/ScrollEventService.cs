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

namespace DapperApps.Phone.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Collection of ScrollEventListeners for a ScrollEventService.
    /// </summary>
    public class ScrollEventListenerCollection : Collection<ScrollEventListener>
    {
        private ScrollViewer _target;

        /// <summary>
        /// The target ScrollViewer for all the ScrollEventListeners in this collection.
        /// </summary>
        public ScrollViewer Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                foreach (ScrollEventListener listener in this)
                {
                    listener.Target = _target;
                }
            }
        }

        protected override void InsertItem(int index, ScrollEventListener item)
        {
            base.InsertItem(index, item);
            if (null != item)
            {
                item.Target = Target;
            }
        }
    }

    /// <summary>
    /// Provides an attached ScrollEventListener property
    /// that listens for, and notifies of, various scrolling events.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class ScrollEventService
    {
        #region ScrollEventListenerCollection Attached Property
        /// <summary>
        /// Gets a ScrollEventListener property for the provided DependencyObject, creates a new one if necessary.
        /// </summary>
        /// <param name="obj">The DependencyObject whose ScrollEventListener you would like to get.</param>
        /// <returns>The ScrollEventListener associated with the provided DependencyObject</returns>
        public static ScrollEventListenerCollection GetScrollEventListeners(DependencyObject obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            else
            {
                ScrollEventListenerCollection listeners =
                    (obj.GetValue(ScrollEventListenersProperty) as ScrollEventListenerCollection) ?? new ScrollEventListenerCollection();
                obj.SetValue(ScrollEventListenersProperty, listeners);
                return listeners;
            }
        }

        /// <summary>
        /// Sets the StateScrollListener Attached Property for the provided DependencyObject.
        /// </summary>
        /// <param name="obj">The DependencyObject whose ScrollListener you would like to set.</param>
        /// <param name="value">The ScrollListener you would like to set to the provided DependencyObject.</param>
        public static void SetScrollEventListeners(DependencyObject obj, ScrollEventListener value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(ScrollEventListenersProperty, value);
        }

        /// <summary>
        /// The ScrollEventListener attached property that provides different scrolling events to listen to.
        /// </summary>
        public static readonly DependencyProperty ScrollEventListenersProperty =
            DependencyProperty.RegisterAttached(
                "ScrollEventListeners",
                typeof(ScrollEventListenerCollection),
                typeof(ScrollEventService),
                new PropertyMetadata(null, OnScrollEventListenersChanged));
        #endregion

        /// <summary>
        /// Handles changes to the ScrollEventListeners DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnScrollEventListenersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement el = o as FrameworkElement;
            if (null != el)
            {
                el.Loaded +=
                    (s, e2) =>
                    {
                        var element = FindChild<ScrollViewer>(o);
                        if (null != element)
                        {
                            ScrollEventListenerCollection oldListeners = e.OldValue as ScrollEventListenerCollection;
                            ScrollEventListenerCollection newListeners = e.NewValue as ScrollEventListenerCollection;
                            if (null != oldListeners)
                            {
                                oldListeners.Target = null;
                            }
                            if (null != newListeners)
                            {
                                newListeners.Target = element;
                            }
                        }
                    };
            }
        }

        /// <summary>
        /// Finds a child element of the provided DependencyObject of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the child object you would like to find.</typeparam>
        /// <param name="element">The parent DependencyObject to search through.</param>
        /// <returns>A child element, of specified type, from the provided DependencyObject.
        ///     Null if no child of specified type is found.</returns>
        internal static T FindChild<T>(DependencyObject element) where T : class
        {
            List<DependencyObject> els = new List<DependencyObject>();
            els.Add(element);
            while (els.Count != 0)
            {
                DependencyObject child = els[0];
                els.RemoveAt(0);

                T typedChild = child as T;
                if (null != typedChild)
                {
                    return typedChild;
                }

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(child); i++)
                {
                    els.Add(VisualTreeHelper.GetChild(child, i));
                }
            }
            return null;
        }
    }
}