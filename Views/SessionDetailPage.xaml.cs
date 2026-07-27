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

    private async void OnStartSessionTapped(object sender, EventArgs e)
    {
        var profile = await _db.GetProfileAsync();
        var detail = _library.GetSessionDetail(_day, profile.Position);

        var activeVm = IPlatformApplication.Current!.Services
            .GetRequiredService<ActiveSessionViewModel>();
        var activePage = new ActiveSessionPage(activeVm);
        activePage.LoadSession(_day, detail);
        await Navigation.PushAsync(activePage);
    }
}