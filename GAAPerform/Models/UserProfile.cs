namespace GAAPerform.Models;

public enum Position
{
    Goalkeeper,
    Fullback,
    Midfielder,
    Forward
}

public enum SeasonMode
{
    InSeason,
    PreSeason,
    OffSeason
}

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Position Position { get; set; } = Position.Midfielder;
    public SeasonMode SeasonMode { get; set; } = SeasonMode.InSeason;
    public DateTime? NextMatchDate { get; set; }
}
