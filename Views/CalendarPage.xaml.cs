using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _vm;

    public CalendarPage(CalendarViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }

    private async void OnAddEventTapped(object sender, EventArgs e)
    {
        var addPage = IPlatformApplication.Current!.Services
            .GetRequiredService<AddEventPage>();
        addPage.SetDate(_vm.SelectedDate);
        await Navigation.PushAsync(addPage);
    }
}