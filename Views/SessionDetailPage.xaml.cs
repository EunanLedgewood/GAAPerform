using GAAPerform.Models;
using GAAPerform.Services;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class SessionDetailPage : ContentPage
{
    private readonly SessionDetailViewModel _vm;
    private readonly TrainingDay _day;
    private readonly SessionLibraryService _library;
    private readonly DatabaseService _db;

    public SessionDetailPage(SessionDetailViewModel vm, TrainingDay day)
    {
        InitializeComponent();
        _vm = vm;
        _day = day;
        _library = IPlatformApplication.Current!.Services
            .GetRequiredService<SessionLibraryService>();
        _db = IPlatformApplication.Current.Services
            .GetRequiredService<DatabaseService>();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync(_day);
    }

    private async void OnStartSessionTapped(object? sender, EventArgs e)
    {
        var profile = await _db.GetProfileAsync();
        var detail = _library.GetSessionDetail(_day, profile.Position);

        try
        {
            var auth = IPlatformApplication.Current!.Services
                .GetRequiredService<GAAPerform.Auth.FirebaseAuthService>();
            var firestore = IPlatformApplication.Current.Services
                .GetRequiredService<GAAPerform.Auth.FirestoreService>();

            if (auth.IsLoggedIn)
            {
                var token = await auth.GetTokenAsync();
                var firebaseExercises = await firestore.GetExercisesAsync(token);

                var filtered = firebaseExercises.Where(e =>
                    (e.SessionTypes.Count == 0 || e.SessionTypes.Contains(_day.Type.ToString())) &&
                    (e.Positions.Count == 0 || e.Positions.Contains(profile.Position.ToString())))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Direct Firebase fetch: {filtered.Count} exercises");
                foreach (var ex in filtered)
                    System.Diagnostics.Debug.WriteLine($"Exercise: {ex.Name} VideoUrl: {ex.VideoUrl}");

                if (filtered.Any())
                {
                    detail.Exercises = filtered.Select(e => new Exercise
                    {
                        Name = e.Name,
                        Sets = e.DefaultSets,
                        Reps = e.DefaultReps,
                        Duration = e.DefaultDuration,
                        Notes = e.Notes,
                        VideoUrl = e.VideoUrl
                    }).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Firebase fetch error: {ex.Message}");
        }

        var activeVm = IPlatformApplication.Current!.Services
            .GetRequiredService<ActiveSessionViewModel>();
        var activePage = new ActiveSessionPage(activeVm);
        activePage.LoadSession(_day, detail);
        await Navigation.PushAsync(activePage);
    }

    private async void OnWatchVideoTapped(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Exercise exercise)
        {
            if (!string.IsNullOrEmpty(exercise.VideoUrl))
            {
                await Launcher.OpenAsync(new Uri(exercise.VideoUrl));
            }
        }
    }
}