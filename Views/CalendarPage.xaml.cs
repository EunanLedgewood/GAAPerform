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
}

public class BoolToGreenConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Color.FromArgb("#1a1a1a");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToTodayBgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#e8f5ee") : Colors.Transparent;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class EventTypeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is GAAPerform.Models.EventType type ? type switch
        {
            GAAPerform.Models.EventType.Match => "⚽",
            GAAPerform.Models.EventType.Training => "🏃",
            GAAPerform.Models.EventType.GymSession => "💪",
            GAAPerform.Models.EventType.Recovery => "🛌",
            _ => "📅"
        } : "📅";
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class EventTypeToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is GAAPerform.Models.EventType type ? type switch
        {
            GAAPerform.Models.EventType.Match => Color.FromArgb("#fff3cd"),
            GAAPerform.Models.EventType.Training => Color.FromArgb("#e8f5ee"),
            GAAPerform.Models.EventType.GymSession => Color.FromArgb("#e8f5ee"),
            GAAPerform.Models.EventType.Recovery => Color.FromArgb("#e8f0f5"),
            _ => Color.FromArgb("#f0f0eb")
        } : Color.FromArgb("#f0f0eb");
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is false;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToFontAttributesConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? FontAttributes.Bold : FontAttributes.None;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}