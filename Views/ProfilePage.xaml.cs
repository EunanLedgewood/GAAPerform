using GAAPerform.Auth;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;

    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnLogoutTapped(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Log out",
            "Are you sure you want to log out?",
            "Log out",
            "Cancel");

        if (confirm)
        {
            var auth = IPlatformApplication.Current!.Services
                .GetRequiredService<FirebaseAuthService>();
            await auth.SignOutAsync();
            Preferences.Remove("is_logged_in");
            Preferences.Remove("user_role");

            var loginPage = IPlatformApplication.Current.Services
                .GetRequiredService<LoginPage>();
            Application.Current!.Windows[0].Page = new NavigationPage(loginPage);
        }
    }
}