using Android.AdServices.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class WeeklyReportViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private int sessionsCompleted;
    [ObservableProperty] private int sessionsTarget = 4;
    [ObservableProperty] private double averageFeeling;
    [ObservableProperty] private double averageSoreness;
    [ObservableProperty] private int readinessScore;
    [ObservableProperty] private string readinessLabel = string.Empty;
    [ObservableProperty] private string encouragementMessage = string.Empty;
    [ObservableProperty] private string weekLabel = string.Empty;
    [ObservableProperty] private double sessionCompletionRate;
    [ObservableProperty] private ObservableCollection<DayLoad> weeklyLoad = new();
    [ObservableProperty] private bool hasData;
    [ObservableProperty] private string streakMessage = string.Empty;

    public WeeklyReportViewModel(DatabaseService db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        var today = DateTime.Today;
        var monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
            monday = today.AddDays(-6);

        WeekLabel = $"Week of {monday:dd MMM yyyy}";

        var logs = await _db.GetRecentLogsAsync(7);
        var thisWeekLogs = logs.Where(l => l.Date >= monday && l.Date <= today).ToList();

        SessionsCompleted = thisWeekLogs.Count;
        SessionCompletionRate = Math.Min((double)SessionsCompleted / SessionsTarget, 1.0);
        HasData = thisWeekLogs.Any();

        if (HasData)
        {
            AverageFeeling = Math.Round(thisWeekLogs.Average(l => l.FeelingScore), 1);
            AverageSoreness = Math.Round(thisWeekLogs.Average(l => l.SorenessScore), 1);
        }

        ReadinessScore = await _db.GetReadinessScoreAsync();
        ReadinessLabel = ReadinessScore switch
        {
            >= 80 => "Excellent week",
            >= 65 => "Good week",
            >= 50 => "Average week",
            _ => "Tough week"
        };

        EncouragementMessage = (SessionsCompleted, AverageFeeling) switch
        {
            ( >= 4, >= 4) => "Outstanding week. You're in great shape heading into the weekend.",
            ( >= 4, _) => "Strong week for sessions. Keep an eye on how your body feels.",
            ( >= 3, >= 4) => "Good week. One more session would have been perfect.",
            ( >= 3, _) => "Decent week. Try to hit your target next week.",
            ( >= 2, _) => "Below target this week. A small effort tomorrow can still make a difference.",
            _ => "Tough week. Don't stress — reset and go again next week."
        };

        // Build daily load chart data
        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        var loadData = new ObservableCollection<DayLoad>();

        for (int i = 0; i < 7; i++)
        {
            var date = monday.AddDays(i);
            var dayLog = thisWeekLogs.FirstOrDefault(l => l.Date.Date == date.Date);

            loadData.Add(new DayLoad
            {
                DayName = days[i],
                HasSession = dayLog is not null,
                FeelingScore = dayLog?.FeelingScore ?? 0,
                BarHeight = dayLog is not null ? dayLog.FeelingScore * 16 : 4
            });
        }

        WeeklyLoad = loadData;

        // Streak
        var allLogs = await _db.GetRecentLogsAsync(30);
        int streak = 0;
        var checkDate = today;
        while (allLogs.Any(l => l.Date.Date == checkDate.Date))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }
        StreakMessage = streak > 1 ? $"🔥 {streak} day streak" : string.Empty;
    }
}

public class DayLoad
{
    public string DayName { get; set; } = string.Empty;
    public bool HasSession { get; set; }
    public int FeelingScore { get; set; }
    public double BarHeight { get; set; }
}