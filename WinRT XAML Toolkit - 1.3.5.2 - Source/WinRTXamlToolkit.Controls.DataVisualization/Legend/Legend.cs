﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WinRTXamlToolkit.Controls.DataVisualization
{
    /// <summary>
    /// Represents a control that displays a list of items and has a title.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    public partial class Legend : HeaderedItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the Legend class.
        /// </summary>
        public Legend()
        {
            DefaultStyleKey = typeof(Legend);
            // By default, the Visibility property should follow ContentVisibility - but users can override it
            SetBinding(VisibilityProperty, new Binding {Path = new PropertyPath("ContentVisibility"), Source = this });
        }

        /// <summary>
        /// Gets or sets the Style of the ISeriesHost's Title.
        /// </summary>
        public Style TitleStyle
        {
            get { return GetValue(TitleStyleProperty) as Style; }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TitleStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register(
                "TitleStyle",
                typeof(Style),
                typeof(Legend),
                new PropertyMetadata(default(Style)));

        /// <summary>
        /// Gets the Visibility of the Legend's content (title and items).
        /// </summary>
        public Visibility ContentVisibility
        {
            get { return (Visibility)GetValue(ContentVisibilityProperty); }
            protected set { SetValue(ContentVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentVisibilityProperty =
            DependencyProperty.Register(
                "ContentVisibility",
                typeof(Visibility),
                typeof(Legend),
                new PropertyMetadata(default(Visibility)));

        /// <summary>
        /// Handles the OnHeaderChanged event for HeaderedItemsControl.
        /// </summary>
        /// <param name="oldHeader">Old header.</param>
        /// <param name="newHeader">New header.</param>
        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            UpdateContentVisibility();
        }

        /// <summary>
        /// Handles the CollectionChanged event for HeaderedItemsControl's ItemsSource.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            UpdateContentVisibility();
        }

        /// <summary>
        /// Updates the ContentVisibility property to reflect the presence of content.
        /// </summary>
        private void UpdateContentVisibility()
        {
            ContentVisibility = (Header != null || Items.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
