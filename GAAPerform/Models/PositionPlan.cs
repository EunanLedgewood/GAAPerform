namespace GAAPerform.Models;

public class PositionPlan
{
    public Position Position { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Icon { get; set; } = string.Empty;
    public string FocusSummary { get; set; } = string.Empty;
}
