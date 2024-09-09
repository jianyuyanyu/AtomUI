﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Utilities;

namespace AtomUI.Controls;

[TemplatePart(RangeCalendarTheme.CalendarItemPart, typeof(RangeCalendarItem))]
[TemplatePart(RangeCalendarTheme.RootPart, typeof(Panel))]
public class RangeCalendar : TemplatedControl
{
   internal const int RowsPerMonth = 7;
   internal const int ColumnsPerMonth = 7;
   internal const int RowsPerYear = 3;
   internal const int ColumnsPerYear = 4;

   #region 公共属性定义

   public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
      AvaloniaProperty.Register<RangeCalendar, DayOfWeek>(
         nameof(FirstDayOfWeek),
         defaultValue: DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek);

   /// <summary>
   /// Gets or sets the day that is considered the beginning of the week.
   /// </summary>
   /// <value>
   /// A <see cref="T:System.DayOfWeek" /> representing the beginning of
   /// the week. The default is <see cref="F:System.DayOfWeek.Sunday" />.
   /// </value>
   public DayOfWeek FirstDayOfWeek
   {
      get => GetValue(FirstDayOfWeekProperty);
      set => SetValue(FirstDayOfWeekProperty, value);
   }

   public static readonly StyledProperty<bool> IsTodayHighlightedProperty =
      AvaloniaProperty.Register<RangeCalendar, bool>(
         nameof(IsTodayHighlighted),
         defaultValue: true);

   /// <summary>
   /// Gets or sets a value indicating whether the current date is
   /// highlighted.
   /// </summary>
   /// <value>
   /// True if the current date is highlighted; otherwise, false. The
   /// default is true.
   /// </value>
   public bool IsTodayHighlighted
   {
      get => GetValue(IsTodayHighlightedProperty);
      set => SetValue(IsTodayHighlightedProperty, value);
   }

   public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
      AvaloniaProperty.Register<RangeCalendar, IBrush?>(nameof(HeaderBackground));

   public IBrush? HeaderBackground
   {
      get => GetValue(HeaderBackgroundProperty);
      set => SetValue(HeaderBackgroundProperty, value);
   }

   public static readonly StyledProperty<CalendarMode> DisplayModeProperty =
      AvaloniaProperty.Register<RangeCalendar, CalendarMode>(
         nameof(DisplayMode),
         validate: IsValidDisplayMode);

   /// <summary>
   /// Gets or sets a value indicating whether the calendar is displayed in
   /// months, years, or decades.
   /// </summary>
   /// <value>
   /// A value indicating what length of time the
   /// <see cref="T:System.Windows.Controls.RangeCalendar" /> should display.
   /// </value>
   public CalendarMode DisplayMode
   {
      get => GetValue(DisplayModeProperty);
      set => SetValue(DisplayModeProperty, value);
   }
   
   public static readonly StyledProperty<DateTime?> SelectedRangeStartDateProperty =
      AvaloniaProperty.Register<RangeCalendar, DateTime?>(nameof(SelectedRangeStartDate),
                                                     defaultBindingMode: BindingMode.TwoWay);
   
   public DateTime? SelectedRangeStartDate
   {
      get => GetValue(SelectedRangeStartDateProperty);
      set => SetValue(SelectedRangeStartDateProperty, value);
   }
   
   public static readonly StyledProperty<DateTime?> SelectedRangeEndDateProperty =
      AvaloniaProperty.Register<RangeCalendar, DateTime?>(nameof(SelectedRangeEndDate),
                                                          defaultBindingMode: BindingMode.TwoWay);
   
   public DateTime? SelectedRangeEndDate
   {
      get => GetValue(SelectedRangeEndDateProperty);
      set => SetValue(SelectedRangeEndDateProperty, value);
   }

   public static readonly StyledProperty<DateTime> DisplayDateProperty =
      AvaloniaProperty.Register<RangeCalendar, DateTime>(nameof(DisplayDate),
                                                    defaultBindingMode: BindingMode.TwoWay);

   /// <summary>
   /// Gets or sets the date to display.
   /// </summary>
   /// <value>The date to display.</value>
   /// <exception cref="T:System.ArgumentOutOfRangeException">
   /// The given date is not in the range specified by
   /// <see cref="P:System.Windows.Controls.RangeCalendar.DisplayDateStart" />
   /// and
   /// <see cref="P:System.Windows.Controls.RangeCalendar.DisplayDateEnd" />.
   /// </exception>
   /// <remarks>
   /// <para>
   /// This property allows the developer to specify a date to display.  If
   /// this property is a null reference (Nothing in Visual Basic),
   /// SelectedDate is displayed.  If SelectedDate is also a null reference
   /// (Nothing in Visual Basic), Today is displayed.  The default is
   /// Today.
   /// </para>
   /// <para>
   /// To set this property in XAML, use a date specified in the format
   /// yyyy/mm/dd.  The mm and dd components must always consist of two
   /// characters, with a leading zero if necessary.  For instance, the
   /// month of May should be specified as 05.
   /// </para>
   /// </remarks>
   public DateTime DisplayDate
   {
      get => GetValue(DisplayDateProperty);
      set => SetValue(DisplayDateProperty, value);
   }

   public static readonly StyledProperty<DateTime?> DisplayDateStartProperty =
      AvaloniaProperty.Register<RangeCalendar, DateTime?>(nameof(DisplayDateStart),
                                                     defaultBindingMode: BindingMode.TwoWay);

   /// <summary>
   /// Gets or sets the first date to be displayed.
   /// </summary>
   /// <value>The first date to display.</value>
   /// <remarks>
   /// To set this property in XAML, use a date specified in the format
   /// yyyy/mm/dd.  The mm and dd components must always consist of two
   /// characters, with a leading zero if necessary.  For instance, the
   /// month of May should be specified as 05.
   /// </remarks>
   public DateTime? DisplayDateStart
   {
      get => GetValue(DisplayDateStartProperty);
      set => SetValue(DisplayDateStartProperty, value);
   }

   public static readonly StyledProperty<DateTime?> DisplayDateEndProperty =
      AvaloniaProperty.Register<RangeCalendar, DateTime?>(nameof(DisplayDateEnd),
                                                     defaultBindingMode: BindingMode.TwoWay);

   /// <summary>
   /// Gets or sets the last date to be displayed.
   /// </summary>
   /// <value>The last date to display.</value>
   /// <remarks>
   /// To set this property in XAML, use a date specified in the format
   /// yyyy/mm/dd.  The mm and dd components must always consist of two
   /// characters, with a leading zero if necessary.  For instance, the
   /// month of May should be specified as 05.
   /// </remarks>
   public DateTime? DisplayDateEnd
   {
      get => GetValue(DisplayDateEndProperty);
      set => SetValue(DisplayDateEndProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal RangeCalendarDayButton? FocusButton { get; set; }
   internal RangeCalendarButton? FocusCalendarButton { get; set; }

   internal Panel? Root { get; set; }

   internal RangeCalendarItem? MonthControl
   {
      get
      {
         if (Root != null && Root.Children.Count > 0) {
            return Root.Children[0] as RangeCalendarItem;
         }

         return null;
      }
   }

   internal Collection<DateTime> RemovedItems { get; set; }
   internal DateTime? LastSelectedDateInternal { get; set; }

   internal DateTime? LastSelectedDate
   {
      get => LastSelectedDateInternal;
      set
      {
         LastSelectedDateInternal = value;

         if (FocusButton != null) {
            FocusButton.IsCurrent = false;
         }

         FocusButton = FindDayButtonFromDay(LastSelectedDate!.Value);
         if (FocusButton != null) {
            FocusButton.IsCurrent = HasFocusInternal;
         }
      }
   }

   internal DateTime SelectedMonth
   {
      get => _selectedMonth;
      set
      {
         int monthDifferenceStart = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeStart);
         int monthDifferenceEnd = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeEnd);

         if (monthDifferenceStart >= 0 && monthDifferenceEnd <= 0) {
            _selectedMonth = DateTimeHelper.DiscardDayTime(value);
         } else {
            if (monthDifferenceStart < 0) {
               _selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
            } else {
               Debug.Assert(monthDifferenceEnd > 0, "monthDifferenceEnd should be greater than 0!");
               _selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
            }
         }
      }
   }

   internal DateTime SelectedYear
   {
      get => _selectedYear;
      set
      {
         if (value.Year < DisplayDateRangeStart.Year) {
            _selectedYear = DisplayDateRangeStart;
         } else {
            if (value.Year > DisplayDateRangeEnd.Year) {
               _selectedYear = DisplayDateRangeEnd;
            } else {
               _selectedYear = value;
            }
         }
      }
   }

   internal DateTime DisplayDateInternal { get; set; }
   internal DateTime SecondaryDisplayDateInternal { get; set; }

   #endregion

   private DateTime _selectedMonth;
   private DateTime _selectedYear;

   private bool _isShiftPressed;
   private bool _displayDateIsChanging;

   /// <summary>
   /// FirstDayOfWeekProperty property changed handler.
   /// </summary>
   /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
   private void OnFirstDayOfWeekChanged(AvaloniaPropertyChangedEventArgs e)
   {
      if (IsValidFirstDayOfWeek(e.NewValue!)) {
         UpdateMonths();
      } else {
         throw new ArgumentOutOfRangeException(nameof(e), "Invalid DayOfWeek");
      }
   }

   /// <summary>
   /// Inherited code: Requires comment.
   /// </summary>
   /// <param name="value">Inherited code: Requires comment 1.</param>
   /// <returns>Inherited code: Requires comment 2.</returns>
   private static bool IsValidFirstDayOfWeek(object value)
   {
      DayOfWeek day = (DayOfWeek)value;

      return day == DayOfWeek.Sunday
             || day == DayOfWeek.Monday
             || day == DayOfWeek.Tuesday
             || day == DayOfWeek.Wednesday
             || day == DayOfWeek.Thursday
             || day == DayOfWeek.Friday
             || day == DayOfWeek.Saturday;
   }

   /// <summary>
   /// IsTodayHighlightedProperty property changed handler.
   /// </summary>
   /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
   private void OnIsTodayHighlightedChanged(AvaloniaPropertyChangedEventArgs e)
   {
      int i = DateTimeHelper.CompareYearMonth(DisplayDateInternal, DateTime.Today);

      if (i > -2 && i < 2) {
         UpdateMonths();
      }
   }

   /// <summary>
   /// DisplayModeProperty property changed handler.
   /// </summary>
   /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
   private void OnDisplayModePropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      CalendarMode mode = (CalendarMode)e.NewValue!;
      CalendarMode oldMode = (CalendarMode)e.OldValue!;
      RangeCalendarItem? monthControl = MonthControl;

      if (monthControl != null) {
         switch (oldMode) {
            case CalendarMode.Month:
            {
               SelectedYear = DisplayDateInternal;
               SelectedMonth = DisplayDateInternal;
               break;
            }
            case CalendarMode.Year:
            {
               SetCurrentValue(DisplayDateProperty, SelectedMonth);
               SelectedYear = SelectedMonth;
               break;
            }
            case CalendarMode.Decade:
            {
               SetCurrentValue(DisplayDateProperty, SelectedYear);
               SelectedMonth = SelectedYear;
               break;
            }
         }

         switch (mode) {
            case CalendarMode.Month:
            {
               OnMonthClick();
               break;
            }
            case CalendarMode.Year:
            case CalendarMode.Decade:
            {
               OnHeaderClick();
               break;
            }
         }
      }

      OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode)e.OldValue, mode));
   }

   private static bool IsValidDisplayMode(CalendarMode mode)
   {
      return mode == CalendarMode.Month
             || mode == CalendarMode.Year
             || mode == CalendarMode.Decade;
   }

   private void OnDisplayModeChanged(CalendarModeChangedEventArgs args)
   {
      DisplayModeChanged?.Invoke(this, args);
   }
   
   /// <summary>
   /// Inherited code: Requires comment.
   /// </summary>
   /// <param name="value">Inherited code: Requires comment 1.</param>
   /// <returns>Inherited code: Requires comment 2.</returns>
   private static bool IsValidSelectionMode(object value)
   {
      CalendarSelectionMode mode = (CalendarSelectionMode)value;

      return mode == CalendarSelectionMode.SingleDate
             || mode == CalendarSelectionMode.SingleRange
             || mode == CalendarSelectionMode.MultipleRange
             || mode == CalendarSelectionMode.None;
   }

   private void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs e)
   {
      // if (!_displayDateIsChanging) {
      //    DateTime? addedDate;
      //
      //    addedDate = (DateTime?)e.NewValue;
      //
      //    if (IsValidDateSelection(this, addedDate)) {
      //       if (addedDate == null) {
      //          SelectedDates.Clear();
      //       } else {
      //          if (!(SelectedDates.Count > 0 && SelectedDates[0] == addedDate.Value)) {
      //             foreach (DateTime item in SelectedDates) {
      //                RemovedItems.Add(item);
      //             }
      //
      //             SelectedDates.ClearInternal();
      //             // the value is added as a range so that the
      //             // SelectedDatesChanged event can be thrown with
      //             // all the removed items
      //             SelectedDates.AddRange(addedDate.Value, addedDate.Value);
      //          }
      //       }
      //       
      //    } else {
      //       throw new ArgumentOutOfRangeException(nameof(e), "SelectedDate value is not valid.");
      //    }
      // }
   }

   private static bool IsSelectionChanged(SelectionChangedEventArgs e)
   {
      if (e.AddedItems.Count != e.RemovedItems.Count) {
         return true;
      }

      foreach (DateTime addedDate in e.AddedItems) {
         if (!e.RemovedItems.Contains(addedDate)) {
            return true;
         }
      }

      return false;
   }

   internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
   {
      if (IsSelectionChanged(e)) {
         e.RoutedEvent = SelectingItemsControl.SelectionChangedEvent;
         e.Source = this;
         SelectedDatesChanged?.Invoke(this, e);
      }
   }

   static RangeCalendar()
   {
      IsEnabledProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnIsEnabledChanged(e));
      FirstDayOfWeekProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnFirstDayOfWeekChanged(e));
      IsTodayHighlightedProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnIsTodayHighlightedChanged(e));
      DisplayModeProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnDisplayModePropertyChanged(e));
      // SelectedDateProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnSelectedDateChanged(e));
      DisplayDateProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnDisplayDateChanged(e));
      DisplayDateStartProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnDisplayDateStartChanged(e));
      DisplayDateEndProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnDisplayDateEndChanged(e));
      KeyDownEvent.AddClassHandler<RangeCalendar>((x, e) => x.HandleCalendarKeyDown(e));
      HorizontalAlignmentProperty.OverrideDefaultValue<RangeCalendar>(HorizontalAlignment.Left);
      VerticalAlignmentProperty.OverrideDefaultValue<RangeCalendar>(VerticalAlignment.Top);
   }

   /// <summary>
   /// Initializes a new instance of the
   /// <see cref="T:System.Windows.Controls.RangeCalendar" /> class.
   /// </summary>
   public RangeCalendar()
   {
      SetCurrentValue(DisplayDateProperty, DateTime.Today);
      UpdateDisplayDate(this, DisplayDate, DateTime.MinValue);
      BlackoutDates = new RangeCalendarBlackoutDatesCollection(this);
      RemovedItems = new Collection<DateTime>();
   }

   protected virtual void OnDisplayDateChanged(AvaloniaPropertyChangedEventArgs e)
   {
      UpdateDisplayDate(this, (DateTime)e.NewValue!, (DateTime)e.OldValue!);
   }

   private static void UpdateDisplayDate(RangeCalendar c, DateTime addedDate, DateTime removedDate)
   {
      _ = c ?? throw new ArgumentNullException(nameof(c));

      // If DisplayDate < DisplayDateStart, DisplayDate = DisplayDateStart
      if (DateTime.Compare(addedDate, c.DisplayDateRangeStart) < 0) {
         c.DisplayDate = c.DisplayDateRangeStart;
         return;
      }

      // If DisplayDate > DisplayDateEnd, DisplayDate = DisplayDateEnd
      if (DateTime.Compare(addedDate, c.DisplayDateRangeEnd) > 0) {
         c.DisplayDate = c.DisplayDateRangeEnd;
         return;
      }

      c.DisplayDateInternal = DateTimeHelper.DiscardDayTime(addedDate);
      c.SecondaryDisplayDateInternal = DateTimeHelper.AddMonths(c.DisplayDateInternal, 1) ?? c.DisplayDateInternal;
      
      c.UpdateMonths();
      c.OnDisplayDate(new CalendarDateChangedEventArgs(removedDate, addedDate));
   }

   protected void OnDisplayDate(CalendarDateChangedEventArgs e)
   {
      DisplayDateChanged?.Invoke(this, e);
   }

   private void OnDisplayDateStartChanged(AvaloniaPropertyChangedEventArgs e)
   {
      if (!_displayDateIsChanging) {
         DateTime? newValue = e.NewValue as DateTime?;

         if (newValue.HasValue) {
            // DisplayDateStart coerces to the value of the
            // SelectedDateMin if SelectedDateMin < DisplayDateStart
       
            DateTime? selectedDateMin = SelectedRangeStartDate;

            if (selectedDateMin.HasValue && DateTime.Compare(selectedDateMin.Value, newValue.Value) < 0) {
               SetCurrentValue(DisplayDateStartProperty, selectedDateMin.Value);
               return;
            }

            // if DisplayDateStart > DisplayDateEnd,
            // DisplayDateEnd = DisplayDateStart
            if (DateTime.Compare(newValue.Value, DisplayDateRangeEnd) > 0) {
               SetCurrentValue(DisplayDateEndProperty, DisplayDateStart);
            }

            // If DisplayDate < DisplayDateStart,
            // DisplayDate = DisplayDateStart
            if (DateTimeHelper.CompareYearMonth(newValue.Value, DisplayDateInternal) > 0) {
               SetCurrentValue(DisplayDateProperty, newValue.Value);
            }
         }

         UpdateMonths();
      }
   }

   /// <summary>
   /// Gets a collection of dates that are marked as not selectable.
   /// </summary>
   /// <value>
   /// A collection of dates that cannot be selected. The default value is
   /// an empty collection.
   /// </value>
   /// <exception cref="System.ArgumentOutOfRangeException">
   /// Adding a date to this collection when it is already selected or
   /// adding a date outside the range specified by DisplayDateStart and
   /// DisplayDateEnd.
   /// </exception>
   /// <remarks>
   /// <para>
   /// Dates in this collection will appear as disabled on the calendar.
   /// </para>
   /// <para>
   /// To make all past dates not selectable, you can use the
   /// AddDatesInPast method provided by the collection returned by this
   /// property.
   /// </para>
   /// </remarks>
   public RangeCalendarBlackoutDatesCollection BlackoutDates { get; private set; }
   
   internal DateTime DisplayDateRangeStart
   {
      get => DisplayDateStart.GetValueOrDefault(DateTime.MinValue);
   }

   private void OnDisplayDateEndChanged(AvaloniaPropertyChangedEventArgs e)
   {
      if (!_displayDateIsChanging) {
         DateTime? newValue = e.NewValue as DateTime?;

         if (newValue.HasValue) {
            // DisplayDateEnd coerces to the value of the
            // SelectedDateMax if SelectedDateMax > DisplayDateEnd
            DateTime? selectedDateMax = SelectedRangeEndDate;

            if (selectedDateMax.HasValue && DateTime.Compare(selectedDateMax.Value, newValue.Value) > 0) {
               SetCurrentValue(DisplayDateEndProperty, selectedDateMax.Value);
               return;
            }

            // if DisplayDateEnd < DisplayDateStart,
            // DisplayDateEnd = DisplayDateStart
            if (DateTime.Compare(newValue.Value, DisplayDateRangeStart) < 0) {
               SetCurrentValue(DisplayDateEndProperty, DisplayDateStart);
               return;
            }

            // If DisplayDate > DisplayDateEnd,
            // DisplayDate = DisplayDateEnd
            if (DateTimeHelper.CompareYearMonth(newValue.Value, DisplayDateInternal) < 0) {
               SetCurrentValue(DisplayDateProperty, newValue.Value);
            }
         }

         UpdateMonths();
      }
   }

   internal DateTime DisplayDateRangeEnd
   {
      get => DisplayDateEnd.GetValueOrDefault(DateTime.MaxValue);
   }

   internal DateTime? HoverStart { get; set; }
   internal DateTime? HoverEndInternal { get; set; }

   internal DateTime? HoverEnd
   {
      get => HoverEndInternal;
      set
      {
         HoverEndInternal = value;
         LastSelectedDate = value;
      }
   }
   
   internal bool HasFocusInternal { get; set; }
   internal bool IsMouseSelection { get; set; }
   
   internal int? HoverStartIndex { get; set; }
   internal int? HoverEndIndex { get; set; }

   internal RangeCalendarDayButton? FindDayButtonFromDay(DateTime day)
   {
      RangeCalendarItem? monthControl = MonthControl;

      // REMOVE_RTM: should be updated if we support MultiCalendar
      int count = RowsPerMonth * ColumnsPerMonth;
      if (monthControl != null) {
         if (monthControl.PrimaryMonthView != null) {
            for (int childIndex = ColumnsPerMonth; childIndex < count; childIndex++) {
               if (monthControl.PrimaryMonthView.Children[childIndex] is RangeCalendarDayButton b) {
                  var d = b.DataContext as DateTime?;

                  if (d.HasValue) {
                     if (DateTimeHelper.CompareDays(d.Value, day) == 0) {
                        return b;
                     }
                  }
               }
            }
         }
         if (monthControl.SecondaryMonthView != null) {
            for (int childIndex = ColumnsPerMonth; childIndex < count; childIndex++) {
               if (monthControl.SecondaryMonthView.Children[childIndex] is RangeCalendarDayButton b) {
                  var d = b.DataContext as DateTime?;

                  if (d.HasValue) {
                     if (DateTimeHelper.CompareDays(d.Value, day) == 0) {
                        return b;
                     }
                  }
               }
            }
         }
      }

      return null;
   }

   private void OnSelectedMonthChanged(DateTime? selectedMonth)
   {
      if (selectedMonth.HasValue) {
         Debug.Assert(DisplayMode == CalendarMode.Year, "DisplayMode should be Year!");
         SelectedMonth = selectedMonth.Value;
         UpdateMonths();
      }
   }

   private void OnSelectedYearChanged(DateTime? selectedYear)
   {
      if (selectedYear.HasValue) {
         Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade!");
         SelectedYear = selectedYear.Value;
         UpdateMonths();
      }
   }

   internal void OnHeaderClick()
   {
      Debug.Assert(DisplayMode == CalendarMode.Year || DisplayMode == CalendarMode.Decade,
                   "The DisplayMode should be Year or Decade");
      RangeCalendarItem? monthControl = MonthControl;
      if (monthControl != null && monthControl.MonthView != null && monthControl.YearView != null) {
         monthControl.MonthView.IsVisible = false;
         monthControl.YearView.IsVisible = true;
         UpdateMonths();
      }
   }

   internal void ResetStates()
   {
      RangeCalendarItem? monthControl = MonthControl;
      int count = RowsPerMonth * ColumnsPerMonth;
      if (monthControl != null) {
         if (monthControl.PrimaryMonthView != null) {
            for (int childIndex = ColumnsPerMonth; childIndex < count; childIndex++) {
               var d = (RangeCalendarDayButton)monthControl.PrimaryMonthView.Children[childIndex];
               d.IgnoreMouseOverState();
            }
         }
         if (monthControl.SecondaryMonthView != null) {
            for (int childIndex = ColumnsPerMonth; childIndex < count; childIndex++) {
               var d = (RangeCalendarDayButton)monthControl.SecondaryMonthView.Children[childIndex];
               d.IgnoreMouseOverState();
            }
         }
      }
   }

   protected internal virtual void UpdateMonths()
   {
      RangeCalendarItem? monthControl = MonthControl;
      if (monthControl != null) {
         UpdateCalendarMonths(monthControl);
      }
   }

   internal void UpdateCalendarMonths(RangeCalendarItem calendarItem)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            calendarItem.UpdateMonthMode();
            break;
         }
         case CalendarMode.Year:
         {
            calendarItem.UpdateYearMode();
            break;
         }
         case CalendarMode.Decade:
         {
            calendarItem.UpdateDecadeMode();
            break;
         }
      }
   }

   internal static bool IsValidDateSelection(RangeCalendar cal, DateTime? value)
   {
      if (!value.HasValue) {
         return true;
      } else {
         if (cal.BlackoutDates.Contains(value.Value)) {
            return false;
         } else {
            cal._displayDateIsChanging = true;
            if (DateTime.Compare(value.Value, cal.DisplayDateRangeStart) < 0) {
               cal.DisplayDateStart = value;
            } else if (DateTime.Compare(value.Value, cal.DisplayDateRangeEnd) > 0) {
               cal.DisplayDateEnd = value;
            }

            cal._displayDateIsChanging = false;

            return true;
         }
      }
   }

   private static bool IsValidKeyboardSelection(RangeCalendar cal, DateTime? value)
   {
      if (!value.HasValue) {
         return true;
      } else {
         if (cal.BlackoutDates.Contains(value.Value)) {
            return false;
         } else {
            return (DateTime.Compare(value.Value, cal.DisplayDateRangeStart) >= 0 &&
                    DateTime.Compare(value.Value, cal.DisplayDateRangeEnd) <= 0);
         }
      }
   }

   /// <summary>
   /// This method highlights the days in MultiSelection mode without
   /// adding them to the SelectedDates collection.
   /// </summary>
   internal void HighlightDays()
   {
      if (HoverEnd != null && HoverStart != null) {
         Debug.Assert(MonthControl is not null);

         int startIndex, endIndex;
         RangeCalendarItem monthControl = MonthControl;

         // This assumes a contiguous set of dates:
         if (HoverEndIndex != null && HoverStartIndex != null) {
            SortHoverIndexes(out startIndex, out endIndex);
            if (monthControl.PrimaryMonthView is not null) {
               int primaryStartIndex = MathUtilities.Clamp(startIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               int primaryEndIndex = MathUtilities.Clamp(endIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               HighlightDays(monthControl.PrimaryMonthView, primaryStartIndex, primaryEndIndex);
            }
            if (monthControl.SecondaryMonthView is not null) {
               int secondaryStartIndex = startIndex - RangeCalendarItem.MonthViewSize;
               int secondaryEndIndex = endIndex - RangeCalendarItem.MonthViewSize;
               secondaryStartIndex = MathUtilities.Clamp(secondaryStartIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               secondaryEndIndex = MathUtilities.Clamp(secondaryEndIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               HighlightDays(monthControl.SecondaryMonthView, secondaryStartIndex, secondaryEndIndex);
            }
         }
      }
   }

   private void HighlightDays(Grid targetMonthView, int startIndex, int endIndex)
   {
      if (HoverEnd != null && HoverStart != null) {
         for (int i = startIndex; i <= endIndex; i++) {
            if (targetMonthView.Children[i] is RangeCalendarDayButton b) {
               b.IsSelected = true;
               var d = b.DataContext as DateTime?;

               if (d.HasValue && DateTimeHelper.CompareDays(HoverEnd.Value, d.Value) == 0) {
                  if (FocusButton != null) {
                     FocusButton.IsCurrent = false;
                  }

                  b.IsCurrent = HasFocusInternal;
                  FocusButton = b;
               }
            }
         }
      }
   }

   /// <summary>
   /// This method un-highlights the days that were hovered over but not
   /// added to the SelectedDates collection or un-highlighted the
   /// previously selected days in SingleRange Mode.
   /// </summary>
   internal void UnHighlightDays()
   {
      if (HoverEnd != null && HoverStart != null) {
         Debug.Assert(MonthControl is not null);

         RangeCalendarItem monthControl = MonthControl;

         if (HoverEndIndex != null && HoverStartIndex != null) {
            SortHoverIndexes(out int startIndex, out int endIndex);
            if (monthControl.PrimaryMonthView is not null) {
               int primaryStartIndex = MathUtilities.Clamp(startIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               int primaryEndIndex = MathUtilities.Clamp(endIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               UnHighlightDays(monthControl.PrimaryMonthView, primaryStartIndex, primaryEndIndex);
            }
            if (monthControl.SecondaryMonthView is not null) {
               int secondaryStartIndex = startIndex - RangeCalendarItem.MonthViewSize;
               int secondaryEndIndex = endIndex - RangeCalendarItem.MonthViewSize;
               secondaryStartIndex = MathUtilities.Clamp(secondaryStartIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               secondaryEndIndex = MathUtilities.Clamp(secondaryEndIndex, 7, RangeCalendarItem.MonthViewSize - 1);
               UnHighlightDays(monthControl.SecondaryMonthView, secondaryStartIndex, secondaryEndIndex);
            }
         }
      }
   }

   private void UnHighlightDays(Grid targetMonthView, int startIndex, int endIndex)
   {
      if (HoverEnd != null && HoverStart != null) {
         // It is SingleRange
         for (int i = startIndex; i <= endIndex; i++) {
            ((RangeCalendarDayButton)targetMonthView.Children[i]).IsSelected = false;
         }
      }
   }

   internal void SortHoverIndexes(out int startIndex, out int endIndex)
   {
      Debug.Assert(HoverStart.HasValue);
      Debug.Assert(HoverEnd.HasValue);
      Debug.Assert(HoverStartIndex.HasValue);
      Debug.Assert(HoverEndIndex.HasValue);

      if (DateTimeHelper.CompareDays(HoverEnd.Value, HoverStart.Value) > 0) {
         startIndex = HoverStartIndex.Value;
         endIndex = HoverEndIndex.Value;
      } else {
         startIndex = HoverEndIndex.Value;
         endIndex = HoverStartIndex.Value;
      }
   }

   internal void OnPreviousMonthClick()
   {
      if (DisplayMode == CalendarMode.Month) {
         DateTime? d = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), -1);
         if (d.HasValue) {
            if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0) {
               LastSelectedDate = d.Value;
            }

            SetCurrentValue(DisplayDateProperty, d.Value);
         }
      }
   }

   internal void OnPreviousClick()
   {
      if (DisplayMode == CalendarMode.Month) {
         DateTime? d = DateTimeHelper.AddYears(DateTimeHelper.DiscardDayTime(DisplayDate), -1);
         if (d.HasValue) {
            if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0) {
               LastSelectedDate = d.Value;
            }

            SetCurrentValue(DisplayDateProperty, d.Value);
         }
      } else if (DisplayMode == CalendarMode.Year) {
         DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), -1);

         if (d.HasValue) {
            SelectedMonth = d.Value;
         } else {
            SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
         }
      } else if (DisplayMode == CalendarMode.Decade) {
         Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade!");

         DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), -10);

         if (d.HasValue) {
            int decade = Math.Max(1, DateTimeHelper.DecadeOfDate(d.Value));
            SelectedYear = new DateTime(decade, 1, 1);
         } else {
            SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
         }
      }

      UpdateMonths();
   }

   internal void OnNextMonthClick()
   {
      if (DisplayMode == CalendarMode.Month) {
         DateTime? d = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), 1);
         if (d.HasValue) {
            if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0) {
               LastSelectedDate = d.Value;
            }

            SetCurrentValue(DisplayDateProperty, d.Value);
         }
      }
   }

   internal void OnNextClick()
   {
      if (DisplayMode == CalendarMode.Month) {
         DateTime? d = DateTimeHelper.AddYears(DateTimeHelper.DiscardDayTime(DisplayDate), 1);
         if (d.HasValue) {
            if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0) {
               LastSelectedDate = d.Value;
            }

            SetCurrentValue(DisplayDateProperty, d.Value);
         }
      } else if (DisplayMode == CalendarMode.Year) {
         DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), 1);

         if (d.HasValue) {
            SelectedMonth = d.Value;
         } else {
            SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
         }
      } else if (DisplayMode == CalendarMode.Decade) {
         Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade");

         DateTime? d = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), 10);

         if (d.HasValue) {
            int decade = Math.Max(1, DateTimeHelper.DecadeOfDate(d.Value));
            SelectedYear = new DateTime(decade, 1, 1);
         } else {
            SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
         }
      }

      UpdateMonths();
   }

   /// <summary>
   /// If the day is a trailing day, Update the DisplayDate.
   /// </summary>
   /// <param name="selectedDate">Inherited code: Requires comment.</param>
   internal void OnDayClick(DateTime selectedDate)
   {
      Debug.Assert(DisplayMode == CalendarMode.Month, "DisplayMode should be Month!");
      int i = DateTimeHelper.CompareYearMonth(selectedDate, DisplayDateInternal);

      if (i > 1) {
         OnNextMonthClick();
      } else if (i < 0) {
         OnPreviousMonthClick();
      }
   }

   private void OnMonthClick()
   {
      Debug.Assert(MonthControl is not null);

      RangeCalendarItem monthControl = MonthControl;
      if (monthControl != null && monthControl.YearView != null && monthControl.MonthView != null) {
         monthControl.YearView.IsVisible = false;
         monthControl.MonthView.IsVisible = true;

         if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDate) != 0) {
            LastSelectedDate = DisplayDate;
         }

         UpdateMonths();
      }
   }

   public override string ToString()
   {
      if (SelectedRangeStartDate != null || SelectedRangeEndDate != null) {
         var builder = new StringBuilder();
         if (SelectedRangeStartDate != null) {
            builder.Append(SelectedRangeStartDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat()));
         } else {
            builder.Append('?');
         }
         builder.Append(" - ");
         if (SelectedRangeEndDate != null) {
            builder.Append(SelectedRangeEndDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat()));
         } else {
            builder.Append('?');
         }

         return builder.ToString();
      }
      return string.Empty;
   }

   public event EventHandler<SelectionChangedEventArgs>? SelectedDatesChanged;

   /// <summary>
   /// Occurs when the
   /// <see cref="P:System.Windows.Controls.RangeCalendar.DisplayDate" />
   /// property is changed.
   /// </summary>
   /// <remarks>
   /// This event occurs after DisplayDate is assigned its new value.
   /// </remarks>
   public event EventHandler<CalendarDateChangedEventArgs>? DisplayDateChanged;

   /// <summary>
   /// Occurs when the
   /// <see cref="P:System.Windows.Controls.RangeCalendar.DisplayMode" />
   /// property is changed.
   /// </summary>
   public event EventHandler<CalendarModeChangedEventArgs>? DisplayModeChanged;

   /// <summary>
   /// Inherited code: Requires comment.
   /// </summary>
   internal event EventHandler<PointerReleasedEventArgs>? DayButtonMouseUp;
   
   private void ProcessSelection(bool shift, DateTime? lastSelectedDate, int? index)
   {
      if (lastSelectedDate != null && IsValidKeyboardSelection(this, lastSelectedDate.Value)) {
         if (shift) {
            RangeCalendarDayButton? b;
            _isShiftPressed = true;
            if (HoverStart == null) {
               if (LastSelectedDate != null) {
                  HoverStart = LastSelectedDate;
               } else {
                  if (DateTimeHelper.CompareYearMonth(DisplayDateInternal, DateTime.Today) == 0) {
                     HoverStart = DateTime.Today;
                  } else {
                     HoverStart = DisplayDateInternal;
                  }
               }

               b = FindDayButtonFromDay(HoverStart.Value);
               if (b != null) {
                  HoverStartIndex = b.Index;
               }
            }

            // the index of the SelectedDate is always the last
            // selectedDate's index
            UnHighlightDays();
            // If we hit a BlackOutDay with keyboard we do not
            // update the HoverEnd
            CalendarDateRange range;

            if (DateTime.Compare(HoverStart.Value, lastSelectedDate.Value) < 0) {
               range = new CalendarDateRange(HoverStart.Value, lastSelectedDate.Value);
            } else {
               range = new CalendarDateRange(lastSelectedDate.Value, HoverStart.Value);
            }

            if (!BlackoutDates.ContainsAny(range)) {
               HoverEnd = lastSelectedDate;

               if (index.HasValue) {
                  HoverEndIndex += index;
               } else {
                  Debug.Assert(HoverEndInternal is not null);

                  // For Home, End, PageUp and PageDown Keys there
                  // is no easy way to predict the index value
                  b = FindDayButtonFromDay(HoverEndInternal.Value);

                  if (b != null) {
                     HoverEndIndex = b.Index;
                  }
               }
            }

            Debug.Assert(HoverEnd is not null);
            OnDayClick(HoverEnd.Value);
            HighlightDays();
         } else {
            HoverStart = lastSelectedDate;
            HoverEnd = lastSelectedDate;
            OnDayClick(lastSelectedDate.Value);
         }
      }
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);
      if (!HasFocusInternal && e.InitialPressMouseButton == MouseButton.Left) {
         Focus();
      }
   }

   internal void OnDayButtonMouseUp(PointerReleasedEventArgs e)
   {
      DayButtonMouseUp?.Invoke(this, e);
   }

   /// <summary>
   /// Default mouse wheel handler for the calendar control.
   /// </summary>
   /// <param name="e">Mouse wheel event args.</param>
   protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
   {
      base.OnPointerWheelChanged(e);
      if (!e.Handled) {
         CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out bool ctrl, out bool shift);

         if (!ctrl) {
            if (e.Delta.Y > 0) {
               ProcessPageUpKey(false);
            } else {
               ProcessPageDownKey(false);
            }
         } else {
            if (e.Delta.Y > 0) {
               ProcessDownKey(ctrl, shift);
            } else {
               ProcessUpKey(ctrl, shift);
            }
         }

         e.Handled = true;
      }
   }

   internal void HandleCalendarKeyDown(KeyEventArgs e)
   {
      if (!e.Handled && IsEnabled) {
         e.Handled = ProcessCalendarKey(e);
      }
   }

   internal bool ProcessCalendarKey(KeyEventArgs e)
   {
      if (DisplayMode == CalendarMode.Month) {
         if (LastSelectedDate.HasValue) {
            // If a blackout day is inactive, when clicked on it, the
            // previous inactive day which is not a blackout day can get
            // the focus.  In this case we should allow keyboard
            // functions on that inactive day
            if (DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDateInternal) != 0 &&
                FocusButton != null && !FocusButton.IsInactive) {
               return true;
            }
         }
      }

      // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
      Key invariantKey = e.Key; //InteractionHelper.GetLogicalKey(FlowDirection, e.Key);

      CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out bool ctrl, out bool shift);

      switch (invariantKey) {
         case Key.Up:
         {
            ProcessUpKey(ctrl, shift);
            return true;
         }
         case Key.Down:
         {
            ProcessDownKey(ctrl, shift);
            return true;
         }
         case Key.Left:
         {
            ProcessLeftKey(shift);
            return true;
         }
         case Key.Right:
         {
            ProcessRightKey(shift);
            return true;
         }
         case Key.PageDown:
         {
            ProcessPageDownKey(shift);
            return true;
         }
         case Key.PageUp:
         {
            ProcessPageUpKey(shift);
            return true;
         }
         case Key.Home:
         {
            ProcessHomeKey(shift);
            return true;
         }
         case Key.End:
         {
            ProcessEndKey(shift);
            return true;
         }
         case Key.Enter:
         case Key.Space:
         {
            return ProcessEnterKey();
         }
      }

      return false;
   }

   internal void ProcessUpKey(bool ctrl, bool shift)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            if (ctrl) {
               SelectedMonth = DisplayDateInternal;
               SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
            } else {
               DateTime? selectedDate =
                  DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), -ColumnsPerMonth);
               ProcessSelection(shift, selectedDate, -ColumnsPerMonth);
            }

            break;
         }
         case CalendarMode.Year:
         {
            if (ctrl) {
               SelectedYear = SelectedMonth;
               SetCurrentValue(DisplayModeProperty, CalendarMode.Decade);
            } else {
               DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -ColumnsPerYear);
               OnSelectedMonthChanged(selectedMonth);
            }

            break;
         }
         case CalendarMode.Decade:
         {
            if (!ctrl) {
               DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -ColumnsPerYear);
               OnSelectedYearChanged(selectedYear);
            }

            break;
         }
      }
   }

   internal void ProcessDownKey(bool ctrl, bool shift)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            if (!ctrl || shift) {
               DateTime? selectedDate =
                  DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), ColumnsPerMonth);
               ProcessSelection(shift, selectedDate, ColumnsPerMonth);
            }

            break;
         }
         case CalendarMode.Year:
         {
            if (ctrl) {
               SetCurrentValue(DisplayDateProperty, SelectedMonth);
               SetCurrentValue(DisplayModeProperty, CalendarMode.Month);
            } else {
               DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, ColumnsPerYear);
               OnSelectedMonthChanged(selectedMonth);
            }

            break;
         }
         case CalendarMode.Decade:
         {
            if (ctrl) {
               SelectedMonth = SelectedYear;
               SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
            } else {
               DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, ColumnsPerYear);
               OnSelectedYearChanged(selectedYear);
            }

            break;
         }
      }
   }

   internal void ProcessLeftKey(bool shift)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            DateTime? selectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), -1);
            ProcessSelection(shift, selectedDate, -1);
            break;
         }
         case CalendarMode.Year:
         {
            DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -1);
            OnSelectedMonthChanged(selectedMonth);
            break;
         }
         case CalendarMode.Decade:
         {
            DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -1);
            OnSelectedYearChanged(selectedYear);
            break;
         }
      }
   }

   internal void ProcessRightKey(bool shift)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            DateTime? selectedDate = DateTimeHelper.AddDays(LastSelectedDate.GetValueOrDefault(DateTime.Today), 1);
            ProcessSelection(shift, selectedDate, 1);
            break;
         }
         case CalendarMode.Year:
         {
            DateTime? selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, 1);
            OnSelectedMonthChanged(selectedMonth);
            break;
         }
         case CalendarMode.Decade:
         {
            DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 1);
            OnSelectedYearChanged(selectedYear);
            break;
         }
      }
   }

   private bool ProcessEnterKey()
   {
      switch (DisplayMode) {
         case CalendarMode.Year:
         {
            SetCurrentValue(DisplayDateProperty, SelectedMonth);
            SetCurrentValue(DisplayModeProperty, CalendarMode.Month);
            return true;
         }
         case CalendarMode.Decade:
         {
            SelectedMonth = SelectedYear;
            SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
            return true;
         }
      }

      return false;
   }

   internal void ProcessHomeKey(bool shift)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            // REMOVE_RTM: Not all types of calendars start with Day1. If Non-Gregorian is supported check this:
            DateTime? selectedDate = new DateTime(DisplayDateInternal.Year, DisplayDateInternal.Month, 1);
            ProcessSelection(shift, selectedDate, null);
            break;
         }
         case CalendarMode.Year:
         {
            DateTime selectedMonth = new DateTime(_selectedMonth.Year, 1, 1);
            OnSelectedMonthChanged(selectedMonth);
            break;
         }
         case CalendarMode.Decade:
         {
            DateTime? selectedYear = new DateTime(DateTimeHelper.DecadeOfDate(SelectedYear), 1, 1);
            OnSelectedYearChanged(selectedYear);
            break;
         }
      }
   }

   internal void ProcessEndKey(bool shift)
   {
      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            DateTime? selectedDate = new DateTime(DisplayDateInternal.Year, DisplayDateInternal.Month, 1);

            if (DateTimeHelper.CompareYearMonth(DateTime.MaxValue, selectedDate.Value) > 0) {
               // since DisplayDate is not equal to
               // DateTime.MaxValue we are sure selectedDate is\
               // not null
               selectedDate = DateTimeHelper.AddMonths(selectedDate.Value, 1)!.Value;
               selectedDate = DateTimeHelper.AddDays(selectedDate.Value, -1)!.Value;
            } else {
               selectedDate = DateTime.MaxValue;
            }

            ProcessSelection(shift, selectedDate, null);
            break;
         }
         case CalendarMode.Year:
         {
            DateTime selectedMonth = new DateTime(_selectedMonth.Year, 12, 1);
            OnSelectedMonthChanged(selectedMonth);
            break;
         }
         case CalendarMode.Decade:
         {
            DateTime? selectedYear = new DateTime(DateTimeHelper.EndOfDecade(SelectedYear), 1, 1);
            OnSelectedYearChanged(selectedYear);
            break;
         }
      }
   }

   internal void ProcessPageDownKey(bool shift)
   {
      if (!shift) {
         OnNextClick();
         return;
      }

      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            DateTime? selectedDate = DateTimeHelper.AddMonths(LastSelectedDate.GetValueOrDefault(DateTime.Today), 1);
            ProcessSelection(shift, selectedDate, null);
            break;
         }
         case CalendarMode.Year:
         {
            DateTime? selectedMonth = DateTimeHelper.AddYears(_selectedMonth, 1);
            OnSelectedMonthChanged(selectedMonth);
            break;
         }
         case CalendarMode.Decade:
         {
            DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, 10);
            OnSelectedYearChanged(selectedYear);
            break;
         }
      }
   }

   internal void ProcessPageUpKey(bool shift)
   {
      if (!shift) {
         OnPreviousClick();
         return;
      }

      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            DateTime? selectedDate = DateTimeHelper.AddMonths(LastSelectedDate.GetValueOrDefault(DateTime.Today), -1);
            ProcessSelection(shift, selectedDate, null);
            break;
         }
         case CalendarMode.Year:
         {
            DateTime? selectedMonth = DateTimeHelper.AddYears(_selectedMonth, -1);
            OnSelectedMonthChanged(selectedMonth);
            break;
         }
         case CalendarMode.Decade:
         {
            DateTime? selectedYear = DateTimeHelper.AddYears(SelectedYear, -10);
            OnSelectedYearChanged(selectedYear);
            break;
         }
      }
   }
   
   protected override void OnGotFocus(GotFocusEventArgs e)
   {
      base.OnGotFocus(e);
      HasFocusInternal = true;

      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            DateTime focusDate;
            if (LastSelectedDate.HasValue &&
                DateTimeHelper.CompareYearMonth(DisplayDateInternal, LastSelectedDate.Value) == 0) {
               focusDate = LastSelectedDate.Value;
            } else {
               focusDate = DisplayDate;
               LastSelectedDate = DisplayDate;
            }

            FocusButton = FindDayButtonFromDay(focusDate);

            if (FocusButton != null) {
               FocusButton.IsCurrent = true;
            }

            break;
         }
         case CalendarMode.Year:
         case CalendarMode.Decade:
         {
            if (this.FocusCalendarButton != null) {
               FocusCalendarButton.IsCalendarButtonFocused = true;
            }

            break;
         }
      }
   }

   protected override void OnLostFocus(RoutedEventArgs e)
   {
      base.OnLostFocus(e);
      HasFocusInternal = false;

      switch (DisplayMode) {
         case CalendarMode.Month:
         {
            if (FocusButton != null) {
               FocusButton.IsCurrent = false;
            }

            break;
         }
         case CalendarMode.Year:
         case CalendarMode.Decade:
         {
            if (FocusCalendarButton != null) {
               FocusCalendarButton.IsCalendarButtonFocused = false;
            }

            break;
         }
      }
   }

   /// <summary>
   ///  Called when the IsEnabled property changes.
   /// </summary>
   /// <param name="e">Property changed args.</param>
   private void OnIsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
   {
      Debug.Assert(e.NewValue is bool, "NewValue should be a boolean!");
      bool isEnabled = (bool)e.NewValue;

      MonthControl?.UpdateDisabled(isEnabled);
   }

   /// <summary>
   /// Builds the visual tree for the
   /// <see cref="T:System.Windows.Controls.RangeCalendar" /> when a new
   /// template is applied.
   /// </summary>
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      Root = e.NameScope.Find<Panel>(RangeCalendarTheme.RootPart);

      SelectedMonth = DisplayDate;
      SelectedYear = DisplayDate;

      if (Root != null) {
         RangeCalendarItem? month = e.NameScope.Find<RangeCalendarItem>(RangeCalendarTheme.CalendarItemPart);

         if (month != null) {
            month.Owner = this;
         }
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      TokenResourceBinder.CreateGlobalTokenBinding(this, BorderThicknessProperty,
                                                   GlobalTokenResourceKey.BorderThickness,
                                                   BindingPriority.Template,
                                                   new RenderScaleAwareThicknessConfigure(this));
   }
}
