using GAAPerform.Services;
using GAAPerform.ViewModels;
using GAAPerform.Views;
using Microsoft.Extensions.Logging;
using GAAPerform.Auth;

namespace GAAPerform;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<TrainingPlanService>();
        builder.Services.AddSingleton<SessionLibraryService>();
        builder.Services.AddSingleton<FirebaseAuthService>();
        builder.Services.AddSingleton<FirestoreService>();

        // ViewModels
        builder.Services.AddTransient<WeekViewModel>();
        builder.Services.AddTransient<ReadinessViewModel>();
        builder.Services.AddTransient<LogViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<AddEventViewModel>();
        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<AddEventViewModel>();
        builder.Services.AddTransient<SessionDetailViewModel>();
        builder.Services.AddTransient<WeeklyReportViewModel>();
        builder.Services.AddTransient<WeeklyReportViewModel>();
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<CoachSquadViewModel>();
        builder.Services.AddTransient<ActiveSessionViewModel>();
        builder.Services.AddTransient<ActiveSessionViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();

        // Views
        builder.Services.AddTransient<WeekPage>();
        builder.Services.AddTransient<ReadinessPage>();
        builder.Services.AddTransient<LogPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<CalendarPage>();
        builder.Services.AddTransient<AddEventPage>();
        builder.Services.AddTransient<SessionDetailPage>();
        builder.Services.AddTransient<WeeklyReportPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CoachSquadPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<MePage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<ActiveSessionPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}