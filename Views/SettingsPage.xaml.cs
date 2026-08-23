using GAAPerform.Auth;
using GAAPerform.Services;

namespace GAAPerform.Views;

public partial class SettingsPage : ContentPage
{
    private readonly FirebaseAuthService _auth;
    private readonly NotificationService _notifications;
    private bool _isLoading = true;

    public SettingsPage(FirebaseAuthService auth, NotificationService notifications)
    {
        InitializeComponent();
        _auth = auth;
        _notifications = notifications;
        LoadSettings();
    }

    private void LoadSettings()
    {
        _isLoading = true;

        // Load saved preferences
        DailyReminderSwitch.IsToggled = Preferences.Get("daily_reminder_enabled", true);
        PostSessionSwitch.IsToggled = Preferences.Get("post_session_reminder_enabled", true);
        DarkModeSwitch.IsToggled = Preferences.Get("dark_mode", false);

        var savedHour = Preferences.Get("reminder_hour", 9);
        var savedMinute = Preferences.Get("reminder_minute", 0);
        ReminderTimePicker.Time = new TimeSpan(savedHour, savedMinute, 0);

        ReminderTimePanel.IsVisible = DailyReminderSwitch.IsToggled;

        _isLoading = false;
    }

    private void OnDailyReminderToggled(object? sender, ToggledEventArgs e)
    {
        if (_isLoading) return;
        Preferences.Set("daily_reminder_enabled", e.Value);
        ReminderTimePanel.IsVisible = e.Value;

        if (e.Value)
        {
            var hour = Preferences.Get("reminder_hour", 9);
            var minute = Preferences.Get("reminder_minute", 0);
            _notifications.ScheduleDailyReminder(new TimeSpan(hour, minute, 0));
        }
        else
        {
            _notifications.CancelAll();
        }
    }

    private void OnPostSessionToggled(object? sender, ToggledEventArgs e)
    {
        if (_isLoading) return;
        Preferences.Set("post_session_reminder_enabled", e.Value);
    }

    private void OnReminderTimeChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        if (e.PropertyName != "Time") return;

        var time = ReminderTimePicker.Time ?? new TimeSpan(9, 0, 0);
        Preferences.Set("reminder_hour", time.Hours);
        Preferences.Set("reminder_minute", time.Minutes);

        if (Preferences.Get("daily_reminder_enabled", true))
            _notifications.ScheduleDailyReminder(time);
    }

    private void OnDarkModeToggled(object? sender, ToggledEventArgs e)
    {
        if (_isLoading) return;
        Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set("dark_mode", e.Value);
    }

    private async void OnProfileTapped(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("Profile", "Profile editing coming soon.", "OK");
    }

    private async void OnResetPasswordTapped(object? sender, EventArgs e)
    {
        var email = Preferences.Get("user_email", string.Empty);
        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlertAsync("Error", "No email found.", "OK");
            return;
        }

        var (success, error) = await _auth.SendPasswordResetAsync(email);
        if (success)
            await DisplayAlertAsync("Reset Password", $"Password reset email sent to {email}.", "OK");
        else
            await DisplayAlertAsync("Error", error ?? "Failed to send reset email.", "OK");
    }

    private async void OnProTapped(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("GAAPerform Pro ⭐", "Pro features coming soon!\n\nUnlock:\n• Advanced analytics\n• Unlimited coach players\n• Custom programs", "OK");
    }

    private async void OnMyTrainingTapped(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("My Training", "Training preferences coming soon.", "OK");
    }

    private async void OnTermsTapped(object? sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://yourwebsite.com/terms");
    }

    private async void OnPrivacyTapped(object? sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://yourwebsite.com/privacy");
    }

    private async void OnUpdatesTapped(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("Check for Updates", "You are running the latest version.", "OK");
    }

    private async void OnSeedExercisesTapped(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Seed Exercise Library",
            "This will add the default exercise library to Firebase. Only do this once.",
            "Seed", "Cancel");

        if (confirm)
        {
            try
            {
                var token = await _auth.GetTokenAsync();
                var firestore = IPlatformApplication.Current!.Services
                    .GetRequiredService<GAAPerform.Auth.FirestoreService>();
                await firestore.SeedExerciseLibraryAsync(token);
                await DisplayAlertAsync("Done", "Exercise library seeded successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }

    private async void OnLogoutTapped(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Log out", "Are you sure?", "Log out", "Cancel");
        if (confirm)
        {
            _notifications.CancelAll();
            await _auth.SignOutAsync();
            Preferences.Remove("is_logged_in");
            Preferences.Remove("user_role");
            Preferences.Remove("user_email");

            var loginPage = IPlatformApplication.Current!.Services
                .GetRequiredService<LoginPage>();
            Application.Current!.Windows[0].Page = new NavigationPage(loginPage);
        }
    }
}