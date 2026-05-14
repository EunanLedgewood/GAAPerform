using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class WeeklyReportPage : ContentPage
{
    private readonly WeeklyReportViewModel _vm;

    public WeeklyReportPage(WeeklyReportViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        Resources.Add("BarColorConverter", new BarColorConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

public class BarColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Color.FromArgb("#e0e0d8");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}