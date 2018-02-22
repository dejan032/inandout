using System;
using System.Globalization;
using System.Windows.Data;

namespace InAndOut.Converters
{
    class TimeSpanToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan.TryParse(value.ToString(), out TimeSpan result);
            return result.TotalMilliseconds > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    
    }
}
