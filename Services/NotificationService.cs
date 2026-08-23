using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace GAAPerform.Services;

public class NotificationService
{
    private const int SessionReminderNotificationId = 1001;
    private const int DailyReminderNotificationId = 1002;
    private const int CoachAssignmentNotificationId = 1003;

    public void RequestPermissionAsync()
    {
        LocalNotificationCenter.Current.RequestNotificationPermission();
    }

    public void SchedulePostSessionReminder()
    {
        var notification = new NotificationRequest
        {
            NotificationId = SessionReminderNotificationId,
            Title = "GAA Perform",
            Description = "Don't forget to log how you're feeling after today's session!",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddHours(2),
                RepeatType = NotificationRepeat.No
            },
            Android = new AndroidOptions
            {
                ChannelId = "gaa_perform_channel"
            }
        };

        LocalNotificationCenter.Current.Show(notification);
    }

    public void ScheduleDailyReminder(TimeSpan notifyAt)
    {
        var notifyTime = DateTime.Today.Add(notifyAt);
        if (notifyTime < DateTime.Now)
            notifyTime = notifyTime.AddDays(1);

        var notification = new NotificationRequest
        {
            NotificationId = DailyReminderNotificationId,
            Title = "GAA Perform",
            Description = "Time to check your training plan for today 💪",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyTime,
                RepeatType = NotificationRepeat.Daily
            },
            Android = new AndroidOptions
            {
                ChannelId = "gaa_perform_channel"
            }
        };

        LocalNotificationCenter.Current.Show(notification);
    }

    public void SendCoachAssignmentNotification(string sessionTitle, DateTime sessionDate)
    {
        var notification = new NotificationRequest
        {
            NotificationId = CoachAssignmentNotificationId,
            Title = "New session assigned!",
            Description = $"Your coach has assigned: {sessionTitle} on {sessionDate:dd MMM}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(5),
                RepeatType = NotificationRepeat.No
            },
            Android = new AndroidOptions
            {
                ChannelId = "gaa_perform_channel"
            }
        };

        LocalNotificationCenter.Current.Show(notification);
    }

    public void CancelAll()
    {
        LocalNotificationCenter.Current.CancelAll();
    }
}