using GAAPerform.Views;
using GAAPerform.Services;

namespace GAAPerform;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        bool hasOnboarded = Preferences.Get("has_onboarded", false);

        if (!hasOnboarded)
            return new Window(new NavigationPage(
                IPlatformApplication.Current!.Services.GetRequiredService<OnboardingPage>()));

        return new Window(new AppShell());
    }
}