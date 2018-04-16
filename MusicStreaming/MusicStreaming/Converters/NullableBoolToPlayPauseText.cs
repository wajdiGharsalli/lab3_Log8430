using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicStreaming.Converters
{
    class NullableBoolToPlayPauseText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = "Play";
            if ((bool?)value == true)
                text = "Pause";
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Pause")
                return true;
            return false;
        }
    }
}
