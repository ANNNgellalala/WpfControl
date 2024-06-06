namespace Calendar;

internal class DefaultCalendarFestivals : ICalendarFestivals, ICalendarHolidays
{
    private static readonly Dictionary<string, string> Festivals = new() {
        {"0101", "元旦"},
        {"0214", "情人节"},
        {"0308", "妇女节"},
        {"0312", "植树节"},
        {"0401", "愚人节"},
        {"0501", "劳动节"},
        {"0504", "青年节"},
        {"0601", "儿童节"},
        {"0701", "建党节"},
        {"0801", "建军节"},
        {"1001", "国庆节"},
        {"1225", "圣诞节"}
    };

    private static readonly Dictionary<string, string> LunarFestivals = new() {
        {"0101", "春节"},
        {"0115", "元宵节"},
        {"0505", "端午节"},
        {"0815", "中秋节"},
        {"0909", "重阳节"},
        {"1208", "腊八节"},
        {"1230", "除夕"},
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string? GetFestival(DateTime dt)
    {
        string? ret = null;
        var key = $"{dt:MMdd}";
        var (_, month, day) = dt.ToLunarDateTime();
        var lunarKey = $"{month:00}{day:00}";
        if (LunarFestivals.TryGetValue(lunarKey, out var v1))
            ret = v1;
        else if (Festivals.TryGetValue(key, out var v2))
            ret = v2;
        return ret;
    }

    public bool IsHoliday(
        DateTime dt)
    {
        return dt.DayOfWeek switch
        {
            DayOfWeek.Saturday => true,
            DayOfWeek.Sunday => true,
            _ => false
        };
    }

    public bool IsWorkday(
        DateTime dt)
    {
        return !IsHoliday(dt);
    }
}