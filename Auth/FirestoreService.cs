using Firebase.Database;
using Firebase.Database.Query;

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