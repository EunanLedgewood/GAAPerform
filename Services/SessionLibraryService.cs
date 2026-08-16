using GAAPerform.Models;

namespace GAAPerform.Services;

public class SessionLibraryService
{
    public SessionDetail GetSessionDetail(TrainingDay day, Position position)
    {
        return day.Type switch
        {
            SessionType.Match => GetMatchDaySession(),
            SessionType.Strength => GetStrengthSession(position),
            SessionType.Field => GetFieldSession(position, day.Label),
            SessionType.Recovery => GetRecoverySession(),
            SessionType.Activation => GetActivationSession(),
            _ => GetRestSession()
        };
    }

    private SessionDetail GetMatchDaySession() => new()
    {
        Title = "Match Day",
        Description = "Focus on staying sharp and conserving energy. Light activation only.",
        Duration = "20 min",
        Intensity = "Low",
        Type = SessionType.Match,
        Exercises = new()
        {
            new Exercise { Name = "Light jog", Duration = "5 min", Notes = "Easy pace, just getting warm" },
            new Exercise { Name = "Dynamic stretching", Duration = "5 min", Notes = "Leg swings, hip circles, arm circles" },
            new Exercise { Name = "Short sprints", Sets = "4", Reps = "20m", Notes = "75% effort only" },
            new Exercise { Name = "Ball work", Duration = "10 min", Notes = "Casual kick-around, don't overdo it" }
        },
        CoachNotes = new() { "Eat a good meal 3 hours before throw-in", "Stay hydrated throughout the day", "Get to the ground early to warm up properly" }
    };

    private SessionDetail GetStrengthSession(Position position) => position switch
    {
        Position.Goalkeeper => new()
        {
            Title = "Goalkeeper Strength",
            Description = "Upper body power and explosive movements for shot stopping.",
            Duration = "50 min",
            Intensity = "High",
            Type = SessionType.Strength,
            Exercises = new()
            {
                new Exercise { Name = "Warm up", Duration = "5 min", Notes = "Light cardio and mobility" },
                new Exercise { Name = "Bench Press", Sets = "4", Reps = "6", Notes = "Heavy — 80% max" },
                new Exercise { Name = "Pull Ups", Sets = "4", Reps = "8", Notes = "Full range of motion" },
                new Exercise { Name = "Medicine Ball Slams", Sets = "3", Reps = "10", Notes = "Explosive — maximum effort each rep" },
                new Exercise { Name = "Lateral Band Walks", Sets = "3", Reps = "15 each side", Notes = "Keep tension throughout" },
                new Exercise { Name = "Core Plank Circuit", Duration = "10 min", Notes = "Plank, side plank, hollow hold" }
            },
            CoachNotes = new() { "Rest 2-3 minutes between heavy sets", "Focus on explosive power not just strength" }
        },
        Position.Fullback => new()
        {
            Title = "Full-back Lower Body",
            Description = "Squat and hinge focus for defensive power and short sprint speed.",
            Duration = "55 min",
            Intensity = "High",
            Type = SessionType.Strength,
            Exercises = new()
            {
                new Exercise { Name = "Warm up", Duration = "5 min", Notes = "Light cardio and mobility" },
                new Exercise { Name = "Back Squat", Sets = "5", Reps = "5", Notes = "Heavy — build to 85% max" },
                new Exercise { Name = "Romanian Deadlift", Sets = "4", Reps = "8", Notes = "Focus on hamstring stretch" },
                new Exercise { Name = "Bulgarian Split Squat", Sets = "3", Reps = "10 each leg", Notes = "Add weight when comfortable" },
                new Exercise { Name = "Hip Thrust", Sets = "4", Reps = "12", Notes = "Drive through the heel" },
                new Exercise { Name = "Calf Raises", Sets = "3", Reps = "20", Notes = "Slow and controlled" }
            },
            CoachNotes = new() { "Rest 2-3 minutes between squat sets", "Keep your back straight on deadlifts" }
        },
        Position.Forward => new()
        {
            Title = "Forward Power Session",
            Description = "Plyometrics and acceleration work for explosive forward movement.",
            Duration = "45 min",
            Intensity = "High",
            Type = SessionType.Strength,
            Exercises = new()
            {
                new Exercise { Name = "Warm up", Duration = "5 min", Notes = "Light cardio and mobility" },
                new Exercise { Name = "Box Jumps", Sets = "4", Reps = "6", Notes = "Maximum height — full reset between reps" },
                new Exercise { Name = "Trap Bar Deadlift", Sets = "4", Reps = "5", Notes = "Explosive concentric phase" },
                new Exercise { Name = "Single Leg Bounds", Sets = "3", Reps = "8 each leg", Notes = "Focus on distance" },
                new Exercise { Name = "Depth Drops", Sets = "3", Reps = "6", Notes = "Absorb landing quietly" },
                new Exercise { Name = "Sprint starts", Sets = "6", Reps = "10m", Notes = "From standing start — maximum effort" }
            },
            CoachNotes = new() { "Full rest between plyometric sets", "Quality over quantity — don't rush reps" }
        },
        _ => new()
        {
            Title = "Midfielder Full Body",
            Description = "Compound lifts targeting full body strength and repeat sprint capacity.",
            Duration = "50 min",
            Intensity = "High",
            Type = SessionType.Strength,
            Exercises = new()
    {
        new Exercise { Name = "Warm up", Duration = "5 min", Notes = "Light cardio and mobility" },
        new Exercise { Name = "Back Squat", Sets = "4", Reps = "6", Notes = "70-80% max", VideoUrl = "https://www.youtube.com/watch?v=rrJIyZGlK8c" },
        new Exercise { Name = "Bench Press", Sets = "4", Reps = "8", Notes = "Moderate weight, full range", VideoUrl = "https://www.youtube.com/watch?v=rT7DgCr-3pg" },
        new Exercise { Name = "Deadlift", Sets = "3", Reps = "5", Notes = "Heavy — brace your core", VideoUrl = "https://www.youtube.com/watch?v=op9kVnSso6Q" },
        new Exercise { Name = "Chin Ups", Sets = "3", Reps = "8", Notes = "Add weight if needed", VideoUrl = "https://www.youtube.com/watch?v=eGo4IYlbE5g" },
        new Exercise { Name = "Farmer Carries", Sets = "4", Reps = "30m", Notes = "Heavy dumbbells, tall posture", VideoUrl = "https://www.youtube.com/watch?v=Fkzk_RqlYig" }
    },
            CoachNotes = new() { "Rest 90 seconds between sets", "Focus on form before adding weight" }
        }
    };

    private SessionDetail GetFieldSession(Position position, string label)
    {
        if (label.Contains("Repeat Sprint") || label.Contains("repeat"))
            return GetRepeatSprintSession();

        if (label.Contains("Light") || label.Contains("light"))
            return GetLightFieldSession(position);

        return position switch
        {
            Position.Goalkeeper => new()
            {
                Title = "Goalkeeper Drills",
                Description = "Shot stopping, distribution, and footwork under pressure.",
                Duration = "45 min",
                Intensity = "Medium",
                Type = SessionType.Field,
                Exercises = new()
                {
                    new Exercise { Name = "Footwork ladder", Duration = "10 min", Notes = "Quick feet patterns" },
                    new Exercise { Name = "Shot stopping — low", Sets = "3", Reps = "10 shots", Notes = "Partner or machine" },
                    new Exercise { Name = "Shot stopping — high", Sets = "3", Reps = "10 shots", Notes = "Work both sides" },
                    new Exercise { Name = "Distribution kicks", Sets = "4", Reps = "10 each foot", Notes = "Accuracy over power" },
                    new Exercise { Name = "1v1 situations", Duration = "10 min", Notes = "Decision making under pressure" }
                },
                CoachNotes = new() { "Focus on positioning before the shot", "Communicate with your defence" }
            },
            Position.Forward => new()
            {
                Title = "Agility + Finishing",
                Description = "First touch, acceleration drills, and shooting under fatigue.",
                Duration = "45 min",
                Intensity = "Medium-High",
                Type = SessionType.Field,
                Exercises = new()
                {
                    new Exercise { Name = "Agility ladder", Duration = "10 min", Notes = "Various patterns" },
                    new Exercise { Name = "Cone acceleration drills", Sets = "6", Reps = "20m", Notes = "Maximum effort each run" },
                    new Exercise { Name = "First touch and shoot", Sets = "4", Reps = "8 shots", Notes = "From different angles" },
                    new Exercise { Name = "1v1 with defender", Duration = "10 min", Notes = "Create space and finish" },
                    new Exercise { Name = "Shooting under fatigue", Sets = "3", Reps = "5 shots", Notes = "After a sprint — simulate match conditions" }
                },
                CoachNotes = new() { "Work on your weaker foot", "Stay composed in front of goal" }
            },
            _ => new()
            {
                Title = "Field Session",
                Description = "Technical work, possession, and conditioning.",
                Duration = "50 min",
                Intensity = "Medium",
                Type = SessionType.Field,
                Exercises = new()
                {
                    new Exercise { Name = "Warm up jog", Duration = "5 min", Notes = "Easy pace" },
                    new Exercise { Name = "Passing drills", Duration = "15 min", Notes = "Short and long range" },
                    new Exercise { Name = "Possession game", Duration = "15 min", Notes = "Keep the ball moving" },
                    new Exercise { Name = "Positional work", Duration = "10 min", Notes = "Focus on your role" },
                    new Exercise { Name = "Cool down", Duration = "5 min", Notes = "Light jog and stretch" }
                },
                CoachNotes = new() { "Keep intensity consistent throughout", "Focus on quality touches" }
            }
        };
    }

    private SessionDetail GetRepeatSprintSession() => new()
    {
        Title = "Repeat Sprints",
        Description = "High intensity running to build match-ready aerobic capacity.",
        Duration = "50 min",
        Intensity = "High",
        Type = SessionType.Field,
        Exercises = new()
        {
            new Exercise { Name = "Warm up jog", Duration = "10 min", Notes = "Gradually increase pace" },
            new Exercise { Name = "Dynamic stretching", Duration = "5 min", Notes = "Leg swings, high knees" },
            new Exercise { Name = "200m repeats", Sets = "5", Reps = "200m", Notes = "90 seconds rest between each" },
            new Exercise { Name = "Possession work", Duration = "15 min", Notes = "Maintain quality when tired" },
            new Exercise { Name = "Cool down", Duration = "5 min", Notes = "Easy jog and stretch" }
        },
        CoachNotes = new() { "Hit the same pace on every rep", "If you slow down more than 5 seconds — stop and rest" }
    };

    private SessionDetail GetLightFieldSession(Position position) => new()
    {
        Title = "Light Field Session",
        Description = "Low intensity — keep the legs moving without creating fatigue.",
        Duration = "40 min",
        Intensity = "Low",
        Type = SessionType.Field,
        Exercises = new()
        {
            new Exercise { Name = "Easy jog", Duration = "10 min", Notes = "Conversational pace" },
            new Exercise { Name = "Ball work", Duration = "15 min", Notes = "Casual — no intensity" },
            new Exercise { Name = "Stretch", Duration = "10 min", Notes = "Focus on tight areas" },
            new Exercise { Name = "Strides", Sets = "4", Reps = "60m", Notes = "70% effort — stay smooth" }
        },
        CoachNotes = new() { "This session should feel easy", "Save your legs for match day" }
    };

    private SessionDetail GetRecoverySession() => new()
    {
        Title = "Recovery Session",
        Description = "Active recovery to flush out soreness and restore movement quality.",
        Duration = "30 min",
        Intensity = "Very Low",
        Type = SessionType.Recovery,
        Exercises = new()
    {
        new Exercise { Name = "Light walk or cycle", Duration = "10 min", Notes = "Very easy — just moving" },
        new Exercise { Name = "Foam rolling", Duration = "10 min", Notes = "Quads, hamstrings, calves, glutes", VideoUrl = "https://www.youtube.com/watch?v=nt67KBSEcUU" },
        new Exercise { Name = "Static stretching", Duration = "10 min", Notes = "Hold each stretch 30-45 seconds", VideoUrl = "https://www.youtube.com/watch?v=L_xrDAtykMI" }
    },
        CoachNotes = new() { "Hydrate well today", "Sleep is the best recovery tool — get 8 hours" }
    };

    private SessionDetail GetActivationSession() => new()
    {
        Title = "Pre-Match Activation",
        Description = "Wake up the nervous system and prepare the body for tomorrow's match.",
        Duration = "20 min",
        Intensity = "Low",
        Type = SessionType.Activation,
        Exercises = new()
        {
            new Exercise { Name = "Light jog", Duration = "5 min", Notes = "Easy pace" },
            new Exercise { Name = "Dynamic warm up", Duration = "5 min", Notes = "Leg swings, hip circles, arm circles" },
            new Exercise { Name = "Short accelerations", Sets = "4", Reps = "30m", Notes = "Build up to 80% — don't go full pace" },
            new Exercise { Name = "Stretch", Duration = "5 min", Notes = "Focus on legs and hips" }
        },
        CoachNotes = new() { "Keep this short and sharp", "Don't tire yourself out the day before a match" }
    };

    private SessionDetail GetRestSession() => new()
    {
        Title = "Rest Day",
        Description = "Complete rest. Let your body recover and rebuild.",
        Duration = "—",
        Intensity = "—",
        Type = SessionType.Rest,
        Exercises = new(),
        CoachNotes = new() { "Rest is part of training", "Eat well and sleep well today" }
    };
}