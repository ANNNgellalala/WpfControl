using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calendar;

[TemplatePart(Name = nameof(Day), Type = typeof(Grid))]
[TemplatePart(Name = nameof(Year), Type = typeof(Grid))]
[TemplatePart(Name = nameof(Month), Type = typeof(Grid))]
public class Calendar : Control, INotifyPropertyChanged
{
    #region 模板成员

    [NotNull]
    private Grid? Day { get; set; }

    [NotNull]
    private Grid? Year { get; set; }

    [NotNull]
    private Grid? Month { get; set; }

    #endregion

    #region 构造函数

    static Calendar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(typeof(Calendar)));
    }

    public Calendar()
    {
        var @default = new DefaultCalendarFestivals();
        CalendarFestivals = @default;
        CalendarHolidays = @default;
        Years = Enumerable.Range(DateTime.Today.Year - 10, 20)
                          .Select(i => new CalendarItem()
                          {
                              Value = i, IsSelected = i == SelectedYear
                          })
                          .ToList();
        Months = Enumerable.Range(1, 12)
                           .Select(i => new CalendarItem()
                           {
                               Value = i, IsSelected = i == SelectedMonth
                           })
                           .ToList();
        ChangeSelectViewCommand = new Command<SelectTarget>(ChangeSelectView, CanChangeSelectView);
        IncreaseCommand = new Command(Increase, CanIncrease);
        DecreaseCommand = new Command(Decrease, CanDecrease);
        DecreaseMonthCommand = new Command(DecreaseMonth, CanDecreaseMonth);
        IncreaseMonthCommand = new Command(IncreaseMonth, CanIncreaseMonth);
        SelectDayCommand = new Command<CalendarItem>(SelectDay, CanSelectDay);
        SelectMonthCommand = new Command<CalendarItem>(SelectMonth, CanSelectMonth);
        SelectYearCommand = new Command<CalendarItem>(SelectYear, CanSelectYear);
        UpdateDates();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        Day = GetTemplateChild(nameof(Day)) as Grid;
        Year = GetTemplateChild(nameof(Year)) as Grid;
        Month = GetTemplateChild(nameof(Month)) as Grid;

        UpdateViewVisibilityState();
    }

    #endregion

    #region 节假日接口依赖属性

    public static readonly DependencyProperty CalendarFestivalsProperty = DependencyProperty.Register(nameof(CalendarFestivals),
        typeof(ICalendarFestivals),
        typeof(Calendar),
        new PropertyMetadata(default(ICalendarFestivals)));

    public ICalendarFestivals CalendarFestivals
    {
        get => (ICalendarFestivals)GetValue(CalendarFestivalsProperty);
        set => SetValue(CalendarFestivalsProperty, value);
    }

    public static readonly DependencyProperty CalendarHolidaysProperty = DependencyProperty.Register(nameof(CalendarHolidays),
        typeof(ICalendarHolidays),
        typeof(Calendar),
        new PropertyMetadata(default(ICalendarHolidays)));

    public ICalendarHolidays CalendarHolidays
    {
        get => (ICalendarHolidays)GetValue(CalendarHolidaysProperty);
        set => SetValue(CalendarHolidaysProperty, value);
    }

    #endregion

    #region 附加文本显示依赖属性

    public static readonly DependencyProperty ShowLunarProperty = DependencyProperty.Register(nameof(ShowLunar),
        typeof(bool),
        typeof(Calendar),
        new PropertyMetadata(default(bool), OnShowingOptionsChanged));

    public bool ShowLunar
    {
        get => (bool)GetValue(ShowLunarProperty);
        set => SetValue(ShowLunarProperty, value);
    }

    public static readonly DependencyProperty ShowSolarTermProperty = DependencyProperty.Register(nameof(ShowSolarTerm),
        typeof(bool),
        typeof(Calendar),
        new PropertyMetadata(default(bool), OnShowingOptionsChanged));

    public bool ShowSolarTerm
    {
        get => (bool)GetValue(ShowSolarTermProperty);
        set => SetValue(ShowSolarTermProperty, value);
    }

    public static readonly DependencyProperty ShowFestivalsProperty = DependencyProperty.Register(nameof(ShowFestivals),
        typeof(bool),
        typeof(Calendar),
        new PropertyMetadata(default(bool), OnShowingOptionsChanged));

    public bool ShowFestivals
    {
        get => (bool)GetValue(ShowFestivalsProperty);
        set => SetValue(ShowFestivalsProperty, value);
    }

    public static readonly DependencyProperty ShowHolidaysProperty = DependencyProperty.Register(nameof(ShowHolidays),
        typeof(bool),
        typeof(Calendar),
        new PropertyMetadata(default(bool), OnShowingOptionsChanged));

    public bool ShowHolidays
    {
        get => (bool)GetValue(ShowHolidaysProperty);
        set => SetValue(ShowHolidaysProperty, value);
    }

    private static void OnShowingOptionsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
            return;

        if (d is not Calendar calendar)
            return;

        calendar.UpdateDates();
    }
    
    private Visibility _timeLabelVisibility = Visibility.Visible;

    public Visibility TimeLabelVisibility 
    {
        get => _timeLabelVisibility;
        set => SetField(ref _timeLabelVisibility, value);
    }

    public static readonly DependencyProperty ShowTimeSelectionProperty = DependencyProperty.Register(nameof(ShowTimeSelection), typeof(bool), typeof(Calendar), new PropertyMetadata(true, OnShowTimeSelectionChanged));

    public bool ShowTimeSelection
    {
        get => (bool)GetValue(ShowTimeSelectionProperty);
        set => SetValue(ShowTimeSelectionProperty, value);
    }
    
    private static void OnShowTimeSelectionChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == e.OldValue)
            return;

        if (d is not Calendar calendar)
            return;

        calendar.TimeLabelVisibility = calendar.ShowTimeSelection ? Visibility.Visible : Visibility.Collapsed;
    }
    #endregion
    
    private void UpdateDates()
    {
        var today = DateTime.Today;
        var dates = new List<CalendarItem>(35);
        var start = new DateTime(SelectedYear, SelectedMonth, 1);
        var dayOfWeek = start.DayOfWeek;
        start = start.AddDays(-1 * (int)dayOfWeek);
        for (var i = 0; i < 35; i++)
        {
            var day = start.AddDays(i);
            var item = new CalendarItem
            {
                Date = DateOnly.FromDateTime(day),
                Value = day.Day,
                Text = day.ToLunarText(ShowSolarTerm, ShowFestivals ? CalendarFestivals : null),
                InCurrentMonth = day.Month == SelectedMonth,
                IsToday = day == today,
                IsHoliday = CalendarHolidays.IsHoliday(day),
                IsSelected = DateOnly.FromDateTime(day) == SelectedDate
            };
            dates.Add(item);
        }

        // 35个Item可能导致本月没有全部显示，需要补足
        var lastDay = new DateOnly(SelectedYear, SelectedMonth, 1).AddMonths(1).AddDays(-1);
        if (dates[^1].Date < lastDay)
        {
            var current = dates[^1].Date.AddDays(1);
            var day = new DateTime(current.Year, current.Month, current.Day);
            while (current <= lastDay)
            {
                var item = new CalendarItem
                {
                    Date = current,
                    Value = current.Day,
                    Text = day.ToLunarText(ShowSolarTerm, ShowFestivals ? CalendarFestivals : null),
                    InCurrentMonth = current.Month == SelectedMonth,
                    IsToday = day == today,
                    IsHoliday = CalendarHolidays.IsHoliday(day),
                    IsSelected = current == SelectedDate
                };
                dates.Add(item);
                current = current.AddDays(1);
            }
        }
        
        Dates = dates;
    }

    #region 选择视图切换控制

    private SelectTarget _selectView = SelectTarget.Day;

    public SelectTarget SelectView
    {
        get => _selectView;
        set
        {
            if (SetField(ref _selectView, value))
                UpdateViewVisibilityState();
        }
    }

    private void UpdateViewVisibilityState()
    {
        switch (SelectView)
        {
            case SelectTarget.Year:
                Years = Enumerable.Range(SelectedYear - 10, 20)
                                  .Select(i => new CalendarItem()
                                  {
                                      Value = i, IsSelected = i == SelectedYear
                                  })
                                  .ToList();
                Year.Visibility = Visibility.Visible;
                Month.Visibility = Visibility.Collapsed;
                Day.Visibility = Visibility.Collapsed;
                break;
            case SelectTarget.Month:
                Months = Enumerable.Range(1, 12)
                                   .Select(i => new CalendarItem()
                                   {
                                       Value = i, IsSelected = i == SelectedMonth
                                   })
                                   .ToList();
                Year.Visibility = Visibility.Collapsed;
                Month.Visibility = Visibility.Visible;
                Day.Visibility = Visibility.Collapsed;
                break;
            case SelectTarget.Day:
                Year.Visibility = Visibility.Collapsed;
                Month.Visibility = Visibility.Collapsed;
                Day.Visibility = Visibility.Visible;
                break;
            case SelectTarget.Time:
                Year.Visibility = Visibility.Collapsed;
                Month.Visibility = Visibility.Collapsed;
                Day.Visibility = Visibility.Collapsed;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ICommand ChangeSelectViewCommand { get; set; }

    private bool CanChangeSelectView(
        SelectTarget target) => true;

    private void ChangeSelectView(
        SelectTarget target) => SelectView = target;

    #endregion

    #region DecreaseCommand、IncreaseCommand和相关方法

    public static readonly DependencyProperty DecreaseCommandProperty =
        DependencyProperty.Register(nameof(DecreaseCommand), typeof(ICommand), typeof(Calendar), new PropertyMetadata(default(ICommand)));

    public ICommand DecreaseCommand
    {
        get => (ICommand)GetValue(DecreaseCommandProperty);
        set => SetValue(DecreaseCommandProperty, value);
    }

    private bool CanDecrease() => SelectView is not SelectTarget.Time;

    private void Decrease()
    {
        switch (SelectView)
        {
            case SelectTarget.Year:
                Years = Enumerable.Range(Years[0].Value - 20, 20)
                                  .Select(i => new CalendarItem()
                                  {
                                      Value = i, IsSelected = i == SelectedYear
                                  })
                                  .ToList();
                break;
            case SelectTarget.Time:
                break;
            case SelectTarget.Month:
            case SelectTarget.Day:
                SelectedYear--;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static readonly DependencyProperty IncreaseCommandProperty =
        DependencyProperty.Register(nameof(IncreaseCommand), typeof(ICommand), typeof(Calendar), new PropertyMetadata(default(ICommand)));

    public ICommand IncreaseCommand
    {
        get => (ICommand)GetValue(IncreaseCommandProperty);
        set => SetValue(IncreaseCommandProperty, value);
    }

    private bool CanIncrease() => SelectView is not SelectTarget.Time;

    private void Increase()
    {
        switch (SelectView)
        {
            case SelectTarget.Year:
                Years = Enumerable.Range(Years[^1].Value + 1, 20)
                                  .Select(i => new CalendarItem()
                                  {
                                      Value = i, IsSelected = i == SelectedYear
                                  })
                                  .ToList();
                break;
            case SelectTarget.Time:
                break;
            case SelectTarget.Day:
            case SelectTarget.Month:
                SelectedYear++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public Command DecreaseMonthCommand { get; set; }

    private bool CanDecreaseMonth() => SelectView is not SelectTarget.Time and not SelectTarget.Month;

    private void DecreaseMonth()
    {
        if (SelectedMonth == 1)
        {
            SelectedYear--;
            SelectedMonth = 12;
        }
        else
            SelectedMonth--;
    }

    public Command IncreaseMonthCommand { get; set; }

    private bool CanIncreaseMonth() => true;

    private void IncreaseMonth()
    {
        if (SelectedMonth == 12)
        {
            SelectedMonth = 1;
            SelectedYear++;
        }
        else
            SelectedMonth++;
    }

    #endregion

    #region 选择年、月、日

    private List<CalendarItem> _years = [];

    public List<CalendarItem> Years
    {
        get => _years;
        private set => SetField(ref _years, value);
    }

    public ICommand SelectYearCommand { get; set; }

    private bool CanSelectYear(
        CalendarItem item) => true;

    private void SelectYear(
        CalendarItem item)
    {
        SelectedYear = item.Value;
        SelectView = SelectTarget.Month;
    }

    private List<CalendarItem> _months = [];

    public List<CalendarItem> Months
    {
        get => _months;
        private set => SetField(ref _months, value);
    }

    public ICommand SelectMonthCommand { get; set; }

    private bool CanSelectMonth(
        CalendarItem item) => true;

    private void SelectMonth(
        CalendarItem item)
    {
        SelectView = SelectTarget.Day;
        SelectedMonth = item.Value;
    }

    private List<CalendarItem> _dates = [];

    public List<CalendarItem> Dates
    {
        get => _dates;
        private set => SetField(ref _dates, value);
    }

    public ICommand SelectDayCommand { get; set; }

    private bool CanSelectDay(
        CalendarItem item) => true;

    private void SelectDay(
        CalendarItem item)
    {
        var find = Dates.Find(i => i.Date == SelectedDate);
        if (find != null)
            find.IsSelected = false;
        // 判断选中的是不是上一个月或者下一个月的日期
        if (item.Date.Month != SelectedMonth)
        {
            if (item.Date < SelectedDate)
                DecreaseMonth();
            else
                IncreaseMonth();
            var @new = Dates.Find(i => i.Date == item.Date);
            ArgumentNullException.ThrowIfNull(@new);
            item = @new!;
        }
        SelectedDate = item.Date;
        item.IsSelected = true;
    }

    #endregion

    #region 年月日时分秒属性和SelecetedDate依赖属性

    private int _selectedYear = DateTime.Today.Year;

    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (!SetField(ref _selectedYear, value))
                return;
            UpdateDates();
        }
    }

    private int _selectedMonth = DateTime.Today.Month;

    public int SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            if (!SetField(ref _selectedMonth, value))
                return;
            UpdateDates();
        }
    }

    private int _selectedHour = DateTime.Now.Hour;

    public int SelectedHour
    {
        get => _selectedHour;
        set => SetField(ref _selectedHour, value);
    }

    private int _selectedMinute = DateTime.Now.Minute;

    public int SelectedMinute
    {
        get => _selectedMinute;
        set => SetField(ref _selectedMinute, value);
    }

    private int _selectedSecond = DateTime.Now.Second;

    public int SelectedSecond
    {
        get => _selectedSecond;
        set => SetField(ref _selectedSecond, value);
    }

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(nameof(SelectedDate), typeof(DateOnly), typeof(Calendar), new PropertyMetadata(DateOnly.FromDateTime(DateTime.Today)));

    public DateOnly SelectedDate
    {
        get => (DateOnly)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(nameof(SelectedTime), typeof(TimeOnly), typeof(Calendar), new PropertyMetadata(TimeOnly.FromDateTime(DateTime.Now)));

    public TimeOnly SelectedTime
    {
        get => (TimeOnly)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    #endregion

    #region INotifyPropertyChanged接口实现

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
