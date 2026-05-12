using GAAPerform.Services;
using GAAPerform.ViewModels;
using GAAPerform.Views;
using Microsoft.Extensions.Logging;

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

        // Views
        builder.Services.AddTransient<WeekPage>();
        builder.Services.AddTransient<ReadinessPage>();
        builder.Services.AddTransient<LogPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<CalendarPage>();
        builder.Services.AddTransient<AddEventPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}