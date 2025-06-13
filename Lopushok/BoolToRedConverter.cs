using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Lopushok;

public class BoolToRedConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? Brushes.LightCoral : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null!;
}