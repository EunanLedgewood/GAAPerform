using GAAPerform.Models;

namespace GAAPerform.Views;

public class BoolToSelectedBgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#e8f5ee") : Color.FromArgb("#ffffff");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToSelectedStrokeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Color.FromArgb("#e0e0d8");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToActiveModeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#c8a84b") : Colors.Transparent;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToActiveModeTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#3a2800") : Color.FromArgb("#6b6b6b");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
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

public class BoolToFontAttributesConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? FontAttributes.Bold : FontAttributes.None;
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

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class EventTypeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is EventType type ? type switch
        {
            EventType.Match => "⚽",
            EventType.Training => "🏃",
            EventType.GymSession => "💪",
            EventType.Recovery => "🛌",
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
        return value is EventType type ? type switch
        {
            EventType.Match => Color.FromArgb("#fff3cd"),
            EventType.Training => Color.FromArgb("#e8f5ee"),
            EventType.GymSession => Color.FromArgb("#e8f5ee"),
            EventType.Recovery => Color.FromArgb("#e8f0f5"),
            _ => Color.FromArgb("#f0f0eb")
        } : Color.FromArgb("#f0f0eb");
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class SessionIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is GAAPerform.Models.SessionType type ? type switch
        {
            GAAPerform.Models.SessionType.Match => "⚽",
            GAAPerform.Models.SessionType.Strength => "💪",
            GAAPerform.Models.SessionType.Field => "🏃",
            GAAPerform.Models.SessionType.Recovery => "🛌",
            GAAPerform.Models.SessionType.Activation => "⚡",
            _ => "—"
        } : "—";
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToCheckBgConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Colors.Transparent;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToCheckColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? Color.FromArgb("#1a5c35") : Color.FromArgb("#d0d0c8");
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class SessionTypeToCheckVisibleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is SessionType t && t != SessionType.Rest && t != SessionType.Match;
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class ExpandCollapseTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => value is true ? "▲ Tap to collapse" : "▼ Tap to see exercises";
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

public class ExerciseFieldTypeToLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is ExerciseFieldType type ? type switch
        {
            ExerciseFieldType.WeightsAndReps => "⚖️ Weights",
            ExerciseFieldType.TimeAndDifficulty => "⏱️ Cardio",
            ExerciseFieldType.RepsOnly => "🔄 Bodyweight",
            ExerciseFieldType.Custom => "✏️ Custom",
            _ => "⚖️ Weights"
        } : "⚖️ Weights";
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}