using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private ObservableCollection<CompletedSession> sessions = new();
    [ObservableProperty] private bool hasSessions;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private int totalSessions;
    [ObservableProperty] private string totalTimeFormatted = string.Empty;

    public HistoryViewModel(DatabaseService db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;

        var allSessions = await _db.GetCompletedSessionsAsync();
        Sessions = new ObservableCollection<CompletedSession>(allSessions);
        HasSessions = Sessions.Any();
        TotalSessions = Sessions.Count;

        var totalSeconds = allSessions.Sum(s => s.DurationSeconds);
        var ts = TimeSpan.FromSeconds(totalSeconds);
        TotalTimeFormatted = $"{(int)ts.TotalHours}h {ts.Minutes}m";

        IsLoading = false;
    }

    [RelayCommand]
    private void ToggleExpanded(CompletedSession session)
    {
        session.IsExpanded = !session.IsExpanded;
        var index = Sessions.IndexOf(session);
        if (index >= 0)
        {
            Sessions.RemoveAt(index);
            Sessions.Insert(index, session);
        }
    }
}