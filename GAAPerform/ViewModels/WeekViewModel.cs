using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class WeekViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly TrainingPlanService _planService;

    [ObservableProperty]
    private ObservableCollection<TrainingDay> weekDays = new();

    [ObservableProperty]
    private string weekLabel = string.Empty;

    [ObservableProperty]
    private bool hasMatchThisWeek;

    [ObservableProperty]
    private string? missedSessionAlert;

    [ObservableProperty]
    private bool showAlert;

    public WeekViewModel(DatabaseService db, TrainingPlanService planService)
    {
        _db = db;
        _planService = planService;
    }

    public async Task LoadAsync()
    {
        var profile = await _db.GetProfileAsync();

        // Default next match to next Sunday if not set
        var matchDate = profile.NextMatchDate ?? GetNextSunday();
        var monday = matchDate.AddDays(-(int)matchDate.DayOfWeek + 1);
        WeekLabel = $"Week of {monday:dd MMM}";
        HasMatchThisWeek = true;

        var plan = _planService.GenerateWeekPlan(matchDate, profile.Position, profile.SeasonMode);

        // Check for any missed sessions (past days not completed)
        var today = DateTime.Today;
        bool foundMissed = false;
        for (int i = 0; i < plan.Count; i++)
        {
            if (plan[i].Date.Date < today && !plan[i].IsCompleted && plan[i].Type != SessionType.Rest && plan[i].Type != SessionType.Match)
            {
                plan = _planService.AdjustForMissedSession(plan, i);
                MissedSessionAlert = $"Missed {plan[i].DayName}'s {GetOriginalLabel(plan[i].Type)} — next session adjusted.";
                ShowAlert = true;
                foundMissed = true;
                break;
            }
        }

        if (!foundMissed) ShowAlert = false;

        WeekDays = new ObservableCollection<TrainingDay>(plan);
    }

    [RelayCommand]
    private async Task ToggleSessionAsync(TrainingDay day)
    {
        if (day.Type == SessionType.Rest || day.Type == SessionType.Match) return;
        day.IsCompleted = !day.IsCompleted;
        // Refresh collection to trigger UI update
        var index = WeekDays.IndexOf(day);
        if (index >= 0)
        {
            WeekDays.RemoveAt(index);
            WeekDays.Insert(index, day);
        }
    }

    private static DateTime GetNextSunday()
    {
        var today = DateTime.Today;
        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
        return daysUntilSunday == 0 ? today.AddDays(7) : today.AddDays(daysUntilSunday);
    }

    private static string GetOriginalLabel(SessionType type) => type switch
    {
        SessionType.Strength => "strength session",
        SessionType.Field => "field session",
        SessionType.Recovery => "recovery session",
        _ => "session"
    };
}
