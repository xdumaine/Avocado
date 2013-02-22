namespace MetroMVVM.Converters
{
    using System;
    using Windows.UI.Xaml.Data;

    public class BoolConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            BoolConverterMode mode = GetMode(parameter);
            switch (mode)
            {
                case BoolConverterMode.FalseIfIsNullOrWhiteSpace:
                    return !string.IsNullOrWhiteSpace((string)value);
                case BoolConverterMode.FalseIfIsNullOrEmpty:
                    return !string.IsNullOrEmpty((string)value);
                case BoolConverterMode.TrueIfIsNullOrWhiteSpace:
                    return string.IsNullOrWhiteSpace((string)value);
                case BoolConverterMode.TrueIfIsNullOrEmpty:
                    return string.IsNullOrEmpty((string)value);
                default:
                    return !string.IsNullOrWhiteSpace((string)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return value;
        }

        #endregion

        private static BoolConverterMode GetMode(object parameter)
        {
            if (parameter == null)
            {
                return BoolConverterMode.FalseIfIsNullOrWhiteSpace;
            }

            else if (parameter is BoolConverterMode)
            {
                return (BoolConverterMode)parameter;
            }
            else
            {
                return (BoolConverterMode)Enum.Parse(typeof(BoolConverterMode), (string)parameter, false);
            }
        }
    }

    public enum BoolConverterMode
    {
        FalseIfIsNullOrWhiteSpace,
        FalseIfIsNullOrEmpty,
        TrueIfIsNullOrWhiteSpace,
        TrueIfIsNullOrEmpty,
    }
}
