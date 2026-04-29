using GAAPerform.Models;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class WeekPage : ContentPage
{
    private readonly WeekViewModel _vm;

    public WeekPage(WeekViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        // Register converters in resources
        Resources.Add("SessionTypeToIconConverter", new SessionTypeToIconConverter());
        Resources.Add("BoolToCheckColorConverter", new BoolToCheckColorConverter());
        Resources.Add("BoolToCheckBgConverter", new BoolToCheckBgConverter());
        Resources.Add("SessionTypeToCheckVisibleConverter", new SessionTypeToCheckVisibleConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

// ── Converters ──────────────────────────────────────────────

public class SessionTypeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is SessionType type ? type switch
        {
            SessionType.Match => "⚽",
            SessionType.Strength => "💪",
            SessionType.Field => "🏃",
            SessionType.Recovery => "🛌",
            SessionType.Activation => "⚡",
            _ => "—"
        } : "—";
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToCheckColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Color.FromArgb("#d0d0c8");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToCheckBgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Colors.Transparent;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public class SessionTypeToCheckVisibleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is SessionType t && t != SessionType.Rest && t != SessionType.Match;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}
