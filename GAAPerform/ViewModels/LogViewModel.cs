using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;

namespace GAAPerform.ViewModels;

public partial class LogViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty]
    private int feelingScore = 3;

    [ObservableProperty]
    private int sorenessScore = 2;

    [ObservableProperty]
    private int matchMinutes = 0;

    [ObservableProperty]
    private bool isSubmitted;

    [ObservableProperty]
    private string submitMessage = string.Empty;

    public int[] MinuteOptions { get; } = { 0, 15, 30, 45, 60, 70 };
    public int[] ScoreOptions { get; } = { 1, 2, 3, 4, 5 };

    public LogViewModel(DatabaseService db)
    {
        _db = db;
    }

    [RelayCommand]
    private void SetFeeling(int score) => FeelingScore = score;

    [RelayCommand]
    private void SetSoreness(int score) => SorenessScore = score;

    [RelayCommand]
    private void SetMatchMinutes(int mins) => MatchMinutes = mins;

    [RelayCommand]
    private async Task SubmitLogAsync()
    {
        var log = new SessionLog
        {
            Date = DateTime.Now,
            FeelingScore = FeelingScore,
            SorenessScore = SorenessScore,
            MatchMinutes = MatchMinutes,
            SessionType = SessionType.Field
        };

        await _db.SaveLogAsync(log);

        SubmitMessage = FeelingScore <= 2 || SorenessScore >= 4
            ? "Session logged. Consider a lighter session tomorrow — your body needs recovery."
            : "Session logged. Readiness score updated. Keep it up!";

        IsSubmitted = true;
    }

    [RelayCommand]
    private void Reset()
    {
        FeelingScore = 3;
        SorenessScore = 2;
        MatchMinutes = 0;
        IsSubmitted = false;
        SubmitMessage = string.Empty;
    }
}
