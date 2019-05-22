using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Examinator.mvvm.models.subModels;

namespace Examinator.converters
{
    class SelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)values[1])
                return new SolidColorBrush(Colors.Blue);
            if ((bool)values[0])
                return new SolidColorBrush(Colors.Gray);
            return new SolidColorBrush(Colors.DodgerBlue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
