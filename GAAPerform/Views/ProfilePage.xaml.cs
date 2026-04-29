using GAAPerform.Models;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;

    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        Resources.Add("BoolToActiveModeConverter", new BoolToActiveModeConverter());
        Resources.Add("BoolToActiveModeTextConverter", new BoolToActiveModeTextConverter());
        // Note: IsSelectedToBg/Stroke converters require multi-binding — simplified to flat colour for MVP
        Resources.Add("IsSelectedToBgConverter", new AlwaysCardBgConverter());
        Resources.Add("IsSelectedToStrokeConverter", new AlwaysBorderConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

public class BoolToActiveModeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#c8a84b") : Colors.Transparent;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToActiveModeTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#3a2800") : Color.FromArgb("#6b6b6b");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

// Simplified — full multi-binding for selected state can be added later
public class AlwaysCardBgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => Color.FromArgb("#ffffff");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public class AlwaysBorderConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => Color.FromArgb("#e0e0d8");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}
