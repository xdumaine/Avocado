using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Win8Controls
{
    /// <summary>
    /// This class corresponds to a calendar item / cell
    /// </summary>
    public class CalendarItem : Button
    {
        #region Members

        readonly Calendar _owningCalendar;
        private ApplicationViewState _applicationViewState = ApplicationViewState.FullScreenLandscape;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new instance of a calendar cell
        /// </summary>
        [Obsolete("Internal use only")]
        public CalendarItem()
        {
            DefaultStyleKey = typeof(CalendarItem);
        }


        /// <summary>
        /// Create new instance of a calendar cell
        /// </summary>
        /// <param name="owner">Calenda control that a cell belongs to</param>
        public CalendarItem(Calendar owner)
        {
            DefaultStyleKey = typeof(CalendarItem);
            _owningCalendar = owner;
            _owningCalendar.SizeChanged += OwningCalendarSizeChanged;
        }

        void OwningCalendarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _applicationViewState = ApplicationView.Value;
            VisualStateManager.GoToState(this, _applicationViewState.ToString(), false);    
        }

        #endregion

        #region Properties

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public int DayNumber
        {
            get { return (int)GetValue(DayNumberProperty); }
            internal set { SetValue(DayNumberProperty, value); }
        }

        /// <summary>
        /// Day number for this calendar cell.
        /// This changes depending on the month shown
        /// </summary>
        public static readonly DependencyProperty DayNumberProperty =
            DependencyProperty.Register("DayNumber", typeof(int), typeof(CalendarItem), new PropertyMetadata(0, OnDayNumberChanged));

        private static void OnDayNumberChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {

        }


        internal bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        internal static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CalendarItem), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Date for the calendar item
        /// </summary>
        public DateTime ItemDate
        {
            get { return (DateTime)GetValue(ItemDateProperty); }
            internal set { SetValue(ItemDateProperty, value); }
        }

        /// <summary>
        /// Date for the calendar item
        /// </summary>
        internal static readonly DependencyProperty ItemDateProperty =
            DependencyProperty.Register("ItemDate", typeof(DateTime), typeof(CalendarItem), new PropertyMetadata(null));

        #endregion

    }

}
