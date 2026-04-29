using SQLite;
using GAAPerform.Models;

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

        // Simple readiness formula: feeling weighted positively, soreness negatively
        double score = (avgFeeling / 5.0 * 50) + ((5 - avgSoreness) / 5.0 * 30) + (Math.Min(sessionCount, 4) / 4.0 * 20);
        return (int)Math.Round(score);
    }
}
