using GAAPerform.Auth;

namespace GAAPerform.Views;

public partial class SettingsPage : ContentPage
{
    private readonly FirebaseAuthService _auth;

    public SettingsPage(FirebaseAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
        DarkModeSwitch.IsToggled = Preferences.Get("dark_mode", false);
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Profile", "Profile editing coming soon.", "OK");
    }

    private async void OnResetPasswordTapped(object sender, EventArgs e)
    {
        var email = Preferences.Get("user_email", string.Empty);
        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Error", "No email found.", "OK");
            return;
        }
        await DisplayAlert("Reset Password", $"A password reset email will be sent to {email}.", "OK");
    }

    private async void OnProTapped(object sender, EventArgs e)
    {
        await DisplayAlert("GAAPerform Pro ⭐", "Pro features coming soon!\n\nUnlock:\n• Advanced analytics\n• Unlimited coach players\n• Custom programs", "OK");
    }

    private async void OnMyTrainingTapped(object sender, EventArgs e)
    {
        await DisplayAlert("My Training", "Training preferences coming soon.", "OK");
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }

    private async void OnNotificationsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Notifications", "Notification settings coming soon.", "OK");
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://yourwebsite.com/terms");
    }

    private async void OnPrivacyTapped(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://yourwebsite.com/privacy");
    }

    private async void OnUpdatesTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Check for Updates", "You are running the latest version.", "OK");
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Log out", "Are you sure?", "Log out", "Cancel");
        if (confirm)
        {
            await _auth.SignOutAsync();
            Preferences.Remove("is_logged_in");
            Preferences.Remove("user_role");
            Preferences.Remove("user_email");

            var loginPage = IPlatformApplication.Current!.Services
                .GetRequiredService<LoginPage>();
            Application.Current!.Windows[0].Page = new NavigationPage(loginPage);
        }
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set("dark_mode", e.Value);
    }
}