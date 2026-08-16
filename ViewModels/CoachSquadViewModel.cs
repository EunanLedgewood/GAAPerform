using AndroidX.ConstraintLayout.Helper.Widget;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Auth;
using GAAPerform.Models;
using IntelliJ.Lang.Annotations;
using System.Collections.ObjectModel;
using GAAPerform.Auth;

namespace GAAPerform.ViewModels;

public partial class CoachSquadViewModel : ObservableObject
{
    private readonly FirebaseAuthService _auth;
    private readonly FirestoreService _firestore;

    [ObservableProperty] private ObservableCollection<string> players = new();
    [ObservableProperty] private string newPlayerEmail = string.Empty;
    [ObservableProperty] private string selectedPlayerEmail = string.Empty;
    [ObservableProperty] private bool hasPlayers;
    [ObservableProperty] private bool isBusy = false;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool hasStatus = false;

    // Session assignment
    [ObservableProperty] private string sessionTitle = string.Empty;
    [ObservableProperty] private string sessionNotes = string.Empty;
    [ObservableProperty] private DateTime sessionDate = DateTime.Today.AddDays(1);
    [ObservableProperty] private bool isAssignMatch = false;
    [ObservableProperty] private bool isAssignTraining = true;
    [ObservableProperty] private bool isAssignGym = false;
    [ObservableProperty] private bool isAssignRecovery = false;
    [ObservableProperty] private EventType selectedEventType = EventType.Training;
    [ObservableProperty] private bool showAssignPanel = false;

    //Result loading to view for coaches
    [ObservableProperty] private ObservableCollection<PlayerSessionResult> playerResults = new();
    [ObservableProperty] private bool hasResults;
    [ObservableProperty] private string selectedPlayerForResults = string.Empty;

    public CoachSquadViewModel(FirebaseAuthService auth, FirestoreService firestore)
    {
        _auth = auth;
        _firestore = firestore;
    }

    public async Task LoadAsync()
    {
        if (!_auth.IsLoggedIn) return;
        IsBusy = true;

        try
        {
            var token = await _auth.GetTokenAsync();
            var playerList = await _firestore.GetPlayersAsync(_auth.CurrentUserId!, token);
            Players = new ObservableCollection<string>(playerList);
            HasPlayers = Players.Any();
        }
        catch (Exception ex)
        {
            ShowStatus($"Error loading squad: {ex.Message}");
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task AddPlayerAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPlayerEmail)) return;

        IsBusy = true;
        try
        {
            var token = await _auth.GetTokenAsync();
            await _firestore.AddPlayerToSquadAsync(_auth.CurrentUserId!, NewPlayerEmail.Trim().ToLower(), token);
            Players.Add(NewPlayerEmail.Trim().ToLower());
            HasPlayers = true;
            NewPlayerEmail = string.Empty;
            ShowStatus("Player added to squad!");
        }
        catch (Exception ex)
        {
            ShowStatus($"Error adding player: {ex.Message}");
        }
        IsBusy = false;
    }

    [RelayCommand]
    private void SelectPlayer(string email)
    {
        SelectedPlayerEmail = email;
        ShowAssignPanel = true;
        SessionTitle = string.Empty;
        SessionNotes = string.Empty;
        SessionDate = DateTime.Today.AddDays(1);
    }

    [RelayCommand]
    private void SelectEventType(string type)
    {
        SelectedEventType = type switch
        {
            "Match" => EventType.Match,
            "Gym" => EventType.GymSession,
            "Recovery" => EventType.Recovery,
            _ => EventType.Training
        };
        IsAssignMatch = SelectedEventType == EventType.Match;
        IsAssignTraining = SelectedEventType == EventType.Training;
        IsAssignGym = SelectedEventType == EventType.GymSession;
        IsAssignRecovery = SelectedEventType == EventType.Recovery;
    }

    [RelayCommand]
    private async Task AssignSessionAsync()
    {
        if (string.IsNullOrWhiteSpace(SessionTitle) || string.IsNullOrWhiteSpace(SelectedPlayerEmail))
        {
            ShowStatus("Please fill in a session title.");
            return;
        }

        IsBusy = true;
        try
        {
            var token = await _auth.GetTokenAsync();
            await _firestore.AssignSessionToPlayerAsync(
                _auth.CurrentUserId!,
                SelectedPlayerEmail,
                SessionTitle,
                SessionDate,
                (int)SelectedEventType,
                SessionNotes,
                token);

            ShowStatus($"Session assigned to {SelectedPlayerEmail}!");
            ShowAssignPanel = false;
            SessionTitle = string.Empty;
            SessionNotes = string.Empty;
        }
        catch (Exception ex)
        {
            ShowStatus($"Error assigning session: {ex.Message}");
        }
        IsBusy = false;
    }

    [RelayCommand]
    private void CloseAssignPanel()
    {
        ShowAssignPanel = false;
        SelectedPlayerEmail = string.Empty;
    }

    private void ShowStatus(string message)
    {
        StatusMessage = message;
        HasStatus = true;
        Task.Delay(3000).ContinueWith(_ =>
        {
            HasStatus = false;
        });
    }

    [RelayCommand]
    private async Task ViewPlayerResultsAsync(string playerEmail)
    {
        SelectedPlayerForResults = playerEmail;
        IsBusy = true;
        try
        {
            var token = await _auth.GetTokenAsync();
            var results = await _firestore.GetPlayerSessionResultsAsync(playerEmail, token);
            PlayerResults = new ObservableCollection<PlayerSessionResult>(results);
            HasResults = PlayerResults.Any();
        }
        catch (Exception ex)
        {
            ShowStatus($"Error loading results: {ex.Message}");
        }
        IsBusy = false;
    }
}