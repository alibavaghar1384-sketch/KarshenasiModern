using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using KarshenasiModern.Models;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views.Converters
{
    public class BodyPartStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not BodyPartStatus status)
                return Brushes.White;

            return BodyPartColorService.GetColor(status);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}