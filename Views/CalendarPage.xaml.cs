using GAAPerform.Models;
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
        await _vm.RefreshAsync();
    }

    private async void OnEventSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not CalendarEvent selectedEvent)
            return;

        // Deselect
        EventsCollection.SelectedItem = null;

        bool confirm = await DisplayAlert(
            "Delete event",
            $"Delete '{selectedEvent.Title}'?",
            "Delete",
            "Cancel");

        if (confirm)
        {
            await _vm.DeleteEventCommand.ExecuteAsync(selectedEvent);
        }
    }
}