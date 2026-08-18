using Firebase.Database;
using Firebase.Database.Query;
using System.Text.Json.Serialization;

namespace GAAPerform.Auth;

public class FirestoreService
{
    private FirebaseClient GetClient(string token)
    {
        return new FirebaseClient(
            AppSettings.Load().Firebase.DatabaseUrl,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(token)
            });
    }

    public async Task SaveUserProfileAsync(string userId, string email, string role, string token)
    {
        using var client = GetClient(token);
        await client
            .Child("users")
            .Child(userId)
            .PutAsync(new { email, role, createdAt = DateTime.UtcNow.ToString("o") });
    }

    public async Task<string?> GetUserRoleAsync(string userId, string token)
    {
        try
        {
            using var client = GetClient(token);
            var result = await client
                .Child("users")
                .Child(userId)
                .Child("role")
                .OnceSingleAsync<string>();
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task AssignSessionToPlayerAsync(string coachId, string playerEmail, string title, DateTime date, int eventType, string notes, string token)
    {
        using var client = GetClient(token);
        var safeEmail = playerEmail.Replace(".", "_").Replace("@", "_at_");
        await client
            .Child("assignedSessions")
            .Child(safeEmail)
            .PostAsync(new
            {
                coachId,
                playerEmail,
                title,
                date = date.ToString("o"),
                eventType,
                notes,
                isCoachAssigned = true
            });
    }

    public async Task<List<AssignedSession>> GetAssignedSessionsAsync(string playerEmail, string token)
    {
        try
        {
            using var client = GetClient(token);
            var safeEmail = playerEmail.Replace(".", "_").Replace("@", "_at_");
            var result = await client
                .Child("assignedSessions")
                .Child(safeEmail)
                .OnceAsync<AssignedSession>();

            return result.Select(r => r.Object).ToList();
        }
        catch
        {
            return new List<AssignedSession>();
        }
    }

    public async Task AddPlayerToSquadAsync(string coachId, string playerEmail, string token)
    {
        using var client = GetClient(token);
        var safeCoachId = coachId.Replace(".", "_");
        await client
            .Child("coachPlayers")
            .Child(safeCoachId)
            .PostAsync(new
            {
                playerEmail,
                addedAt = DateTime.UtcNow.ToString("o")
            });
    }

    public async Task<List<string>> GetPlayersAsync(string coachId, string token)
    {
        try
        {
            using var client = GetClient(token);
            var safeCoachId = coachId.Replace(".", "_");
            var result = await client
                .Child("coachPlayers")
                .Child(safeCoachId)
                .OnceAsync<PlayerRecord>();

            return result.Select(r => r.Object.PlayerEmail).ToList();
        }
        catch
        {
            return new List<string>();
        }
    }
    public async Task SavePlayerSessionResultAsync(
    string playerEmail,
    string sessionTitle,
    DateTime date,
    int durationSeconds,
    string comment,
    string token)
    {
        using var client = GetClient(token);
        var safeEmail = playerEmail.Replace(".", "_").Replace("@", "_at_");
        await client
            .Child("sessionResults")
            .Child(safeEmail)
            .PostAsync(new
            {
                playerEmail,
                sessionTitle,
                date = date.ToString("o"),
                durationSeconds,
                comment,
                completedAt = DateTime.UtcNow.ToString("o")
            });
    }

    public async Task<List<PlayerSessionResult>> GetPlayerSessionResultsAsync(
        string playerEmail,
        string token)
    {
        try
        {
            using var client = GetClient(token);
            var safeEmail = playerEmail.Replace(".", "_").Replace("@", "_at_");
            var result = await client
                .Child("sessionResults")
                .Child(safeEmail)
                .OnceAsync<PlayerSessionResult>();
            return result.Select(r => r.Object).OrderByDescending(r => r.CompletedAt).ToList();
        }
        catch
        {
            return new List<PlayerSessionResult>();
        }
    }

    public async Task<List<FirebaseExercise>> GetExercisesAsync(string token)
    {
        try
        {
            using var client = GetClient(token);
            var result = await client
                .Child("exercises")
                .OnceAsync<FirebaseExercise>();

            return result.Select(r =>
            {
                r.Object.Id = r.Key;
                return r.Object;
            }).ToList();
        }
        catch
        {
            return new List<FirebaseExercise>();
        }
    }

    public async Task SaveExerciseAsync(FirebaseExercise exercise, string token)
    {
        using var client = GetClient(token);
        if (string.IsNullOrEmpty(exercise.Id))
        {
            await client.Child("exercises").PostAsync(exercise);
        }
        else
        {
            await client.Child("exercises").Child(exercise.Id).PutAsync(exercise);
        }
    }

    public async Task DeleteExerciseAsync(string exerciseId, string token)
    {
        using var client = GetClient(token);
        await client.Child("exercises").Child(exerciseId).DeleteAsync();
    }

    public async Task SeedExerciseLibraryAsync(string token)
    {
        var exercises = new List<object>
    {
        new { name = "Back Squat", category = "Strength", defaultSets = "4", defaultReps = "6", notes = "70-80% max. Keep chest up and knees tracking over toes.", videoUrl = "https://www.youtube.com/watch?v=ultWZbUMPL8", positions = new[] { "Midfielder", "Fullback", "Forward" }, sessionTypes = new[] { "Strength" } },
        new { name = "Bench Press", category = "Strength", defaultSets = "4", defaultReps = "8", notes = "Full range of motion. Control the descent.", videoUrl = "https://www.youtube.com/watch?v=rT7DgCr-3pg", positions = new[] { "Goalkeeper", "Midfielder" }, sessionTypes = new[] { "Strength" } },
        new { name = "Deadlift", category = "Strength", defaultSets = "3", defaultReps = "5", notes = "Brace your core. Drive through the floor.", videoUrl = "https://www.youtube.com/watch?v=op9kVnSso6Q", positions = new[] { "Fullback", "Midfielder" }, sessionTypes = new[] { "Strength" } },
        new { name = "Pull Ups", category = "Strength", defaultSets = "4", defaultReps = "8", notes = "Full range — dead hang to chin over bar.", videoUrl = "https://www.youtube.com/watch?v=eGo4IYlbE5g", positions = new[] { "Goalkeeper", "Midfielder" }, sessionTypes = new[] { "Strength" } },
        new { name = "Romanian Deadlift", category = "Strength", defaultSets = "4", defaultReps = "8", notes = "Focus on hamstring stretch. Keep bar close to body.", videoUrl = "https://www.youtube.com/watch?v=hCDzSR6bW10", positions = new[] { "Fullback", "Midfielder" }, sessionTypes = new[] { "Strength" } },
        new { name = "Bulgarian Split Squat", category = "Strength", defaultSets = "3", defaultReps = "10", notes = "Rear foot elevated. Add weight when comfortable.", videoUrl = "https://www.youtube.com/watch?v=2C-uNgKwPLE", positions = new[] { "Forward", "Fullback" }, sessionTypes = new[] { "Strength" } },
        new { name = "Hip Thrust", category = "Strength", defaultSets = "4", defaultReps = "12", notes = "Drive through the heel. Full hip extension at top.", videoUrl = "https://www.youtube.com/watch?v=xDmFkJxPzeM", positions = new[] { "Fullback", "Forward" }, sessionTypes = new[] { "Strength" } },
        new { name = "Box Jumps", category = "Power", defaultSets = "4", defaultReps = "6", notes = "Maximum height. Full reset between reps.", videoUrl = "https://www.youtube.com/watch?v=NBY9-kTuHEk", positions = new[] { "Forward", "Midfielder" }, sessionTypes = new[] { "Strength" } },
        new { name = "Farmer Carries", category = "Strength", defaultSets = "4", defaultReps = "30m", notes = "Heavy dumbbells. Tall posture throughout.", videoUrl = "https://www.youtube.com/watch?v=Fkzk_RqlYig", positions = new[] { "Midfielder", "Fullback" }, sessionTypes = new[] { "Strength" } },
        new { name = "Foam Rolling", category = "Recovery", defaultDuration = "10 min", notes = "Quads, hamstrings, calves, glutes. Slow controlled pressure.", videoUrl = "https://www.youtube.com/watch?v=nt67KBSEcUU", positions = new string[] { }, sessionTypes = new[] { "Recovery" } },
        new { name = "Static Stretching", category = "Recovery", defaultDuration = "10 min", notes = "Hold each stretch 30-45 seconds. No bouncing.", videoUrl = "https://www.youtube.com/watch?v=L_xrDAtykMI", positions = new string[] { }, sessionTypes = new[] { "Recovery" } },
        new { name = "200m Repeats", category = "Conditioning", defaultSets = "5", defaultReps = "200m", notes = "90 seconds rest between reps. Hit the same pace each time.", videoUrl = "", positions = new[] { "Midfielder", "Forward" }, sessionTypes = new[] { "Field" } },
        new { name = "Agility Ladder", category = "Speed", defaultDuration = "10 min", notes = "Various patterns. Focus on quick feet.", videoUrl = "https://www.youtube.com/watch?v=wN3PtCMElFU", positions = new[] { "Forward", "Midfielder" }, sessionTypes = new[] { "Field" } },
        new { name = "Dynamic Stretching", category = "Warmup", defaultDuration = "5 min", notes = "Leg swings, hip circles, arm circles.", videoUrl = "https://www.youtube.com/watch?v=EC3Nf8cKBnM", positions = new string[] { }, sessionTypes = new[] { "Field", "Strength", "Recovery" } },
        new { name = "Medicine Ball Slams", category = "Power", defaultSets = "3", defaultReps = "10", notes = "Explosive — maximum effort each rep.", videoUrl = "https://www.youtube.com/watch?v=GnZCMbDFNsM", positions = new[] { "Goalkeeper", "Midfielder" }, sessionTypes = new[] { "Strength" } }
    };

        using var client = GetClient(token);
        foreach (var exercise in exercises)
        {
            await client.Child("exercises").PostAsync(exercise);
        }
    }
}

public class AssignedSession
{
    public string CoachId { get; set; } = string.Empty;
    public string PlayerEmail { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int EventType { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsCoachAssigned { get; set; }
}

public class PlayerRecord
{
    public string PlayerEmail { get; set; } = string.Empty;
}
public class PlayerSessionResult
{
    public string PlayerEmail { get; set; } = string.Empty;
    public string SessionTitle { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string CompletedAt { get; set; } = string.Empty;

    public string DurationFormatted
    {
        get
        {
            var ts = TimeSpan.FromSeconds(DurationSeconds);
            return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}

public class FirebaseExercise
{
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
    [JsonPropertyName("defaultSets")]
    public string DefaultSets { get; set; } = string.Empty;
    [JsonPropertyName("defaultReps")]
    public string DefaultReps { get; set; } = string.Empty;
    [JsonPropertyName("defaultDuration")]
    public string DefaultDuration { get; set; } = string.Empty;
    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;
    [JsonPropertyName("videoUrl")]
    public string VideoUrl { get; set; } = string.Empty;
    [JsonPropertyName("positions")]
    public List<string> Positions { get; set; } = new();
    [JsonPropertyName("sessionTypes")]
    public List<string> SessionTypes { get; set; } = new();
}