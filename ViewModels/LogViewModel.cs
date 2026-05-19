using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;

namespace GAAPerform.ViewModels;

public partial class LogViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private DateTime sessionDate = DateTime.Today;
    [ObservableProperty] private int feelingScore = 3;
    [ObservableProperty] private int sorenessScore = 2;
    [ObservableProperty] private int matchMinutes = 0;
    [ObservableProperty] private bool isSubmitted;
    [ObservableProperty] private bool isNotSubmitted = true;
    [ObservableProperty] private string submitMessage = string.Empty;

    // Session type flags
    [ObservableProperty] private bool isMatch = false;
    [ObservableProperty] private bool isGymSession = false;
    [ObservableProperty] private bool isPitchSession = false;
    [ObservableProperty] private bool isRecovery = false;
    [ObservableProperty] private SessionType selectedSessionType = SessionType.Field;

    // Show match minutes only for match sessions
    [ObservableProperty] private bool showMatchMinutes = false;

    public LogViewModel(DatabaseService db)
    {
        _db = db;
    }

    [RelayCommand]
    private void SelectSessionType(string type)
    {
        SelectedSessionType = type switch
        {
            "Match" => SessionType.Match,
            "Gym" => SessionType.Strength,
            "Recovery" => SessionType.Recovery,
            _ => SessionType.Field
        };

        IsMatch = SelectedSessionType == SessionType.Match;
        IsGymSession = SelectedSessionType == SessionType.Strength;
        IsPitchSession = SelectedSessionType == SessionType.Field;
        IsRecovery = SelectedSessionType == SessionType.Recovery;
        ShowMatchMinutes = SelectedSessionType == SessionType.Match;
    }

    [RelayCommand]
    private void SetFeeling(string score) => FeelingScore = int.Parse(score);

    [RelayCommand]
    private void SetSoreness(string score) => SorenessScore = int.Parse(score);

    [RelayCommand]
    private void SetMatchMinutes(string mins) => MatchMinutes = int.Parse(mins);

    [RelayCommand]
    private async Task SubmitLogAsync()
    {
        var log = new SessionLog
        {
            Date = SessionDate,
            FeelingScore = FeelingScore,
            SorenessScore = SorenessScore,
            MatchMinutes = IsMatch ? MatchMinutes : 0,
            SessionType = SelectedSessionType
        };

        await _db.SaveLogAsync(log);

        // Also create a calendar event so it shows on the calendar and week view
        var eventType = SelectedSessionType switch
        {
            SessionType.Match => EventType.Match,
            SessionType.Strength => EventType.GymSession,
            SessionType.Recovery => EventType.Recovery,
            _ => EventType.Training
        };

        var title = SelectedSessionType switch
        {
            SessionType.Match => "Match",
            SessionType.Strength => "Gym Session",
            SessionType.Recovery => "Recovery",
            _ => "Pitch Session"
        };

        var calEvent = new CalendarEvent
        {
            Date = SessionDate,
            EventType = eventType,
            Title = title,
            IsCompleted = true
        };

        await _db.SaveEventAsync(calEvent);

        SubmitMessage = FeelingScore <= 2 || SorenessScore >= 4
            ? "Session logged. Consider a lighter session tomorrow."
            : "Session logged. Readiness score updated. Keep it up!";

        IsSubmitted = true;
        IsNotSubmitted = false;
    }

    [RelayCommand]
    private void Reset()
    {
        SessionDate = DateTime.Today;
        FeelingScore = 3;
        SorenessScore = 2;
        MatchMinutes = 0;
        IsSubmitted = false;
        IsNotSubmitted = true;
        IsMatch = false;
        IsGymSession = false;
        IsPitchSession = false;
        IsRecovery = false;
        ShowMatchMinutes = false;
        SubmitMessage = string.Empty;
    }
}