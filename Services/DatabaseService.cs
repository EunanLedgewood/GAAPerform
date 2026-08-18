using GAAPerform.Auth;
using GAAPerform.Models;
using SQLite;

namespace GAAPerform.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "gaaperform.db3");
    }

    private async Task InitAsync()
    {
        if (_db is not null) return;
        _db = new SQLiteAsyncConnection(_dbPath);
        await _db.CreateTableAsync<SessionLog>();
        await _db.CreateTableAsync<UserProfile>();
        await _db.CreateTableAsync<CalendarEvent>();
        await _db.CreateTableAsync<CompletedSession>();
        await _db.CreateTableAsync<CachedExercise>();
    }

    public async Task<UserProfile> GetProfileAsync()
    {
        await InitAsync();
        var profile = await _db!.Table<UserProfile>().FirstOrDefaultAsync();
        if (profile is null)
        {
            profile = new UserProfile { Name = "Player", Position = Position.Midfielder, SeasonMode = SeasonMode.InSeason };
            await _db.InsertAsync(profile);
        }
        return profile;
    }

    public async Task SaveProfileAsync(UserProfile profile)
    {
        await InitAsync();
        if (profile.Id == 0)
            await _db!.InsertAsync(profile);
        else
            await _db!.UpdateAsync(profile);
    }

    public async Task<List<SessionLog>> GetRecentLogsAsync(int count = 7)
    {
        await InitAsync();
        return await _db!.Table<SessionLog>()
            .OrderByDescending(l => l.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task SaveLogAsync(SessionLog log)
    {
        await InitAsync();
        await _db!.InsertAsync(log);
    }

    public async Task<double> GetAverageFeelingAsync()
    {
        await InitAsync();
        var logs = await GetRecentLogsAsync(7);
        if (!logs.Any()) return 0;
        return logs.Average(l => l.FeelingScore);
    }

    public async Task<double> GetAverageSorenessAsync()
    {
        await InitAsync();
        var logs = await GetRecentLogsAsync(7);
        if (!logs.Any()) return 0;
        return logs.Average(l => l.SorenessScore);
    }

    public async Task<int> GetReadinessScoreAsync()
    {
        await InitAsync();
        var logs = await GetRecentLogsAsync(7);
        if (!logs.Any()) return 70;
        var avgFeeling = logs.Average(l => l.FeelingScore);
        var avgSoreness = logs.Average(l => l.SorenessScore);
        var sessionCount = logs.Count;
        double score = (avgFeeling / 5.0 * 50) + ((5 - avgSoreness) / 5.0 * 30) + (Math.Min(sessionCount, 4) / 4.0 * 20);
        return (int)Math.Round(score);
    }

    public async Task<List<CalendarEvent>> GetEventsForMonthAsync(int year, int month)
    {
        await InitAsync();
        try
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            return await _db!.Table<CalendarEvent>()
                .Where(e => e.Date >= start && e.Date < end)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEventsForMonthAsync error: {ex}");
            return new List<CalendarEvent>();
        }
    }

    public async Task<List<CalendarEvent>> GetEventsForDateAsync(DateTime date)
    {
        await InitAsync();
        try
        {
            var start = date.Date;
            var end = start.AddDays(1);
            return await _db!.Table<CalendarEvent>()
                .Where(e => e.Date >= start && e.Date < end)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEventsForDateAsync error: {ex}");
            return new List<CalendarEvent>();
        }
    }

    public async Task<List<CalendarEvent>> GetUpcomingEventsAsync(int days = 30)
    {
        await InitAsync();
        var start = DateTime.Today;
        var end = start.AddDays(days);
        return await _db!.Table<CalendarEvent>()
            .Where(e => e.Date >= start && e.Date < end)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task SaveEventAsync(CalendarEvent calEvent)
    {
        await InitAsync();
        if (calEvent.Id == 0)
            await _db!.InsertAsync(calEvent);
        else
            await _db!.UpdateAsync(calEvent);
    }

    public async Task DeleteEventAsync(CalendarEvent calEvent)
    {
        await InitAsync();
        await _db!.DeleteAsync(calEvent);
    }

    public async Task<List<CalendarEvent>> GetEventsForWeekAsync(DateTime monday, DateTime sunday)
    {
        await InitAsync();
        var start = monday.Date;
        var end = sunday.Date.AddDays(1);
        return await _db!.Table<CalendarEvent>()
            .Where(e => e.Date >= start && e.Date < end)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task SyncCoachAssignedSessionsAsync(string playerEmail, FirestoreService firestore, string token)
    {
        await InitAsync();
        var assignedSessions = await firestore.GetAssignedSessionsAsync(playerEmail, token);

        foreach (var session in assignedSessions)
        {
            if (DateTime.TryParse(session.Date, out var date))
            {
                // Check if already exists
                var existing = await _db!.Table<CalendarEvent>()
                    .Where(e => e.Date == date && e.IsCoachAssigned && e.Title == session.Title)
                    .FirstOrDefaultAsync();

                if (existing is null)
                {
                    var calEvent = new CalendarEvent
                    {
                        Date = date,
                        EventTypeInt = session.EventType,
                        Title = session.Title,
                        Notes = session.Notes,
                        IsCoachAssigned = true
                    };
                    await _db.InsertAsync(calEvent);
                }
            }
        }
    }

    public async Task SaveCompletedSessionAsync(CompletedSession session)
    {
        await InitAsync();
        session.SetsJson = System.Text.Json.JsonSerializer.Serialize(session.Sets);
        if (session.Id == 0)
            await _db!.InsertAsync(session);
        else
            await _db!.UpdateAsync(session);
    }

    public async Task<List<CompletedSession>> GetCompletedSessionsAsync()
    {
        await InitAsync();
        var sessions = await _db!.Table<CompletedSession>()
            .OrderByDescending(s => s.Date)
            .ToListAsync();

        foreach (var session in sessions)
        {
            if (!string.IsNullOrEmpty(session.SetsJson))
            {
                session.Sets = System.Text.Json.JsonSerializer
                    .Deserialize<List<CompletedSet>>(session.SetsJson) ?? new();
            }
        }
        return sessions;
    }

    public async Task CacheExercisesAsync(List<FirebaseExercise> exercises)
    {
        await InitAsync();
        await _db!.ExecuteAsync("DELETE FROM CachedExercise");
        foreach (var exercise in exercises)
        {
            await _db.InsertAsync(new CachedExercise
            {
                ExerciseId = exercise.Id,
                Name = exercise.Name,
                Category = exercise.Category,
                DefaultSets = exercise.DefaultSets,
                DefaultReps = exercise.DefaultReps,
                DefaultDuration = exercise.DefaultDuration,
                Notes = exercise.Notes,
                VideoUrl = exercise.VideoUrl,
                PositionsJson = System.Text.Json.JsonSerializer.Serialize(exercise.Positions),
                SessionTypesJson = System.Text.Json.JsonSerializer.Serialize(exercise.SessionTypes)
            });
        }
    }

    public async Task<List<FirebaseExercise>> GetCachedExercisesAsync()
    {
        await InitAsync();
        var cached = await _db!.Table<CachedExercise>().ToListAsync();
        return cached.Select(c => new FirebaseExercise
        {
            Id = c.ExerciseId,
            Name = c.Name,
            Category = c.Category,
            DefaultSets = c.DefaultSets,
            DefaultReps = c.DefaultReps,
            DefaultDuration = c.DefaultDuration,
            Notes = c.Notes,
            VideoUrl = c.VideoUrl,
            Positions = System.Text.Json.JsonSerializer
                .Deserialize<List<string>>(c.PositionsJson) ?? new(),
            SessionTypes = System.Text.Json.JsonSerializer
                .Deserialize<List<string>>(c.SessionTypesJson) ?? new()
        }).ToList();
    }
}