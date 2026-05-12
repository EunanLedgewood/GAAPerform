namespace GAAPerform.Models;

public enum EventType
{
    Match,
    Training,
    GymSession,
    Recovery,
    Rest,
    Other
}

public class CalendarEvent
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public EventType EventType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsCoachAssigned { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
}