using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private UserProfile _profile = new();

    [ObservableProperty]
    private ObservableCollection<PositionPlan> positions = new();

    [ObservableProperty]
    private PositionPlan? selectedPosition;

    [ObservableProperty]
    private string seasonModeTitle = string.Empty;

    [ObservableProperty]
    private string seasonModeDescription = string.Empty;

    [ObservableProperty]
    private bool isInSeason;

    [ObservableProperty]
    private bool isPreSeason;

    [ObservableProperty]
    private bool isOffSeason;

    public ProfileViewModel(DatabaseService db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        _profile = await _db.GetProfileAsync();

        var plans = TrainingPlanService.GetPositionPlans();
        Positions = new ObservableCollection<PositionPlan>(plans);
        SelectedPosition = plans.FirstOrDefault(p => p.Position == _profile.Position) ?? plans[2];

        SetSeasonMode(_profile.SeasonMode);
    }

    [RelayCommand]
    private async Task SelectPositionAsync(PositionPlan plan)
    {
        SelectedPosition = plan;
        _profile.Position = plan.Position;
        await _db.SaveProfileAsync(_profile);
    }

    [RelayCommand]
    private async Task SetSeasonModeAsync(string mode)
    {
        _profile.SeasonMode = mode switch
        {
            "InSeason" => SeasonMode.InSeason,
            "PreSeason" => SeasonMode.PreSeason,
            _ => SeasonMode.OffSeason
        };
        SetSeasonMode(_profile.SeasonMode);
        await _db.SaveProfileAsync(_profile);
    }

    private void SetSeasonMode(SeasonMode mode)
    {
        IsInSeason = mode == SeasonMode.InSeason;
        IsPreSeason = mode == SeasonMode.PreSeason;
        IsOffSeason = mode == SeasonMode.OffSeason;

        (SeasonModeTitle, SeasonModeDescription) = mode switch
        {
            SeasonMode.InSeason => ("In-season mode active", "Gym volume reduced. Focus is on match readiness, maintenance, and injury prevention."),
            SeasonMode.PreSeason => ("Pre-season mode active", "Build your base. High gym volume, aerobic conditioning, and technical skill work."),
            _ => ("Off-season mode active", "Recovery and general fitness. Low load, stay active, and recharge mentally.")
        };
    }
}
