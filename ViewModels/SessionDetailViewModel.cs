using CommunityToolkit.Mvvm.ComponentModel;
using GAAPerform.Models;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class SessionDetailViewModel : ObservableObject
{
    private readonly SessionLibraryService _library;
    private readonly DatabaseService _db;

    [ObservableProperty] private string sessionTitle = string.Empty;
    [ObservableProperty] private string sessionDescription = string.Empty;
    [ObservableProperty] private string sessionDuration = string.Empty;
    [ObservableProperty] private string sessionIntensity = string.Empty;
    [ObservableProperty] private string sessionIcon = string.Empty;
    [ObservableProperty] private ObservableCollection<Exercise> exercises = new();
    [ObservableProperty] private ObservableCollection<string> coachNotes = new();
    [ObservableProperty] private bool hasCoachNotes;
    [ObservableProperty] private bool hasExercises;
    [ObservableProperty] private bool isRestDay;

    public SessionDetailViewModel(SessionLibraryService library, DatabaseService db)
    {
        _library = library;
        _db = db;
    }

    public async Task LoadAsync(TrainingDay day)
    {
        var profile = await _db.GetProfileAsync();
        var detail = _library.GetSessionDetail(day, profile.Position);

        SessionTitle = detail.Title;
        SessionDescription = detail.Description;
        SessionDuration = detail.Duration;
        SessionIntensity = detail.Intensity;
        IsRestDay = day.Type == SessionType.Rest;
        HasExercises = detail.Exercises.Any();
        HasCoachNotes = detail.CoachNotes.Any();

        SessionIcon = day.Type switch
        {
            SessionType.Match => "⚽",
            SessionType.Strength => "💪",
            SessionType.Field => "🏃",
            SessionType.Recovery => "🛌",
            SessionType.Activation => "⚡",
            _ => "😴"
        };

        Exercises = new ObservableCollection<Exercise>(detail.Exercises);
        CoachNotes = new ObservableCollection<string>(detail.CoachNotes);
    }
}