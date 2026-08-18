using GAAPerform.Auth;
using GAAPerform.Models;

namespace GAAPerform.Services;

public class ExerciseCacheService
{
    private readonly DatabaseService _db;
    private readonly FirestoreService _firestore;
    private readonly FirebaseAuthService _auth;
    private List<FirebaseExercise> _cachedExercises = new();
    private DateTime _lastFetch = DateTime.MinValue;
    private const int CacheMinutes = 60;

    public ExerciseCacheService(
        DatabaseService db,
        FirestoreService firestore,
        FirebaseAuthService auth)
    {
        _db = db;
        _firestore = firestore;
        _auth = auth;
    }

    public async Task<List<FirebaseExercise>> GetExercisesAsync()
    {
        // Return cached if fresh
        if (_cachedExercises.Any() &&
            DateTime.Now - _lastFetch < TimeSpan.FromMinutes(CacheMinutes))
            return _cachedExercises;

        try
        {
            if (_auth.IsLoggedIn)
            {
                var token = await _auth.GetTokenAsync();
                var exercises = await _firestore.GetExercisesAsync(token);

                if (exercises.Any())
                {
                    _cachedExercises = exercises;
                    _lastFetch = DateTime.Now;

                    // Save to local SQLite cache
                    await _db.CacheExercisesAsync(exercises);
                    return _cachedExercises;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exercise fetch error: {ex.Message}");
        }

        // Fall back to local cache
        var local = await _db.GetCachedExercisesAsync();
        _cachedExercises = local;
        return _cachedExercises;
    }

    public async Task<List<FirebaseExercise>> GetExercisesForSessionAsync(
        string sessionType,
        string position)
    {
        var all = await GetExercisesAsync();
        System.Diagnostics.Debug.WriteLine($"Filtering for sessionType: {sessionType} position: {position}");
        foreach (var e in all)
            System.Diagnostics.Debug.WriteLine($"Exercise {e.Name} sessionTypes: {string.Join(",", e.SessionTypes)} positions: {string.Join(",", e.Positions)}");

        return all.Where(e =>
            (e.SessionTypes.Count == 0 || e.SessionTypes.Contains(sessionType)) &&
            (e.Positions.Count == 0 || e.Positions.Contains(position)))
            .ToList();
    }

    public void InvalidateCache()
    {
        _lastFetch = DateTime.MinValue;
        _cachedExercises.Clear();
    }
}