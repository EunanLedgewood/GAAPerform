namespace GAAPerform.Models;

public class SessionLog
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int FeelingScore { get; set; }   // 1-5
    public int SorenessScore { get; set; }  // 1-5
    public int MatchMinutes { get; set; }
    public SessionType SessionType { get; set; }
    public string Notes { get; set; } = string.Empty;
}
