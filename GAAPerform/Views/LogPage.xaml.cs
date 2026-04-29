using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class LogPage : ContentPage
{
    private readonly LogViewModel _vm;

    public LogPage(LogViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        Resources.Add("ScoreToColorConverter", new ScoreToColorConverter());
        Resources.Add("MinuteButton", new Style(typeof(Button))
        {
            Setters =
            {
                new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromArgb("#f0f0eb") },
                new Setter { Property = Button.TextColorProperty, Value = Color.FromArgb("#1a1a1a") },
                new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
                new Setter { Property = Button.CornerRadiusProperty, Value = 20 },
                new Setter { Property = Button.FontSizeProperty, Value = 13.0 },
                new Setter { Property = Button.BorderColorProperty, Value = Color.FromArgb("#e0e0d8") },
                new Setter { Property = Button.BorderWidthProperty, Value = 0.5 },
            }
        });
    }
}

public class ScoreToColorConverter : IValueConverter
{
    private static readonly Color Selected = Color.FromArgb("#1a5c35");
    private static readonly Color Unselected = Color.FromArgb("#e0e0d8");

    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is int currentScore && parameter is string paramStr && int.TryParse(paramStr, out int btnScore))
            return currentScore == btnScore ? Selected : Unselected;
        return Unselected;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}
