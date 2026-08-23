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
    public TrainingDay? CompletedDay { get; private set; }

    [ObservableProperty] private string sessionTitle = string.Empty;
    [ObservableProperty] private SessionType sessionType;
    [ObservableProperty] private string timerDisplay = "00:00:00";
    [ObservableProperty] private bool isRunning = false;
    [ObservableProperty] private bool isFinished = false;
    [ObservableProperty] private bool isNotFinished = true;
    [ObservableProperty] private string playerComment = string.Empty;
    [ObservableProperty] private string sessionDescription = string.Empty;
    [ObservableProperty] private string sessionDuration = string.Empty;
    [ObservableProperty] private string sessionIntensity = string.Empty;
    [ObservableProperty] private ObservableCollection<CompletedExercise> previewExercises = new();
    [ObservableProperty] private bool isCompleting = false;

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
        CompletedDay = day;
        SessionTitle = detail.Title;
        SessionDescription = detail.Description;
        SessionDuration = detail.Duration;
        SessionIntensity = detail.Intensity;
        SessionType = day.Type;

        _allSets = GroupExercisesIntoSets(detail.Exercises);
        TotalSets = _allSets.Count;
        CurrentSetIndex = 0;

        // Preview exercises for overview screen
        PreviewExercises = new ObservableCollection<CompletedExercise>(
            _allSets.SelectMany(s => s.Exercises).Take(10).ToList());

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
                new CompletedExercise
                {
                    Name = "Custom exercise",
                    CoachNotes = "Add your own exercises",
                    Sets = new ObservableCollection<ExerciseSet> { new ExerciseSet() }
                }
            }
            });
            return sets;
        }

        var completedExercises = exercises.Select(e => new CompletedExercise
        {
            Name = e.Name,
            CoachNotes = e.Notes,
            VideoUrl = e.VideoUrl,
            Sets = new ObservableCollection<ExerciseSet> { new ExerciseSet { Reps = e.Reps } }
        }).ToList();

        foreach (var ex in completedExercises)
            System.Diagnostics.Debug.WriteLine($"Exercise: {ex.Name} VideoUrl: {ex.VideoUrl}");

        sets.Add(new CompletedSet
        {
            SetNumber = 1,
            Exercises = completedExercises
        });

        return sets;
    }

    [RelayCommand]
    private void AddSet(CompletedExercise exercise)
    {
        var lastSet = exercise.Sets.LastOrDefault();
        exercise.Sets.Add(new ExerciseSet
        {
            Weight = lastSet?.Weight ?? string.Empty,
            Reps = lastSet?.Reps ?? string.Empty,
            Time = lastSet?.Time ?? string.Empty,
            Difficulty = lastSet?.Difficulty ?? string.Empty,
            CustomField1 = lastSet?.CustomField1 ?? string.Empty,
            CustomField2 = lastSet?.CustomField2 ?? string.Empty
        });
    }

    [RelayCommand]
    private void RemoveSet(CompletedExercise exercise)
    {
        if (exercise.Sets.Count <= 1) return;
        exercise.Sets.RemoveAt(exercise.Sets.Count - 1);
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

    [RelayCommand]
    public void FinishSession()
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
        if (IsCompleting) return;
        IsCompleting = true;
        System.Diagnostics.Debug.WriteLine("CompleteSessionAsync called");
        try
        {
            if (_allSets.Any() && CurrentSetIndex < _allSets.Count)
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
            System.Diagnostics.Debug.WriteLine("Session saved to DB");

            await _db.SaveLogAsync(new Models.SessionLog
            {
                Date = DateTime.Now,
                FeelingScore = 3,
                SorenessScore = 2,
                SessionType = SessionType
            });

            // Only schedule if enabled
            if (Preferences.Get("post_session_reminder_enabled", true))
            {
                var notificationService = IPlatformApplication.Current!.Services
                    .GetRequiredService<NotificationService>();
                notificationService.SchedulePostSessionReminder();
            }

            if (_day is not null)
            {
                var key = $"completed_{_day.Date.Date:yyyy-MM-dd}";
                Preferences.Set(key, true);
                System.Diagnostics.Debug.WriteLine($"Completion saved: {key}");
            }

            if (!string.IsNullOrEmpty(PlayerComment))
            {
                try
                {
                    var auth = IPlatformApplication.Current!.Services
                        .GetRequiredService<GAAPerform.Auth.FirebaseAuthService>();
                    var firestore = IPlatformApplication.Current.Services
                        .GetRequiredService<GAAPerform.Auth.FirestoreService>();

                    if (auth.IsLoggedIn && auth.CurrentUserEmail is not null)
                    {
                        var token = await auth.GetTokenAsync();
                        await firestore.SavePlayerSessionResultAsync(
                            auth.CurrentUserEmail,
                            SessionTitle,
                            DateTime.Now,
                            _elapsedSeconds,
                            PlayerComment,
                            token);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Firebase save error: {ex.Message}");
                }
            }

            var page = Application.Current?.Windows[0].Page;
            System.Diagnostics.Debug.WriteLine($"Page type: {page?.GetType().Name}");

            if (page is AppShell shell)
            {
                await shell.Navigation.PopToRootAsync();
                System.Diagnostics.Debug.WriteLine("Shell PopToRootAsync done");
            }
            else if (page is NavigationPage navPage)
            {
                await navPage.PopToRootAsync();
                System.Diagnostics.Debug.WriteLine("NavPage PopToRootAsync done");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            IsCompleting = false;
        }
    }


    [RelayCommand]
    private void ChangeFieldType(CompletedExercise exercise)
    {
        // Cycle through field types
        exercise.FieldType = exercise.FieldType switch
        {
            ExerciseFieldType.WeightsAndReps => ExerciseFieldType.TimeAndDifficulty,
            ExerciseFieldType.TimeAndDifficulty => ExerciseFieldType.RepsOnly,
            ExerciseFieldType.RepsOnly => ExerciseFieldType.Custom,
            _ => ExerciseFieldType.WeightsAndReps
        };

        RefreshExercise(exercise);
    }

    [RelayCommand]
    private async Task WatchExerciseVideo(CompletedExercise exercise)
    {
        var url = exercise.EffectiveVideoUrl;
        if (string.IsNullOrEmpty(url)) return;

        // Convert YouTube watch URL to embed URL to prevent YouTube app interception
        if (url.Contains("youtube.com/watch?v="))
        {
            var videoId = url.Split("v=")[1].Split("&")[0];
            url = $"https://www.youtube-nocookie.com/embed/{videoId}";
        }
        else if (url.Contains("youtu.be/"))
        {
            var videoId = url.Split("youtu.be/")[1].Split("?")[0];
            url = $"https://www.youtube-nocookie.com/embed/{videoId}";
        }

        await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
    }

    public void RefreshExercise(CompletedExercise exercise)
    {
        var index = CurrentExercises.IndexOf(exercise);
        if (index >= 0)
        {
            var refreshed = new CompletedExercise
            {
                Name = exercise.Name,
                CoachNotes = exercise.CoachNotes,
                IsCompleted = exercise.IsCompleted,
                FieldType = exercise.FieldType,
                CustomLabel1 = exercise.CustomLabel1,
                CustomLabel2 = exercise.CustomLabel2,
                Sets = exercise.Sets
            };
            CurrentExercises.RemoveAt(index);
            CurrentExercises.Insert(index, refreshed);
        }
    }

    public void OnWeightChanged(CompletedExercise exercise, int setIndex, string value)
    {
        // Auto-fill subsequent empty sets
        for (int i = setIndex + 1; i < exercise.Sets.Count; i++)
        {
            if (string.IsNullOrEmpty(exercise.Sets[i].Weight))
                exercise.Sets[i].Weight = value;
        }
    }

    public void OnRepsChanged(CompletedExercise exercise, int setIndex, string value)
    {
        for (int i = setIndex + 1; i < exercise.Sets.Count; i++)
        {
            if (string.IsNullOrEmpty(exercise.Sets[i].Reps))
                exercise.Sets[i].Reps = value;
        }
    }

    public void OnTimeChanged(CompletedExercise exercise, int setIndex, string value)
    {
        for (int i = setIndex + 1; i < exercise.Sets.Count; i++)
        {
            if (string.IsNullOrEmpty(exercise.Sets[i].Time))
                exercise.Sets[i].Time = value;
        }
    }

    public void OnDifficultyChanged(CompletedExercise exercise, int setIndex, string value)
    {
        for (int i = setIndex + 1; i < exercise.Sets.Count; i++)
        {
            if (string.IsNullOrEmpty(exercise.Sets[i].Difficulty))
                exercise.Sets[i].Difficulty = value;
        }
    }

    public void Cleanup()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}