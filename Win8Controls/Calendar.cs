using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.ViewManagement;


namespace Win8Controls
{
    /// <summary>
    /// er
    /// Calendar control for Windows Phone 7
    /// </summary>
    public class Calendar : Control
    {
        private readonly DateTimeFormatInfo _dateTimeFormatInfo;
        private PropertyInfo _dateSourcePropertyInfo;
        private ApplicationViewState _applicationViewState = ApplicationViewState.FullScreenLandscape;

        #region Constructor

        /// <summary>
        /// Create new instance of a calendar
        /// </summary>
        public Calendar()
        {
            DefaultStyleKey = typeof(Calendar);
            var binding = new Binding();
            Loaded += CalendarLoaded;
            SetBinding(PrivateDataContextPropertyProperty, binding);
            WireUpDataSource(DataContext, DataContext);
            _dateTimeFormatInfo = !CultureInfo.CurrentCulture.IsNeutralCulture ?
                CultureInfo.CurrentCulture.DateTimeFormat :
                (new CultureInfo("en-US")).DateTimeFormat;
            SetupDaysOfWeekLabels();
            SizeChanged += OnCalendarSizeChanged;
        }

        void OnCalendarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _applicationViewState = ApplicationView.Value;
            UpdateVisualsBasedOnState();
        }

        private void UpdateVisualsBasedOnState()
        {
            SetupDaysOfWeekLabels();
            SetYearMonthLabel();
            VisualStateManager.GoToState(this, _applicationViewState.ToString(), false);
        }

        private void SetupDaysOfWeekLabels()
        {
            if (_applicationViewState != ApplicationViewState.Snapped)
            {
                Sunday = _dateTimeFormatInfo.DayNames[0];
                Monday = _dateTimeFormatInfo.DayNames[1];
                Tuesday = _dateTimeFormatInfo.DayNames[2];
                Wednesday = _dateTimeFormatInfo.DayNames[3];
                Thursday = _dateTimeFormatInfo.DayNames[4];
                Friday = _dateTimeFormatInfo.DayNames[5];
                Saturday = _dateTimeFormatInfo.DayNames[6];

            }
            else
            {
                Sunday = _dateTimeFormatInfo.AbbreviatedDayNames[0];
                Monday = _dateTimeFormatInfo.AbbreviatedDayNames[1];
                Tuesday = _dateTimeFormatInfo.AbbreviatedDayNames[2];
                Wednesday = _dateTimeFormatInfo.AbbreviatedDayNames[3];
                Thursday = _dateTimeFormatInfo.AbbreviatedDayNames[4];
                Friday = _dateTimeFormatInfo.AbbreviatedDayNames[5];
                Saturday = _dateTimeFormatInfo.AbbreviatedDayNames[6];

            }
        }


        void CalendarLoaded(object sender, RoutedEventArgs e)
        {
            if (EnableGestures)
            {
                EnableGesturesSupport();
            }
        }

        private void EnableGesturesSupport()
        {
            DisableGesturesSupport();
            ManipulationCompleted += CalendarManipulationCompleted;
        }

        void CalendarManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }

        private void DisableGesturesSupport()
        {
            ManipulationCompleted -= CalendarManipulationCompleted;
        }




        #endregion

        #region Gestures

        #endregion

        #region Members

        private Grid _itemsGrid;
        CalendarItem _lastItem;
        private bool _addedItems;
        private int _month = DateTime.Today.Month;
        private int _year = DateTime.Today.Year;

        #endregion

        #region Events

        /// <summary>
        /// Event that occurs before month/year combination is changed
        /// </summary>
        public event EventHandler<MonthChangedEventArgs> MonthChanging;

        /// <summary>
        /// Event that occurs after month/year combination is changed
        /// </summary>
        public event EventHandler<MonthChangedEventArgs> MonthChanged;

        /// <summary>
        /// Event that occurs after a date is selected on the calendar
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Event that occurs after a date is tapped by the user on the calendar
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> DateTapped;


        /// <summary>
        /// Raises MonthChanging event
        /// </summary>
        /// <param name="year">Year for event arguments</param>
        /// <param name="month">Month for event arguments</param>
        protected void OnMonthChanging(int year, int month)
        {
            if (MonthChanging != null)
            {
                MonthChanging(this, new MonthChangedEventArgs(year, month));
            }
        }

        /// <summary>
        /// Raises MonthChanged event
        /// </summary>
        /// <param name="year">Year for event arguments</param>
        /// <param name="month">Month for event arguments</param>
        protected void OnMonthChanged(int year, int month)
        {
            if (MonthChanged != null)
            {
                MonthChanged(this, new MonthChangedEventArgs(year, month));
            }
        }

        /// <summary>
        /// Raises SelectedChanged event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnSelectionChanged(DateTime dateTime)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new SelectionChangedEventArgs(dateTime));
            }
            if (SelectedDateCommand != null)
            {
                if (SelectedDateCommand.CanExecute(dateTime))
                {
                    SelectedDateCommand.Execute(dateTime);
                }
            }
            Refresh();
        }

        /// <summary>
        /// Raises SelectedChanged event
        /// </summary>
        /// <param name="dateTime">Selected date</param>
        protected void OnDateTapped(DateTime dateTime)
        {
            if (DateTapped != null)
            {
                DateTapped(this, new SelectionChangedEventArgs(dateTime));
            }
            if (DateTapppedCommand != null)
            {
                if (DateTapppedCommand.CanExecute(dateTime))
                {
                    DateTapppedCommand.Execute(dateTime);
                }
            }
        }



        #endregion

        #region Constants

        private const short RowCount = 6;
        private const short ColumnCount = 8;

        #endregion

        #region Properties



        internal object PrivateDataContextProperty
        {
            get { return GetValue(PrivateDataContextPropertyProperty); }
            set { SetValue(PrivateDataContextPropertyProperty, value); }
        }

        internal static readonly DependencyProperty PrivateDataContextPropertyProperty =
            DependencyProperty.Register("PrivateDataContextProperty", typeof(object), typeof(Calendar), new PropertyMetadata(null, OnPrivateDataContextChanged));

        private static void OnPrivateDataContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                calendar.WireUpDataSource(e.OldValue, e.NewValue);
                calendar.Refresh();
            }
        }

        private void WireUpDataSource(object oldValue, object newValue)
        {
            if (newValue != null)
            {
                var source = newValue as INotifyPropertyChanged;
                if (source != null)
                {
                    source.PropertyChanged += SourcePropertyChanged;
                }
            }
            if (oldValue != null)
            {
                var source = newValue as INotifyPropertyChanged;
                if (source != null)
                {
                    source.PropertyChanged -= SourcePropertyChanged;
                }
            }
        }


        /// <summary>
        /// Explicitly refresh the calendar
        /// </summary>
        public void Refresh()
        {
            BuildItems();
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var expression = GetBindingExpression(DatesSourceProperty);
            //if (expression != null)
            //{
            //    if (expression.ParentBinding.Path.Path.EndsWith(e.PropertyName))
            //    {
            //        Refresh();
            //    }
            //}
        }

        /// <summary>
        /// Collection of objects containing dates
        /// </summary>
        public IEnumerable DatesSource
        {
            get { return (IEnumerable)GetValue(DatesSourceProperty); }
            set { SetValue(DatesSourceProperty, value); }
        }

        /// <summary>
        /// Collection of objects containing dates
        /// </summary>
        public static readonly DependencyProperty DatesSourceProperty =
            DependencyProperty.Register("DatesSource", typeof(IEnumerable), typeof(Calendar), new PropertyMetadata(null, OnDatesSourceChanged));

        private static void OnDatesSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                calendar.BuildItems();
                if (e.OldValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)e.NewValue).CollectionChanged -= calendar.DatesSourceChanged;
                }
                if (e.NewValue is INotifyCollectionChanged)
                {
                    (e.NewValue as INotifyCollectionChanged).CollectionChanged += calendar.DatesSourceChanged;
                }
            }
        }

        private void DatesSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Property name for each object in DatesSource that contains the date to be evaluating 
        /// when building a calendar
        /// </summary>
        public static readonly DependencyProperty DatePropertyNameForDatesSourceProperty =
            DependencyProperty.Register("DatePropertyNameForDatesSource", typeof(string), typeof(Calendar), new PropertyMetadata(string.Empty, OnDatesSourceChanged));


        /// <summary>
        /// Property name for each object in DatesSource that contains the date to be evaluating 
        /// when building a calendar
        /// </summary>
        /// <value>
        /// Property name for each object in DatesSource that contains the date to be evaluating 
        /// when building a calendar
        /// </value>
        public string DatePropertyNameForDatesSource
        {
            get { return (string)GetValue(DatePropertyNameForDatesSourceProperty); }
            set { SetValue(DatePropertyNameForDatesSourceProperty, value); }
        }


        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public Style CalendarItemStyle
        {
            get { return (Style)GetValue(CalendarItemStyleProperty); }
            set { SetValue(CalendarItemStyleProperty, value); }
        }

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public static readonly DependencyProperty CalendarItemStyleProperty =
            DependencyProperty.Register("CalendarItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public Style CalendarWeekItemStyle
        {
            get { return (Style)GetValue(CalendarWeekItemStyleStyleProperty); }
            set { SetValue(CalendarWeekItemStyleStyleProperty, value); }
        }

        /// <summary>
        /// Style for the calendar item
        /// </summary>
        public static readonly DependencyProperty CalendarWeekItemStyleStyleProperty =
            DependencyProperty.Register("CalendarWeekItemStyle", typeof(Style), typeof(Calendar), new PropertyMetadata(null));

        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public string YearMonthLabel
        {
            get { return (string)GetValue(YearMonthLabelProperty); }
            internal set { SetValue(YearMonthLabelProperty, value); }
        }

        /// <summary>
        /// This value is shown in calendar header and includes month and year
        /// </summary>
        public static readonly DependencyProperty YearMonthLabelProperty =
            DependencyProperty.Register("YearMonthLabel", typeof(string), typeof(Calendar), new PropertyMetadata(""));

        /// <summary>
        /// This value currently selected date on the calendar
        /// This property can be bound to
        /// </summary>
        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// This value currently selected date on the calendar
        /// This property can be bound to
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.MinValue, OnSelectedDateChanged));

        private static void OnSelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null)
            {
                calendar.OnSelectionChanged((DateTime)e.NewValue);
                calendar.SelectedMonth = ((DateTime)(e.NewValue)).Month;
                calendar.SelectedYear = ((DateTime)(e.NewValue)).Year;
            }
            
        }


        public double HeaderFontSize
        {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }
        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(Calendar), new PropertyMetadata(14));


        public string Sunday
        {
            get { return (string)GetValue(SundayProperty); }
            set { SetValue(SundayProperty, value); }
        }
        public static readonly DependencyProperty SundayProperty =
            DependencyProperty.Register("Sunday", typeof(string), typeof(Calendar), new PropertyMetadata("Sunday"));


        public string Monday
        {
            get { return (string)GetValue(MondayProperty); }
            set { SetValue(MondayProperty, value); }
        }
        public static readonly DependencyProperty MondayProperty =
            DependencyProperty.Register("Monday", typeof(string), typeof(Calendar), new PropertyMetadata("Monday"));


        public string Tuesday
        {
            get { return (string)GetValue(TuesdayProperty); }
            set { SetValue(TuesdayProperty, value); }
        }
        public static readonly DependencyProperty TuesdayProperty =
            DependencyProperty.Register("Tuesday", typeof(string), typeof(Calendar), new PropertyMetadata("Tuesday"));


        public string Wednesday
        {
            get { return (string)GetValue(WednesdayProperty); }
            set { SetValue(WednesdayProperty, value); }
        }
        public static readonly DependencyProperty WednesdayProperty =
            DependencyProperty.Register("Wednesday", typeof(string), typeof(Calendar), new PropertyMetadata("Wednesday"));


        public string Thursday
        {
            get { return (string)GetValue(ThursdayProperty); }
            set { SetValue(ThursdayProperty, value); }
        }
        public static readonly DependencyProperty ThursdayProperty =
            DependencyProperty.Register("Thursday", typeof(string), typeof(Calendar), new PropertyMetadata("Thursday"));


        public string Friday
        {
            get { return (string)GetValue(FridayProperty); }
            set { SetValue(FridayProperty, value); }
        }
        public static readonly DependencyProperty FridayProperty =
            DependencyProperty.Register("Friday", typeof(string), typeof(Calendar), new PropertyMetadata("Friday"));


        public string Saturday
        {
            get { return (string)GetValue(SaturdayProperty); }
            set { SetValue(SaturdayProperty, value); }
        }
        public static readonly DependencyProperty SaturdayProperty =
            DependencyProperty.Register("Saturday", typeof(string), typeof(Calendar), new PropertyMetadata("Saturday"));


        /// <summary>
        /// Currently selected year
        /// </summary>
        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        /// <summary>
        /// Currently selected year
        /// </summary>
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Year, OnSelectedYearMonthChanged));

        private static void OnSelectedYearMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar != null && (calendar._year != calendar.SelectedYear || calendar._month != calendar.SelectedMonth))
            {
                if (!calendar._ignoreMonthChange)
                {
                    calendar._year = calendar.SelectedYear;
                    calendar._month = calendar.SelectedMonth;
                    calendar.SetYearMonthLabel();
                }
            }
        }


        /// <summary>
        /// Currently selected month
        /// </summary>
        public int SelectedMonth
        {
            get { return (int)GetValue(SelectedMonthProperty); }
            set { SetValue(SelectedMonthProperty, value); }
        }

        /// <summary>
        /// Currently selected month
        /// </summary>
        public static readonly DependencyProperty SelectedMonthProperty =
            DependencyProperty.Register("SelectedMonth", typeof(int), typeof(Calendar), new PropertyMetadata(DateTime.Today.Month, OnSelectedYearMonthChanged));


        /// <summary>
        /// If true, previous and next month buttons are shown
        /// </summary>
        public bool ShowNavigationButtons
        {
            get { return (bool)GetValue(ShowNavigationButtonsProperty); }
            set { SetValue(ShowNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// If true, previous and next month buttons are shown
        /// </summary>
        public static readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register("ShowNavigationButtons", typeof(bool), typeof(Calendar), new PropertyMetadata(true));

        /// <summary>
        /// If true, gesture support is enabled
        /// </summary>
        public bool EnableGestures
        {
            get { return (bool)GetValue(EnableGesturesProperty); }
            set { SetValue(EnableGesturesProperty, value); }
        }

        /// <summary>
        /// If true, gesture support is enabled
        /// </summary>
        public static readonly DependencyProperty EnableGesturesProperty =
            DependencyProperty.Register("EnableGestures", typeof(bool), typeof(Calendar), new PropertyMetadata(false, OnEnableGesturesChanged));

        /// <summary>
        /// Handle changes to gesture support
        /// </summary>
        /// <param name="sender">Calendar control</param>
        /// <param name="e">Event arguments</param>
        public static void OnEnableGesturesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = (Calendar)sender;
            if (target.EnableGestures)
            {
                target.EnableGesturesSupport();
            }
            else
            {
                target.DisableGesturesSupport();
            }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public bool ShowSelectedDate
        {
            get { return (bool)GetValue(ShowSelectedDateProperty); }
            set { SetValue(ShowSelectedDateProperty, value); }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public static readonly DependencyProperty ShowSelectedDateProperty =
            DependencyProperty.Register("ShowSelectedDate", typeof(bool), typeof(Calendar), new PropertyMetadata(true));


        /// <summary>
        /// Sets an option of how to display week number
        /// </summary>
        public WeekNumberDisplayOption WeekNumberDisplay
        {
            get { return (WeekNumberDisplayOption)GetValue(WeekNumberDisplayProperty); }
            set { SetValue(WeekNumberDisplayProperty, value); }
        }

        /// <summary>
        /// If set to false, selected date is not highlighted
        /// </summary>
        public static readonly DependencyProperty WeekNumberDisplayProperty =
            DependencyProperty.Register("WeekNumberDisplay", typeof(WeekNumberDisplayOption), typeof(Calendar),
            new PropertyMetadata(WeekNumberDisplayOption.None, OnWeekNumberDisplayChanged));

        /// <summary>
        /// Update calendar display when display option changes
        /// </summary>
        /// <param name="sender">Calendar control</param>
        /// <param name="e">Event arguments</param>
        public static void OnWeekNumberDisplayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Calendar)sender).BuildItems();
        }





        /// <summary>
        /// Gets or sets the selected date command.
        /// </summary>
        /// <value>
        /// The selected date command.
        /// </value>
        public ICommand SelectedDateCommand
        {
            get { return (ICommand)GetValue(SelectedDateCommandProperty); }
            set { SetValue(SelectedDateCommandProperty, value); }
        }


        /// <summary>
        /// The selected date command property
        /// </summary>
        public static readonly DependencyProperty SelectedDateCommandProperty =
            DependencyProperty.Register("SelectedDateCommand", typeof(ICommand), typeof(Calendar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the tapped date command.
        /// </summary>
        /// <value>
        /// The tapped date command.
        /// </value>
        public ICommand DateTapppedCommand
        {
            get { return (ICommand)GetValue(DateTappedCommandProperty); }
            set { SetValue(DateTappedCommandProperty, value); }
        }


        /// <summary>
        /// The selected date command property
        /// </summary>
        public static readonly DependencyProperty DateTappedCommandProperty =
            DependencyProperty.Register("DateTapppedCommand", typeof(ICommand), typeof(Calendar), new PropertyMetadata(null));



        #endregion

        #region Template

        /// <summary>
        /// Apply default template and perform initialization
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var previousButton = GetTemplateChild("PreviousMonthButton") as Button;
            if (previousButton != null) previousButton.Click += PreviousButtonClick;
            var nextButton = GetTemplateChild("NextMonthButton") as Button;
            if (nextButton != null) nextButton.Click += NextButtonClick;
            _itemsGrid = GetTemplateChild("ItemsGrid") as Grid;
            SetYearMonthLabel();
        }

        #endregion

        #region Event handling

        void NextButtonClick(object sender, RoutedEventArgs e)
        {
            IncrementMonth();
        }

        private void IncrementMonth()
        {
            if (_year != 2499 || _month != 12)
            {
                _month += 1;
                if (_month == 13)
                {
                    _month = 1;
                    _year += 1;
                }
                SetYearMonthLabel();
            }
        }

        void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            DecrementMonth();
        }

        private void DecrementMonth()
        {
            if (_year != 1753 || _month != 1)
            {
                _month -= 1;
                if (_month == 0)
                {
                    _month = 12;
                    _year -= 1;
                }
                SetYearMonthLabel();
            }
        }

        //private void IncrementYear()
        //{
        //    if (_year != 2499)
        //    {
        //        _year += 1;
        //        SetYearMonthLabel();
        //    }
        //}

        //private void DecrementYear()
        //{
        //    if (_year != 1753)
        //    {
        //        _year -= 1;
        //        SetYearMonthLabel();
        //    }
        //}


        private void ProcessSelectedItem(object sender)
        {
            if (_lastItem != null)
            {
                _lastItem.IsSelected = false;
            }
            _lastItem = (sender as CalendarItem);
            if (_lastItem != null)
            {
                if (ShowSelectedDate)
                    _lastItem.IsSelected = true;
                SelectedDate = _lastItem.ItemDate;
            }
        }

        #endregion

        #region Methods
        private bool _ignoreMonthChange;
        private void SetYearMonthLabel()
        {
            OnMonthChanging(_year, _month);
            YearMonthLabel = string.Concat(GetMonthName(), " ", _year.ToString());
            _ignoreMonthChange = true;
            SelectedMonth = _month;
            SelectedYear = _year;
            _ignoreMonthChange = false;
            BuildItems();
            OnMonthChanged(_year, _month);
        }

        private string GetMonthName()
        {
            if(_applicationViewState == ApplicationViewState.Snapped)
            {
                return _dateTimeFormatInfo.AbbreviatedMonthNames[_month - 1];
            }
            return _dateTimeFormatInfo.MonthNames[_month - 1];
        }

        private void BuildItems()
        {
            if (_itemsGrid != null)
            {
                AddDefaultItems();
                var startOfMonth = new DateTime(_year, _month, 1);
                DayOfWeek dayOfWeek = startOfMonth.DayOfWeek;
                var daysInMonth = (int)Math.Floor(startOfMonth.AddMonths(1).Subtract(startOfMonth).TotalDays);
                var addedDays = 0;
                int lastWeekNumber = 0;
                for (int rowCount = 1; rowCount <= RowCount; rowCount++)
                {
                    for (var columnCount = 1; columnCount < ColumnCount; columnCount++)
                    {
                        int rowCounter = rowCount;
                        int columnCounter = columnCount;
                        var item = (CalendarItem)(from oneChild in _itemsGrid.Children
                                                  where oneChild is CalendarItem &&
                                                  ((CalendarItem)oneChild).Tag.ToString() == string.Concat(rowCounter.ToString(), ":", columnCounter.ToString())
                                                  select oneChild).FirstOrDefault();
                        if (item != null)
                        {
                            if (rowCount == 1 && columnCount < (int) dayOfWeek + 1)
                            {
                                item.Visibility = Visibility.Collapsed;
                            }
                            else if (addedDays < daysInMonth)
                            {
                                item.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                item.Visibility = Visibility.Collapsed;
                            }

                            int count = rowCount;
                            var weekItem = (CalendarWeekItem) (from oneChild in _itemsGrid.Children
                                                               where oneChild is CalendarWeekItem &&
                                                                     ((CalendarWeekItem) oneChild).Tag.ToString() ==
                                                                     string.Concat(count.ToString(), ":0")
                                                               select oneChild).FirstOrDefault();

                            if (item.Visibility == Visibility.Visible)
                            {
                                item.ItemDate = startOfMonth.AddDays(addedDays);
                                if (SelectedDate == DateTime.MinValue && item.ItemDate == DateTime.Today)
                                {
                                    SelectedDate = item.ItemDate;
                                    if (ShowSelectedDate)
                                        item.IsSelected = true;
                                    _lastItem = item;
                                }
                                else
                                {
                                    if (item.ItemDate == SelectedDate)
                                    {
                                        if (ShowSelectedDate)
                                            item.IsSelected = true;
                                    }
                                    else
                                    {
                                        item.IsSelected = false;
                                    }
                                }
                                addedDays += 1;
                                item.DayNumber = addedDays;
                                item.DataContext = FindItem(item.ItemDate);

                                if (WeekNumberDisplay != WeekNumberDisplayOption.None)
                                {
                                    int weekNumber;
                                    if (WeekNumberDisplay == WeekNumberDisplayOption.WeekOfYear)
                                    {
                                        var systemCalendar = CultureInfo.CurrentCulture.Calendar;
                                        weekNumber = systemCalendar.GetWeekOfYear(
                                            item.ItemDate,
                                            CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
                                            CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
                                    }
                                    else
                                    {
                                        weekNumber = rowCount;
                                    }
                                    weekItem.WeekNumber = weekNumber;
                                    lastWeekNumber = weekNumber;
                                    weekItem.Visibility = Visibility.Visible;
                                }
                            }
                            else
                            {
                                if (WeekNumberDisplay != WeekNumberDisplayOption.None &&
                                    weekItem.WeekNumber != lastWeekNumber)
                                {
                                    weekItem.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddDefaultItems()
        {
            if (!_addedItems && _itemsGrid != null)
            {
                for (int rowCount = 1; rowCount <= RowCount; rowCount++)
                {
                    for (int columnCount = 1; columnCount < ColumnCount; columnCount++)
                    {
                        var item = new CalendarItem(this);
                        item.SetValue(Grid.RowProperty, rowCount);
                        item.SetValue(Grid.ColumnProperty, columnCount);
                        item.Visibility = Visibility.Collapsed;
                        item.Tag = string.Concat(rowCount.ToString(), ":", columnCount.ToString());
                        item.Tapped += ItemTapped;
                        if (CalendarItemStyle != null)
                        {
                            item.Style = CalendarItemStyle;
                        }
                        _itemsGrid.Children.Add(item);
                    }
                    if (WeekNumberDisplay != WeekNumberDisplayOption.None)
                    {
                        const int columnCount = 0;
                        var item = new CalendarWeekItem();
                        item.SetValue(Grid.RowProperty, rowCount);
                        item.SetValue(Grid.ColumnProperty, columnCount);
                        item.Visibility = Visibility.Collapsed;
                        item.Tag = string.Concat(rowCount.ToString(), ":", columnCount.ToString());
                        if (CalendarWeekItemStyle != null)
                        {
                            item.Style = CalendarWeekItemStyle;
                        }
                        _itemsGrid.Children.Add(item);
                    }
                }
                _addedItems = true;
            }
        }

        private void ItemTapped(object sender, TappedRoutedEventArgs e)
        {
            ProcessSelectedItem(sender);
            OnDateTapped(SelectedDate);
        }


        #endregion

        private object FindItem(DateTime dateTime)
        {
            if (DatesSource != null)
            {
                foreach (var item in DatesSource)
                {
                    if (_dateSourcePropertyInfo == null)
                    {
                        _dateSourcePropertyInfo = item.GetType().GetTypeInfo().GetDeclaredProperty(DatePropertyNameForDatesSource);
                    }
                    if (dateTime.Equals(_dateSourcePropertyInfo.GetValue(item, null)))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

    }
}
