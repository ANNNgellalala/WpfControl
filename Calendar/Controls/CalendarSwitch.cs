using System.Windows;
using System.Windows.Controls.Primitives;

namespace Calendar;

public class CalendarSwitch : ToggleButton
{
    static CalendarSwitch()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarSwitch), new FrameworkPropertyMetadata(typeof(CalendarSwitch)));
    }
}
