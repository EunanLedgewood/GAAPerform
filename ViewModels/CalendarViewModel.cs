using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Models;
using GAAPerform.Services;
using GAAPerform.Views;
using System.Collections.ObjectModel;

namespace GAAPerform.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private int currentYear;
    [ObservableProperty] private int currentMonth;
    [ObservableProperty] private string monthLabel = string.Empty;
    [ObservableProperty] private ObservableCollection<CalendarDay> calendarDays = new();
    [ObservableProperty] private ObservableCollection<CalendarEvent> selectedDayEvents = new();
    [ObservableProperty] private string selectedDateLabel = string.Empty;
    [ObservableProperty] private bool hasSelectedDayEvents;
    [ObservableProperty] private DateTime selectedDate = DateTime.Today;

    public CalendarViewModel(DatabaseService db)
    {
        _db = db;
        currentYear = DateTime.Today.Year;
        currentMonth = DateTime.Today.Month;
    }

    public async Task LoadAsync()
    {
        MonthLabel = new DateTime(CurrentYear, CurrentMonth, 1).ToString("MMMM yyyy");
        await BuildCalendarAsync();
    }

    private async Task BuildCalendarAsync()
    {
        var events = await _db.GetEventsForMonthAsync(CurrentYear, CurrentMonth);
        var days = new ObservableCollection<CalendarDay>();

        var firstDay = new DateTime(CurrentYear, CurrentMonth, 1);
        var daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);

        // Add empty slots for days before the 1st (Monday start)
        int startOffset = ((int)firstDay.DayOfWeek + 6) % 7;
        for (int i = 0; i < startOffset; i++)
            days.Add(new CalendarDay { IsEmpty = true });

        // Add actual days
        for (int d = 1; d <= daysInMonth; d++)
        {
            var date = new DateTime(CurrentYear, CurrentMonth, d);
            var dayEvents = events.Where(e => e.Date.Date == date.Date).ToList();

            days.Add(new CalendarDay
            {
                Date = date,
                DayNumber = d,
                IsToday = date.Date == DateTime.Today,
                IsEmpty = false,
                HasMatch = dayEvents.Any(e => e.EventType == EventType.Match),
                HasTraining = dayEvents.Any(e => e.EventType == EventType.Training || e.EventType == EventType.GymSession),
                HasOther = dayEvents.Any(e => e.EventType == EventType.Recovery || e.EventType == EventType.Other),
                EventCount = dayEvents.Count
            });
        }

        CalendarDays = days;
        await SelectDateAsync(DateTime.Today.Month == CurrentMonth && DateTime.Today.Year == CurrentYear
            ? DateTime.Today : firstDay);
    }

    [RelayCommand]
    private async Task SelectDateAsync(DateTime date)
    {
        SelectedDate = date;
        SelectedDateLabel = date.ToString("dddd d MMMM");
        var events = await _db.GetEventsForDateAsync(date);
        SelectedDayEvents = new ObservableCollection<CalendarEvent>(events);
        HasSelectedDayEvents = SelectedDayEvents.Any();
    }

    [RelayCommand]
    private async Task PreviousMonthAsync()
    {
        if (CurrentMonth == 1) { CurrentMonth = 12; CurrentYear--; }
        else CurrentMonth--;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextMonthAsync()
    {
        if (CurrentMonth == 12) { CurrentMonth = 1; CurrentYear++; }
        else CurrentMonth++;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteEventAsync(CalendarEvent calEvent)
    {
        await _db.DeleteEventAsync(calEvent);
        await SelectDateAsync(SelectedDate);
        await BuildCalendarAsync();
    }

    [RelayCommand]
    private async Task AddEventAsync()
    {
        var addPage = IPlatformApplication.Current!.Services.GetRequiredService<AddEventPage>();
        addPage.SetDate(SelectedDate);
        await Shell.Current.Navigation.PushAsync(addPage);
    }

    public async Task RefreshAsync()
    {
        await BuildCalendarAsync();
    }
}

public class CalendarDay
{
    public DateTime Date { get; set; }
    public int DayNumber { get; set; }
    public bool IsToday { get; set; }
    public bool IsEmpty { get; set; }
    public bool HasMatch { get; set; }
    public bool HasTraining { get; set; }
    public bool HasOther { get; set; }
    public int EventCount { get; set; }
}