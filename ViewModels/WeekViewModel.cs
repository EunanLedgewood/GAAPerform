using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using GAAPerform.Views;
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
        var matchDate = profile.NextMatchDate ?? GetNextSunday();
        var monday = matchDate.AddDays(-(int)matchDate.DayOfWeek + 1);
        WeekLabel = $"Week of {monday:dd MMM}";
        HasMatchThisWeek = true;

        var plan = _planService.GenerateWeekPlan(matchDate, profile.Position, profile.SeasonMode);

        var today = DateTime.Today;
        bool foundMissed = false;
        for (int i = 0; i < plan.Count; i++)
        {
            if (plan[i].Date.Date < today && !plan[i].IsCompleted &&
                plan[i].Type != SessionType.Rest && plan[i].Type != SessionType.Match)
            {
                plan = _planService.AdjustForMissedSession(plan, i);
                MissedSessionAlert = $"Missed {plan[i].DayName}'s session — next session adjusted.";
                ShowAlert = true;
                foundMissed = true;
                break;
            }
        }

        if (!foundMissed) ShowAlert = false;
        WeekDays = new ObservableCollection<TrainingDay>(plan);
    }

    [RelayCommand]
    private async Task ToggleSession(TrainingDay day)
    {
        var detailVm = IPlatformApplication.Current!.Services
            .GetRequiredService<SessionDetailViewModel>();
        var detailPage = new SessionDetailPage(detailVm, day);
        await Shell.Current.Navigation.PushAsync(detailPage);
    }

    private static DateTime GetNextSunday()
    {
        var today = DateTime.Today;
        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
        return daysUntilSunday == 0 ? today.AddDays(7) : today.AddDays(daysUntilSunday);
    }
}