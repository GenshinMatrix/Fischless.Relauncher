using System.Globalization;
using System.Windows.Data;

namespace Fischless.Relauncher.Views.Controls;

/// <summary>
/// Converts the element's ActualWidth to a negative half value
/// so that the tooltip appears horizontally centered.
/// </summary>
public class TooltipCenterOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double width && !double.IsNaN(width))
        {
            // Return negative half width for centering
            return -width / 2d + 16d;
        }
        return 0d;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}