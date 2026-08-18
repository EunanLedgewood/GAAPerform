using CommunityToolkit.Mvvm.ComponentModel;
using GAAPerform.Auth;
using GAAPerform.Models;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class SessionDetailViewModel : ObservableObject
{
    private readonly SessionLibraryService _library;
    private readonly DatabaseService _db;
    private readonly ExerciseCacheService _exerciseCache;

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

    public SessionDetailViewModel(
        SessionLibraryService library,
        DatabaseService db,
        ExerciseCacheService exerciseCache)
    {
        _library = library;
        _db = db;
        _exerciseCache = exerciseCache;
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

        // Try to get exercises from Firebase first
        try
        {
            var sessionTypeName = day.Type.ToString();
            var positionName = profile.Position.ToString();
            var firebaseExercises = await _exerciseCache
                .GetExercisesForSessionAsync(sessionTypeName, positionName);

            System.Diagnostics.Debug.WriteLine($"Firebase exercises count: {firebaseExercises.Count}");
            foreach (var ex in firebaseExercises)
                System.Diagnostics.Debug.WriteLine($"Firebase exercise: {ex.Name} VideoUrl: {ex.VideoUrl}");

            if (firebaseExercises.Any())
            {
                var mapped = firebaseExercises.Select(e => new Exercise
                {
                    Name = e.Name,
                    Sets = e.DefaultSets,
                    Reps = e.DefaultReps,
                    Duration = e.DefaultDuration,
                    Notes = e.Notes,
                    VideoUrl = e.VideoUrl
                }).ToList();

                Exercises = new ObservableCollection<Exercise>(mapped);
                HasExercises = true;
            }
            else
            {
                // Fall back to hardcoded library
                Exercises = new ObservableCollection<Exercise>(detail.Exercises);
                HasExercises = detail.Exercises.Any();
            }
        }
        catch
        {
            Exercises = new ObservableCollection<Exercise>(detail.Exercises);
            HasExercises = detail.Exercises.Any();
        }
        CoachNotes = new ObservableCollection<string>(detail.CoachNotes);
    }
}