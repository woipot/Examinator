using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Examinator.converters
{
    class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                
                switch ((int)value) {
                    case 2:
                        return new SolidColorBrush(Colors.Red);
                    case 3:
                        return new SolidColorBrush(Colors.Yellow);
                    case 4:
                        return new SolidColorBrush(Colors.GreenYellow);
                    case 5:
                        return new SolidColorBrush(Colors.Green);
                }
            }
            return new SolidColorBrush(Colors.Black);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
