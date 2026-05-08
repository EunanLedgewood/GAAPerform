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
        Resources.Add("SessionIconConverter", new SessionIconConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

public class SessionIconConverter : IValueConverter
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

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}