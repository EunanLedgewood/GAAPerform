namespace GAAPerform;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        AddCoachTabIfNeeded();
    }

    private void AddCoachTabIfNeeded()
    {
        var role = Preferences.Get("user_role", "Player");
        if (role == "Coach")
        {
            var coachTab = new ShellContent
            {
                Title = "Squad",
                ContentTemplate = new DataTemplate(() =>
                    IPlatformApplication.Current!.Services
                        .GetRequiredService<Views.CoachSquadPage>()),
                Route = "CoachSquadPage"
            };

            if (Items.FirstOrDefault() is TabBar tabBar)
                tabBar.Items.Add(coachTab);
        }
    }
}