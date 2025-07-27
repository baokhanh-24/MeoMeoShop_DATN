namespace MeoMeo.Shared.DTOs;

public class AppSettings
{
    public FcmNotificationSetting FcmNotificationSetting { get; init; }
    public NotificationSetting NotificationSetting { get; init; }
}
public class FcmNotificationSetting
{
    public string SenderId { get; init; }
    public string ServerKey { get; init; }
}

public class NotificationSetting
{
    public TimeSpan DailyHour { get; init; }
    public TimeSpan DeleteTokenHour { get; init; }
    public int BeforeHour { get; init; }
    public int NumberYearSendNotification { get; init; }
}