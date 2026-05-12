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

        Resources.Add("BoolToActiveModeConverter", new BoolToActiveModeConverter());
        Resources.Add("BoolToActiveModeTextConverter", new BoolToActiveModeTextConverter());
        Resources.Add("BoolToSelectedStrokeConverter", new BoolToSelectedStrokeConverter());
    }

    public void SetDate(DateTime date)
    {
        _vm.SetDate(date);
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _calendarVm.RefreshAsync();
    }
}