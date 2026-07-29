namespace GAAPerform.Models;

public class CompletedSet
{
    public int SetNumber { get; set; }
    public List<CompletedExercise> Exercises { get; set; } = new();
}

public class ExerciseSet
{
    public string Weight { get; set; } = string.Empty;
    public string Reps { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
}

public class CompletedExercise
{
    public string Name { get; set; } = string.Empty;
    public string CoachNotes { get; set; } = string.Empty;
    public string ActualWeight { get; set; } = string.Empty;
    public string ActualReps { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public List<ExerciseSet> Sets { get; set; } = new() { new ExerciseSet() };
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

    [SQLite.Ignore]
    public List<CompletedSet> Sets { get; set; } = new();

    // Store sets as JSON string in SQLite
    public string SetsJson { get; set; } = string.Empty;

    [SQLite.Ignore]
    public string DurationFormatted =>
        $"{DurationSeconds / 3600:00}:{(DurationSeconds % 3600) / 60:00}:{DurationSeconds % 60:00}";

    [SQLite.Ignore]
    public bool IsExpanded { get; set; } = false;

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

