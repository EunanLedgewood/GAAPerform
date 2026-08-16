using System.Collections.ObjectModel;

namespace GAAPerform.Models;

public enum ExerciseFieldType
{
    WeightsAndReps,
    TimeAndDifficulty,
    RepsOnly,
    Custom
}

public class CompletedSet
{
    public int SetNumber { get; set; }
    public List<CompletedExercise> Exercises { get; set; } = new();
}

public class ExerciseSet
{
    public string Weight { get; set; } = string.Empty;
    public string Reps { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string CustomField1 { get; set; } = string.Empty;
    public string CustomField2 { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}

public class CompletedExercise
{
    public string Name { get; set; } = string.Empty;
    public string CoachNotes { get; set; } = string.Empty;
    public string ActualWeight { get; set; } = string.Empty;
    public string ActualReps { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public ExerciseFieldType FieldType { get; set; } = ExerciseFieldType.WeightsAndReps;
    public string CustomLabel1 { get; set; } = "Field 1";
    public string CustomLabel2 { get; set; } = "Field 2";
    public ObservableCollection<ExerciseSet> Sets { get; set; } = new() { new ExerciseSet() };

    // Computed properties for display
    public bool IsWeightsAndReps => FieldType == ExerciseFieldType.WeightsAndReps;
    public bool IsTimeAndDifficulty => FieldType == ExerciseFieldType.TimeAndDifficulty;
    public bool IsRepsOnly => FieldType == ExerciseFieldType.RepsOnly;
    public bool IsCustom => FieldType == ExerciseFieldType.Custom;
}

public class CompletedSession
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public SessionType SessionType { get; set; }
    public int DurationSeconds { get; set; }
    public string PlayerComment { get; set; } = string.Empty;
    public string CoachEmail { get; set; } = string.Empty;
    public bool IsSharedWithCoach { get; set; } = false;
    public bool IsExpanded { get; set; } = false;

    [SQLite.Ignore]
    public List<CompletedSet> Sets { get; set; } = new();

    public string SetsJson { get; set; } = string.Empty;

    [SQLite.Ignore]
    public string DurationFormatted =>
        $"{DurationSeconds / 3600:00}:{(DurationSeconds % 3600) / 60:00}:{DurationSeconds % 60:00}";

    [SQLite.Ignore]
    public string SessionTypeIcon => SessionType switch
    {
        SessionType.Match => "⚽",
        SessionType.Strength => "💪",
        SessionType.Field => "🏃",
        SessionType.Recovery => "🛌",
        _ => "📋"
    };
}

