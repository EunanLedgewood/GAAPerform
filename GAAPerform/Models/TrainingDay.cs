namespace GAAPerform.Models;

public enum SessionType
{
    Match,
    Strength,
    Field,
    Recovery,
    Rest,
    Activation
}

public class TrainingDay
{
    public int Id { get; set; }
    public string DayName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;
    public SessionType Type { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsMissed { get; set; }
    public DateTime Date { get; set; }
}
