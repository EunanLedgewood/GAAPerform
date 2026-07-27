using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class ActiveSessionViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private System.Timers.Timer? _timer;
    private int _elapsedSeconds = 0;

    [ObservableProperty] private string sessionTitle = string.Empty;
    [ObservableProperty] private SessionType sessionType;
    [ObservableProperty] private string timerDisplay = "00:00:00";
    [ObservableProperty] private bool isRunning = false;
    [ObservableProperty] private bool isFinished = false;
    [ObservableProperty] private bool isNotFinished = true;
    [ObservableProperty] private string playerComment = string.Empty;

    // Sets navigation
    [ObservableProperty] private int currentSetIndex = 0;
    [ObservableProperty] private int totalSets = 0;
    [ObservableProperty] private string setLabel = string.Empty;
    [ObservableProperty] private bool canGoNext = false;
    [ObservableProperty] private bool canGoPrevious = false;
    [ObservableProperty] private ObservableCollection<CompletedExercise> currentExercises = new();

    private List<CompletedSet> _allSets = new();
    private TrainingDay? _day;

    public ActiveSessionViewModel(DatabaseService db)
    {
        _db = db;
    }

    public void LoadSession(TrainingDay day, SessionDetail detail)
    {
        _day = day;
        SessionTitle = detail.Title;
        SessionType = day.Type;

        // Group exercises into sets of 3 (or use natural grouping)
        _allSets = GroupExercisesIntoSets(detail.Exercises);
        TotalSets = _allSets.Count;
        CurrentSetIndex = 0;
        UpdateCurrentSet();
    }

    private List<CompletedSet> GroupExercisesIntoSets(List<Exercise> exercises)
    {
        var sets = new List<CompletedSet>();
        if (!exercises.Any())
        {
            sets.Add(new CompletedSet
            {
                SetNumber = 1,
                Exercises = new List<CompletedExercise>
                {
                    new CompletedExercise { Name = "Custom exercise", CoachNotes = "Add your own exercises" }
                }
            });
            return sets;
        }

        // Group every 3 exercises into a set
        int setNumber = 1;
        for (int i = 0; i < exercises.Count; i += 3)
        {
            var setExercises = exercises.Skip(i).Take(3).Select(e => new CompletedExercise
            {
                Name = e.Name,
                CoachNotes = e.Notes,
                ActualWeight = string.Empty,
                ActualReps = e.Reps
            }).ToList();

            sets.Add(new CompletedSet
            {
                SetNumber = setNumber++,
                Exercises = setExercises
            });
        }
        return sets;
    }

    private void UpdateCurrentSet()
    {
        if (!_allSets.Any()) return;

        var currentSet = _allSets[CurrentSetIndex];
        CurrentExercises = new ObservableCollection<CompletedExercise>(currentSet.Exercises);
        SetLabel = $"Set {CurrentSetIndex + 1} of {TotalSets}";
        CanGoPrevious = CurrentSetIndex > 0;
        CanGoNext = CurrentSetIndex < TotalSets - 1;
    }

    [RelayCommand]
    private void StartSession()
    {
        IsRunning = true;
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (s, e) =>
        {
            _elapsedSeconds++;
            var ts = TimeSpan.FromSeconds(_elapsedSeconds);
            TimerDisplay = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        };
        _timer.Start();
    }

    [RelayCommand]
    private void NextSet()
    {
        // Save current exercises back to the set
        _allSets[CurrentSetIndex].Exercises = CurrentExercises.ToList();

        if (CurrentSetIndex < TotalSets - 1)
        {
            CurrentSetIndex++;
            UpdateCurrentSet();
        }
        else
        {
            // Last set — finish
            FinishSession();
        }
    }

    [RelayCommand]
    private void PreviousSet()
    {
        _allSets[CurrentSetIndex].Exercises = CurrentExercises.ToList();
        if (CurrentSetIndex > 0)
        {
            CurrentSetIndex--;
            UpdateCurrentSet();
        }
    }

    [RelayCommand]
    private void ToggleExerciseComplete(CompletedExercise exercise)
    {
        exercise.IsCompleted = !exercise.IsCompleted;
        var index = CurrentExercises.IndexOf(exercise);
        if (index >= 0)
        {
            CurrentExercises.RemoveAt(index);
            CurrentExercises.Insert(index, exercise);
        }
    }

    private void FinishSession()
    {
        _timer?.Stop();
        _timer?.Dispose();
        IsRunning = false;
        IsFinished = true;
        IsNotFinished = false;
    }

    [RelayCommand]
    private async Task CompleteSessionAsync()
    {
        _allSets[CurrentSetIndex].Exercises = CurrentExercises.ToList();

        var completed = new CompletedSession
        {
            Date = DateTime.Now,
            SessionTitle = SessionTitle,
            SessionType = SessionType,
            DurationSeconds = _elapsedSeconds,
            PlayerComment = PlayerComment,
            Sets = _allSets
        };

        await _db.SaveCompletedSessionAsync(completed);

        // Also log it as a session log for readiness tracking
        await _db.SaveLogAsync(new Models.SessionLog
        {
            Date = DateTime.Now,
            FeelingScore = 3,
            SorenessScore = 2,
            SessionType = SessionType
        });
    }

    public void Cleanup()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}