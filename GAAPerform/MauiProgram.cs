using Microsoft.Extensions.Logging;
using GAAPerform.Services;
using GAAPerform.ViewModels;
using GAAPerform.Views;

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

        // Services (singletons — shared across the app)
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<TrainingPlanService>();

        // ViewModels
        builder.Services.AddTransient<WeekViewModel>();
        builder.Services.AddTransient<ReadinessViewModel>();
        builder.Services.AddTransient<LogViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Views
        builder.Services.AddTransient<WeekPage>();
        builder.Services.AddTransient<ReadinessPage>();
        builder.Services.AddTransient<LogPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
