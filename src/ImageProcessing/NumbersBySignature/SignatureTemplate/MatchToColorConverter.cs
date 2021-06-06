namespace ImageProcessing.NumbersBySignature.SignatureTemplate
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using MediaColor = System.Windows.Media.Color;
    using Color = System.Drawing.Color;

    [ValueConversion(typeof(bool?), typeof(SolidColorBrush))]
    public class MatchToColorConverter : IValueConverter
    {
        private static readonly Color DefaultColor = Color.Transparent;
        private static readonly Color MatchColor = Color.Green;
        private static readonly Color NoMatchColor = Color.Firebrick;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as bool?).HasValue)
            {
                bool isMatch = ((bool?)value).Value;
                var color = isMatch ? MatchColor : NoMatchColor;
                return new SolidColorBrush(ToMediaColor(color));
            }

            return new SolidColorBrush(ToMediaColor(DefaultColor));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static MediaColor ToMediaColor(Color c)
        {
            return MediaColor.FromRgb(c.R, c.G, c.B);
        }
    }
}