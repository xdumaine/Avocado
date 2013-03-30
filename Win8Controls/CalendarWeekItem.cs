using System;
using System.Net;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Win8Controls
{
    /// <summary>
    /// Class representing week number cell
    /// </summary>
    public class CalendarWeekItem : Control
    {
        #region Constructor

        /// <summary>
        /// Create new instance of a calendar week number cell
        /// </summary>
        public CalendarWeekItem()
        {
            DefaultStyleKey = typeof(CalendarWeekItem);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public int? WeekNumber
        {
            get { return (int)GetValue(WeekNumberProperty); }
            internal set { SetValue(WeekNumberProperty, value); }
        }

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public static readonly DependencyProperty WeekNumberProperty =
            DependencyProperty.Register("WeekNumber", typeof(int), typeof(CalendarWeekItem), new PropertyMetadata(null));

        #endregion
    }
}
