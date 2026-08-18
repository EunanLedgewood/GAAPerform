namespace GAAPerform.Models;

public class CachedExercise
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement]
    public int Id { get; set; }
    public string ExerciseId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DefaultSets { get; set; } = string.Empty;
    public string DefaultReps { get; set; } = string.Empty;
    public string DefaultDuration { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string PositionsJson { get; set; } = "[]";
    public string SessionTypesJson { get; set; } = "[]";
}