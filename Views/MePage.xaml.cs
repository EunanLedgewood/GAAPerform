using GAAPerform.Services;

namespace GAAPerform.Views;

public partial class MePage : ContentPage
{
    private readonly DatabaseService _db;

    public MePage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatsAsync();
    }

    private async Task LoadStatsAsync()
    {
        var email = Preferences.Get("user_email", "Player");
        var role = Preferences.Get("user_role", "Player");

        EmailLabel.Text = email;
        RoleLabel.Text = role;

        var logs = await _db.GetRecentLogsAsync(30);
        SessionCountLabel.Text = logs.Count.ToString();

        var readiness = await _db.GetReadinessScoreAsync();
        ReadinessLabel.Text = readiness.ToString();

        // Calculate streak
        int streak = 0;
        var checkDate = DateTime.Today;
        while (logs.Any(l => l.Date.Date == checkDate.Date))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }
        StreakLabel.Text = streak.ToString();

        if (logs.Any())
        {
            var recent = logs.First();
            RecentActivityLabel.Text = $"Last session: {recent.Date:dd MMM} — Feeling {recent.FeelingScore}/5";
        }
    }

    private async void OnSettingsTapped(object? sender, EventArgs e)
    {
        var settingsPage = IPlatformApplication.Current!.Services
            .GetRequiredService<SettingsPage>();
        await Navigation.PushAsync(settingsPage);
    }
}