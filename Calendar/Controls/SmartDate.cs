using System.Windows;
using System.Windows.Controls;

namespace Calendar;

public class SmartDate : Control
{
    static SmartDate()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SmartDate), new FrameworkPropertyMetadata(typeof(SmartDate)));
    }
}
