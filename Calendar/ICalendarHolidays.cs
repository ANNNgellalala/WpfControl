namespace Calendar;

public interface ICalendarHolidays
{
    /// <summary>
    /// 是否为假日
    /// </summary>
    /// <returns></returns>
    bool IsHoliday(DateTime dt);

    /// <summary>
    /// 是否为工作日
    /// </summary>
    /// <returns></returns>
    bool IsWorkday(DateTime dt);
}