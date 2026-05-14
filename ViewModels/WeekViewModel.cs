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

    [ObservableProperty]
    private TrainingDay? selectedDay;

    public WeekViewModel(DatabaseService db, TrainingPlanService planService)
    {
        _db = db;
        _planService = planService;
    }

    public async Task LoadAsync()
    {
        var profile = await _db.GetProfileAsync();

        // Find the current week's Monday
        var today = DateTime.Today;
        var monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
            monday = today.AddDays(-6);

        var sunday = monday.AddDays(6);
        WeekLabel = $"Week of {monday:dd MMM}";

        // Look for a match in the calendar this week
        var weekEvents = await _db.GetEventsForWeekAsync(monday, sunday);
        var matchEvent = weekEvents.FirstOrDefault(e => e.EventType == Models.EventType.Match);

        DateTime matchDate;
        bool hasCalendarMatch = matchEvent is not null;

        if (hasCalendarMatch)
        {
            matchDate = matchEvent!.Date;
            HasMatchThisWeek = true;
        }
        else
        {
            // Fall back to next Sunday if no match in calendar
            matchDate = sunday;
            HasMatchThisWeek = false;
        }

        var plan = _planService.GenerateWeekPlan(matchDate, profile.Position, profile.SeasonMode);

        // Overlay any calendar events onto the plan
        foreach (var day in plan)
        {
            var dayEvents = weekEvents.Where(e => e.Date.Date == day.Date.Date).ToList();
            if (dayEvents.Any())
            {
                var firstEvent = dayEvents.First();
                if (firstEvent.EventType != Models.EventType.Match)
                {
                    day.Label = firstEvent.Title;
                    day.Meta = firstEvent.Notes.Length > 0 ? firstEvent.Notes : day.Meta;
                    day.Type = firstEvent.EventType switch
                    {
                        Models.EventType.Training => SessionType.Field,
                        Models.EventType.GymSession => SessionType.Strength,
                        Models.EventType.Recovery => SessionType.Recovery,
                        _ => day.Type
                    };
                }
            }
        }

        // Check for missed sessions
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
    private void ToggleSession(TrainingDay day)
    {
        SelectedDay = day;
    }

    private static DateTime GetNextSunday()
    {
        var today = DateTime.Today;
        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
        return daysUntilSunday == 0 ? today.AddDays(7) : today.AddDays(daysUntilSunday);
    }
}