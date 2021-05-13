namespace ImageProcessing.NumbersBySignature
{
    using System.Drawing;

    public static class ImageProcessor
    {
        public static Bitmap ConvertToBlackAndWhite(Bitmap image)
        {
            // Determines the threshold which determines if a pixel should be painted
            // in white or black.
            const int thresholdValue = 120;

            var bmp = new Bitmap(image);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var oldColor = bmp.GetPixel(x, y);
                    var averageColorValue = (oldColor.R + oldColor.G + oldColor.B) / 3;
                    var newColor = averageColorValue > thresholdValue ? Color.White : Color.Black;

                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }
    }
}