﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WinRTXamlToolkit.Controls.DataVisualization.Charting
{
    /// <summary>
    /// A label used to display data in an axis.
    /// </summary>
    public class AxisLabel : Control
    {
        #region public string StringFormat
        /// <summary>
        /// Gets or sets the text string format.
        /// </summary>
        public string StringFormat
        {
            get { return GetValue(StringFormatProperty) as string; }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <summary>
        /// Identifies the StringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(
                "StringFormat",
                typeof(string),
                typeof(AxisLabel),
                new PropertyMetadata(null, OnStringFormatPropertyChanged));

        /// <summary>
        /// StringFormatProperty property changed handler.
        /// </summary>
        /// <param name="d">AxisLabel that changed its StringFormat.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AxisLabel source = (AxisLabel)d;
            string newValue = (string)e.NewValue;
            source.OnStringFormatPropertyChanged(newValue);
        }

        /// <summary>
        /// StringFormatProperty property changed handler.
        /// </summary>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnStringFormatPropertyChanged(string newValue)
        {
            UpdateFormattedContent();
        }
        #endregion public string StringFormat

        #region public string FormattedContent
        /// <summary>
        /// Gets the formatted content property.
        /// </summary>
        public string FormattedContent
        {
            get { return GetValue(FormattedContentProperty) as string; }
            protected set { SetValue(FormattedContentProperty, value); }
        }

        /// <summary>
        /// Identifies the FormattedContent dependency property.
        /// </summary>
        public static readonly DependencyProperty FormattedContentProperty =
            DependencyProperty.Register(
                "FormattedContent",
                typeof(string),
                typeof(AxisLabel),
                new PropertyMetadata(null));
        #endregion public string FormattedContent

        /// <summary>
        /// Instantiates a new instance of the AxisLabel class.
        /// </summary>
        public AxisLabel()
        {
            DefaultStyleKey = typeof(AxisLabel);
            this.SetBinding(FormattedContentProperty, new Binding { Converter = new StringFormatConverter(), ConverterParameter = StringFormat ?? "{0}" });
        }

        /// <summary>
        /// Updates the formatted text.
        /// </summary>
        protected virtual void UpdateFormattedContent()
        {
            this.SetBinding(FormattedContentProperty, new Binding { Converter = new StringFormatConverter(), ConverterParameter = StringFormat ?? "{0}" });
        }
    }
}
