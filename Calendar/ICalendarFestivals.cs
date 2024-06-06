namespace Calendar;

/// <summary>
/// 节日接口
/// </summary>
public interface ICalendarFestivals
{
    /// <summary>
    /// 获得 节日键值对
    /// </summary>
    /// <returns></returns>
    string? GetFestival(DateTime dt);
}