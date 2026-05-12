using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;

namespace GAAPerform.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private string playerName = string.Empty;
    [ObservableProperty] private Position selectedPosition = Position.Midfielder;
    [ObservableProperty] private SeasonMode selectedSeasonMode = SeasonMode.InSeason;
    [ObservableProperty] private DateTime nextMatchDate = DateTime.Today.AddDays(7);
    [ObservableProperty] private int currentStep = 1;
    [ObservableProperty] private bool isStep1 = true;
    [ObservableProperty] private bool isStep2 = false;
    [ObservableProperty] private bool isStep3 = false;

    [ObservableProperty] private bool isGoalkeeper = false;
    [ObservableProperty] private bool isFullback = false;
    [ObservableProperty] private bool isMidfielder = true;
    [ObservableProperty] private bool isForward = false;

    [ObservableProperty] private bool isInSeason = true;
    [ObservableProperty] private bool isPreSeason = false;
    [ObservableProperty] private bool isOffSeason = false;

    public OnboardingViewModel(DatabaseService db)
    {
        _db = db;
    }

    [RelayCommand]
    private void GoToStep2()
    {
        if (string.IsNullOrWhiteSpace(PlayerName)) return;
        IsStep1 = false;
        IsStep2 = true;
        IsStep3 = false;
    }

    [RelayCommand]
    private void GoToStep3()
    {
        IsStep1 = false;
        IsStep2 = false;
        IsStep3 = true;
    }

    [RelayCommand]
    private void SelectPosition(string position)
    {
        SelectedPosition = position switch
        {
            "Goalkeeper" => Position.Goalkeeper,
            "Fullback" => Position.Fullback,
            "Forward" => Position.Forward,
            _ => Position.Midfielder
        };
        IsGoalkeeper = SelectedPosition == Position.Goalkeeper;
        IsFullback = SelectedPosition == Position.Fullback;
        IsMidfielder = SelectedPosition == Position.Midfielder;
        IsForward = SelectedPosition == Position.Forward;
    }

    [RelayCommand]
    private void SelectSeasonMode(string mode)
    {
        SelectedSeasonMode = mode switch
        {
            "PreSeason" => SeasonMode.PreSeason,
            "OffSeason" => SeasonMode.OffSeason,
            _ => SeasonMode.InSeason
        };
        IsInSeason = SelectedSeasonMode == SeasonMode.InSeason;
        IsPreSeason = SelectedSeasonMode == SeasonMode.PreSeason;
        IsOffSeason = SelectedSeasonMode == SeasonMode.OffSeason;
    }

    [RelayCommand]
    private async Task CompleteOnboardingAsync()
    {
        var profile = await _db.GetProfileAsync();
        profile.Name = PlayerName.Trim();
        profile.Position = SelectedPosition;
        profile.SeasonMode = SelectedSeasonMode;
        profile.NextMatchDate = NextMatchDate;
        profile.HasCompletedOnboarding = true;
        await _db.SaveProfileAsync(profile);

        Preferences.Set("has_onboarded", true);

        Application.Current!.Windows[0].Page = new AppShell();
    }
}