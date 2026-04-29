using CommunityToolkit.Mvvm.ComponentModel;
using GAAPerform.Services;

namespace GAAPerform.ViewModels;

public partial class ReadinessViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty]
    private int readinessScore;

    [ObservableProperty]
    private string readinessLabel = string.Empty;

    [ObservableProperty]
    private string readinessDescription = string.Empty;

    [ObservableProperty]
    private double averageFeeling;

    [ObservableProperty]
    private double averageSoreness;

    [ObservableProperty]
    private int sessionsDoneThisWeek;

    [ObservableProperty]
    private int lastMatchMinutes;

    [ObservableProperty]
    private double readinessProgress; // 0.0 - 1.0 for progress bar/ring

    public ReadinessViewModel(DatabaseService db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        ReadinessScore = await _db.GetReadinessScoreAsync();
        AverageFeeling = Math.Round(await _db.GetAverageFeelingAsync(), 1);
        AverageSoreness = Math.Round(await _db.GetAverageSorenessAsync(), 1);
        ReadinessProgress = ReadinessScore / 100.0;

        var logs = await _db.GetRecentLogsAsync(7);
        SessionsDoneThisWeek = logs.Count;
        LastMatchMinutes = logs.FirstOrDefault(l => l.MatchMinutes > 0)?.MatchMinutes ?? 0;

        (ReadinessLabel, ReadinessDescription) = ReadinessScore switch
        {
            >= 80 => ("Great readiness", "You're in top shape — push hard this week."),
            >= 65 => ("Good readiness", "Body feeling solid. Stay consistent."),
            >= 50 => ("Moderate readiness", "Consider a lighter session today."),
            _ => ("Low readiness", "High burnout risk — prioritise recovery.")
        };
    }
}
