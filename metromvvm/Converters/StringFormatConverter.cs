namespace MetroMVVM.Converters
{
    using System;
    using Windows.UI.Xaml.Data;

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (targetType == typeof(string) && parameter is string)
                return String.Format(parameter as string, value);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return value;
        }
    }
}
