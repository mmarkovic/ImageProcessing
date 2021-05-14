namespace ImageProcessing.NumbersBySignature
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    public static class ImageProcessor
    {
        public static Bitmap ConvertToBlackAndWhite(Bitmap image)
        {
            // Determines the threshold which determines if a pixel should be painted
            // in white or black.
            const int thresholdValue = 120;

            var bmp = new Bitmap(image);

            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bitmapData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            unsafe
            {
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        var oldColor = GetColor(currentLine, x);
                        var averageColorValue = (oldColor.R + oldColor.G + oldColor.B) / 3;

                        var newColor = averageColorValue > thresholdValue ? Color.White : Color.Black;

                        SetColor(currentLine, x, newColor);
                    }
                });
            }

            bmp.UnlockBits(bitmapData);

            return bmp;
        }

        private static unsafe Color GetColor(byte* currentLine, int x)
        {
            return Color.FromArgb(
                currentLine[x + 2],
                currentLine[x + 1],
                currentLine[x]);
        }

        private static unsafe void SetColor(byte* currentLine, int x, Color c)
        {
            currentLine[x + 2] = c.R;
            currentLine[x + 1] = c.G;
            currentLine[x] = c.B;
        }
    }
}