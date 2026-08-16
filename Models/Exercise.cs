namespace GAAPerform.Models;

public class Exercise
{
    public string Name { get; set; } = string.Empty;
    public string Sets { get; set; } = string.Empty;
    public string Reps { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
}

public class SessionDetail
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Intensity { get; set; } = string.Empty;
    public SessionType Type { get; set; }
    public List<Exercise> Exercises { get; set; } = new();
    public List<string> CoachNotes { get; set; } = new();
}