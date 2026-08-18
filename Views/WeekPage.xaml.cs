using GAAPerform.Models;
using GAAPerform.Services;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class WeekPage : ContentPage
{
    private readonly WeekViewModel _vm;
    private readonly DatabaseService _db;

    public WeekPage(WeekViewModel vm, DatabaseService db)
    {
        InitializeComponent();
        _vm = vm;
        _db = db;
        BindingContext = vm;
        _vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeekViewModel.SelectedDay) && _vm.SelectedDay is not null)
        {
            var day = _vm.SelectedDay;
            _vm.SelectedDay = null;

            var profile = await _db.GetProfileAsync();

            var library = IPlatformApplication.Current!.Services
                .GetRequiredService<SessionLibraryService>();
            var detail = library.GetSessionDetail(day, profile.Position);

            // Fetch Firebase exercises
            try
            {
                var auth = IPlatformApplication.Current.Services
                    .GetRequiredService<GAAPerform.Auth.FirebaseAuthService>();
                var firestore = IPlatformApplication.Current.Services
                    .GetRequiredService<GAAPerform.Auth.FirestoreService>();

                if (auth.IsLoggedIn)
                {
                    var token = await auth.GetTokenAsync();
                    var firebaseExercises = await firestore.GetExercisesAsync(token);
                    var filtered = firebaseExercises.Where(ex =>
                        (ex.SessionTypes.Count == 0 || ex.SessionTypes.Contains(day.Type.ToString())) &&
                        (ex.Positions.Count == 0 || ex.Positions.Contains(profile.Position.ToString())))
                        .ToList();

                    if (filtered.Any())
                    {
                        detail.Exercises = filtered.Select(ex => new GAAPerform.Models.Exercise
                        {
                            Name = ex.Name,
                            Sets = ex.DefaultSets,
                            Reps = ex.DefaultReps,
                            Duration = ex.DefaultDuration,
                            Notes = ex.Notes,
                            VideoUrl = ex.VideoUrl
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
            activePage.LoadSession(day, detail);
            await Navigation.PushAsync(activePage);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}