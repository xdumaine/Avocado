using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Avocado.Common
{
    class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = (string)parameter;
            var strings = str.Split("|".ToCharArray());
            if (strings.Count() != 2)
            {
                throw new IndexOutOfRangeException("Converter parameter must contain exactly two strings separated by a single pipe '|'");
            }
            return (bool)value ? strings[0] : strings[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class NullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || string.IsNullOrEmpty((string)value))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


}
