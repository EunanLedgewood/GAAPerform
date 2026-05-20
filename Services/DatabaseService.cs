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
}