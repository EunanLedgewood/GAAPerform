using GAAPerform.Auth;
using GAAPerform.Views;
using GAAPerform.ViewModels;

namespace GAAPerform;

public partial class App : Application
{
    private readonly FirebaseAuthService _auth;

    public App(FirebaseAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        bool isLoggedIn = Preferences.Get("is_logged_in", false);

        if (!isLoggedIn)
        {
            var loginPage = IPlatformApplication.Current!.Services
                .GetRequiredService<LoginPage>();
            return new Window(new NavigationPage(loginPage));
        }

        return new Window(new NavigationPage(new AppShell()));
    }
}