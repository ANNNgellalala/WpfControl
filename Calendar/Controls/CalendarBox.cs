using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Calendar;

public class CalendarBox : ButtonBase
{
    public static readonly DependencyProperty IsTodayProperty =
        DependencyProperty.Register(nameof(IsToday), typeof(bool), typeof(CalendarBox), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty InCurrentMonthProperty =
        DependencyProperty.Register(nameof(InCurrentMonth), typeof(bool), typeof(CalendarBox), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CalendarBox), new PropertyMetadata(default(bool)));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool InCurrentMonth
    {
        get => (bool)GetValue(InCurrentMonthProperty);
        set => SetValue(InCurrentMonthProperty, value);
    }

    public bool IsToday
    {
        get => (bool)GetValue(IsTodayProperty);
        set => SetValue(IsTodayProperty, value);
    }

    static CalendarBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarBox), new FrameworkPropertyMetadata(typeof(CalendarBox)));
    }
}
