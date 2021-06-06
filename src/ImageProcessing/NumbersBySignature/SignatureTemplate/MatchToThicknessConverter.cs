namespace ImageProcessing.NumbersBySignature.SignatureTemplate
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool?), typeof(Thickness))]
    public class MatchToThicknessConverter : IValueConverter
    {
        private static readonly Thickness BorderWithValue = new Thickness(3);
        private static readonly Thickness BorderWithoutValue = new Thickness(0);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as bool?).HasValue && ((bool?)value).Value)
            {
                return BorderWithValue;
            }

            return BorderWithoutValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}