using GAAPerform.Models;

namespace GAAPerform.Services;

public class TrainingPlanService
{
    public List<TrainingDay> GenerateWeekPlan(DateTime matchDate, Position position, SeasonMode mode)
    {
        // Build plan working backwards from match day (Sunday by default)
        var plan = new List<TrainingDay>();
        var monday = matchDate.AddDays(-(int)matchDate.DayOfWeek + 1);

        var sessionMap = GetSessionMap(position, mode);

        for (int i = 0; i < 7; i++)
        {
            var date = monday.AddDays(i);
            var isMatchDay = date.Date == matchDate.Date;

            plan.Add(new TrainingDay
            {
                DayName = date.ToString("dddd"),
                Date = date,
                Label = isMatchDay ? "Match Day" : sessionMap[i].label,
                Meta = isMatchDay ? "Match day — stay sharp" : sessionMap[i].meta,
                Type = isMatchDay ? SessionType.Match : sessionMap[i].type,
                IsCompleted = false
            });
        }

        return plan;
    }

    private Dictionary<int, (string label, string meta, SessionType type)> GetSessionMap(Position position, SeasonMode mode)
    {
        // Base week structure: Mon=Recovery, Tue=Field, Wed=Strength, Thu=Field, Fri=Rest, Sat=Activation, Sun=Match
        if (mode == SeasonMode.InSeason)
        {
            return new Dictionary<int, (string, string, SessionType)>
            {
                [0] = ("Recovery", "Light walk + mobility work", SessionType.Recovery),
                [1] = GetFieldSession(position),
                [2] = GetStrengthSession(position, reduced: true),
                [3] = ("Light Field", "40 min moderate intensity", SessionType.Field),
                [4] = ("Rest", "Full rest day", SessionType.Rest),
                [5] = ("Activation", "20 min light activation", SessionType.Activation),
                [6] = ("Match Day", "Match day — stay sharp", SessionType.Match)
            };
        }
        else if (mode == SeasonMode.PreSeason)
        {
            return new Dictionary<int, (string, string, SessionType)>
            {
                [0] = ("Conditioning", "Aerobic base run — 45 min", SessionType.Field),
                [1] = GetStrengthSession(position, reduced: false),
                [2] = ("Interval Run", "High intensity intervals — 35 min", SessionType.Field),
                [3] = GetStrengthSession(position, reduced: false),
                [4] = ("Field Skills", "Ball work + sprints — 50 min", SessionType.Field),
                [5] = ("Recovery", "Light mobility + stretch", SessionType.Recovery),
                [6] = ("Rest", "Full rest day", SessionType.Rest)
            };
        }
        else // OffSeason
        {
            return new Dictionary<int, (string, string, SessionType)>
            {
                [0] = ("Easy Run", "30 min easy jog", SessionType.Field),
                [1] = ("Gym", "General fitness — full body", SessionType.Strength),
                [2] = ("Rest", "Full rest day", SessionType.Rest),
                [3] = ("Active Recovery", "Swim or cycle — 30 min", SessionType.Recovery),
                [4] = ("Gym", "General fitness — full body", SessionType.Strength),
                [5] = ("Rest", "Full rest day", SessionType.Rest),
                [6] = ("Rest", "Full rest day", SessionType.Rest)
            };
        }
    }

    private (string label, string meta, SessionType type) GetFieldSession(Position position) => position switch
    {
        Position.Goalkeeper => ("Goalkeeper Drills", "Shot stopping + distribution — 45 min", SessionType.Field),
        Position.Fullback => ("Defensive Drills", "Marking + tackling work — 45 min", SessionType.Field),
        Position.Midfielder => ("Repeat Sprints", "5x200m + possession work — 50 min", SessionType.Field),
        Position.Forward => ("Agility + Finishing", "Acceleration drills + shooting — 45 min", SessionType.Field),
        _ => ("Field Session", "45 min moderate intensity", SessionType.Field)
    };

    private (string label, string meta, SessionType type) GetStrengthSession(Position position, bool reduced) => position switch
    {
        Position.Goalkeeper => ("Upper Strength", reduced ? "Reduced: push/pull — 35 min" : "Push/pull + core — 50 min", SessionType.Strength),
        Position.Fullback => ("Lower Strength", reduced ? "Reduced: squat/hinge — 35 min" : "Squat + hinge heavy — 55 min", SessionType.Strength),
        Position.Midfielder => ("Full Body", reduced ? "Reduced: compound lifts — 35 min" : "Compound lifts — 50 min", SessionType.Strength),
        Position.Forward => ("Power + Speed", reduced ? "Reduced: plyometrics — 30 min" : "Plyometrics + power — 45 min", SessionType.Strength),
        _ => ("Strength", reduced ? "Gym — reduced volume" : "Gym — full session", SessionType.Strength)
    };

    public List<TrainingDay> AdjustForMissedSession(List<TrainingDay> plan, int missedIndex)
    {
        var adjusted = plan.ToList();
        // Find next non-rest, non-match session and reduce it
        for (int i = missedIndex + 1; i < adjusted.Count; i++)
        {
            if (adjusted[i].Type != SessionType.Rest && adjusted[i].Type != SessionType.Match)
            {
                adjusted[i].Label += " (adjusted)";
                adjusted[i].Meta = "Reduced intensity — missed previous session";
                adjusted[i].IsMissed = false;
                break;
            }
        }
        adjusted[missedIndex].IsMissed = true;
        return adjusted;
    }

    public static List<PositionPlan> GetPositionPlans() => new()
    {
        new PositionPlan
        {
            Position = Position.Goalkeeper,
            Title = "Goalkeeper Plan",
            Icon = "🥅",
            FocusSummary = "Power + agility",
            Description = "Explosive power, reaction training, and aerial work. High intensity bursts with full recovery between efforts.",
            Tags = new() { "Power", "Reactions", "Aerial" }
        },
        new PositionPlan
        {
            Position = Position.Fullback,
            Title = "Full-back Plan",
            Icon = "🛡️",
            FocusSummary = "Strength + short bursts",
            Description = "Physical strength, short sprint recovery, and defensive positioning. Built to win physical battles.",
            Tags = new() { "Strength", "Short sprints", "Marking" }
        },
        new PositionPlan
        {
            Position = Position.Midfielder,
            Title = "Midfielder Plan",
            Icon = "🔄",
            FocusSummary = "Endurance + repeat sprints",
            Description = "Repeat sprints, endurance base, and aerobic capacity. High work rate needed across the full field.",
            Tags = new() { "Endurance", "Repeat sprints", "Aerobic base" }
        },
        new PositionPlan
        {
            Position = Position.Forward,
            Title = "Forward Plan",
            Icon = "⚡",
            FocusSummary = "Agility + acceleration",
            Description = "First-step acceleration, agility, and shooting under fatigue. Sharp and explosive from a standing start.",
            Tags = new() { "Agility", "Acceleration", "Shooting" }
        }
    };
}
