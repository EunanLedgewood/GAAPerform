using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class AddEventPage : ContentPage
{
    private readonly AddEventViewModel _vm;
    private readonly CalendarViewModel _calendarVm;

    public AddEventPage(AddEventViewModel vm, CalendarViewModel calendarVm)
    {
        InitializeComponent();
        _vm = vm;
        _calendarVm = calendarVm;
        BindingContext = vm;
    }

    public void SetDate(DateTime date)
    {
        _vm.SetDate(date);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        await _vm.SaveEventCommand.ExecuteAsync(null);
        await _calendarVm.RefreshAsync();
        await Navigation.PopAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _calendarVm.RefreshAsync();
    }
}