using GAAPerform.Services;
using GAAPerform.Views;

namespace GAAPerform;

public partial class App : Application
{
    private readonly DatabaseService _db;

    public App(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var profile = _db.GetProfileAsync().Result;

        if (!profile.HasCompletedOnboarding)
        {
            var onboardingPage = Handler!.MauiContext!.Services
                .GetRequiredService<OnboardingPage>();
            return new Window(new NavigationPage(onboardingPage));
        }

        return new Window(new AppShell());
    }
}