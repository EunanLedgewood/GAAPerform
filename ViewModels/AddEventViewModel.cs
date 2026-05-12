using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using static System.Net.WebRequestMethods;

namespace GAAPerform.ViewModels;

public partial class AddEventViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private DateTime eventDate = DateTime.Today;
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private string location = string.Empty;
    [ObservableProperty] private EventType selectedEventType = EventType.Match;

    // Event type flags
    [ObservableProperty] private bool isMatch = true;
    [ObservableProperty] private bool isTraining = false;
    [ObservableProperty] private bool isGymSession = false;
    [ObservableProperty] private bool isRecovery = false;
    [ObservableProperty] private bool isOther = false;

    public AddEventViewModel(DatabaseService db)
    {
        _db = db;
    }

    public void SetDate(DateTime date)
    {
        EventDate = date;
    }

    [RelayCommand]
    private void SelectEventType(string type)
    {
        SelectedEventType = type switch
        {
            "Training" => EventType.Training,
            "GymSession" => EventType.GymSession,
            "Recovery" => EventType.Recovery,
            "Other" => EventType.Other,
            _ => EventType.Match
        };

        IsMatch = SelectedEventType == EventType.Match;
        IsTraining = SelectedEventType == EventType.Training;
        IsGymSession = SelectedEventType == EventType.GymSession;
        IsRecovery = SelectedEventType == EventType.Recovery;
        IsOther = SelectedEventType == EventType.Other;

        // Auto fill title if empty
        if (string.IsNullOrWhiteSpace(Title))
        {
            Title = SelectedEventType switch
            {
                EventType.Match => "Match",
                EventType.Training => "Field Training",
                EventType.GymSession => "Gym Session",
                EventType.Recovery => "Recovery",
                _ => string.Empty
            };
        }
    }

    [RelayCommand]
    private async Task SaveEventAsync()
    {
        if (string.IsNullOrWhiteSpace(Title)) return;

        var calEvent = new CalendarEvent
        {
            Date = EventDate,
            EventType = SelectedEventType,
            Title = Title.Trim(),
            Notes = Notes.Trim(),
            Location = Location.Trim()
        };

        await _db.SaveEventAsync(calEvent);
    }
}