namespace Calendar;

public sealed class CalendarItem : InternalNotifyPropertyChangedObject
{
    private int _value;

    public int Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }

    private string _text;

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    private bool _isHoliday;

    public bool IsHoliday
    {
        get => _isHoliday;
        set => SetField(ref _isHoliday, value);
    }

    private bool _inCurrentMonth;

    public bool InCurrentMonth
    {
        get => _inCurrentMonth;
        set => SetField(ref _inCurrentMonth, value);
    }

    private bool _isToday;

    public bool IsToday
    {
        get => _isToday;
        set => SetField(ref _isToday, value);
    }

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }
    
    public DateOnly Date { get; init; }
}
