namespace ImageProcessing.NumbersBySignature
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    public static class ImageProcessor
    {
        public static Bitmap ConvertToBlackAndWhite(Bitmap image)
        {
            // Defines the threshold which determines if a pixel should be painted in white or black.
            const int thresholdValue = 120;

            var bmp = new Bitmap(image);

            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            var heightInPixels = bitmapData.Height;
            var widthInBytes = bitmapData.Width * bytesPerPixel;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    var ptrCurrentLine = ptrFirstPixel + (y * bitmapData.Stride);

                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        var oldColor = GetColor(ptrCurrentLine, x);
                        var averageColorValue = (oldColor.R + oldColor.G + oldColor.B) / 3;

                        var newColor = averageColorValue > thresholdValue ? Color.White : Color.Black;

                        SetColor(ptrCurrentLine, x, newColor);
                    }
                });
            }

            bmp.UnlockBits(bitmapData);

            return bmp;
        }

        public static Bitmap RemoveNoise(Bitmap blackWhiteImage)
        {
            // Defines the size of the matrix used to remove the noises in a black and white image.
            // NOTE: the size should be at least 3 and an odd number
            const int matrixSize = 7;

            // Needed for edge cases of the image border.
            const int matrixEdge = matrixSize / 2;

            var original = new Bitmap(blackWhiteImage);
            var target = new Bitmap(blackWhiteImage);

            var rectangle = new Rectangle(0, 0, target.Width, target.Height);
            var originalBitmapData = original.LockBits(rectangle, ImageLockMode.ReadOnly, target.PixelFormat);
            var targetBitmapData = target.LockBits(rectangle, ImageLockMode.ReadWrite, target.PixelFormat);

            var bytesPerPixel = Image.GetPixelFormatSize(target.PixelFormat) / 8;
            var heightInPixels = targetBitmapData.Height;
            var widthInBytes = targetBitmapData.Width * bytesPerPixel;

            unsafe
            {
                var ptrFirstPixelOriginal = (byte*)originalBitmapData.Scan0;
                var ptrFirstPixelTarget = (byte*)targetBitmapData.Scan0;

                Parallel.For(matrixEdge, heightInPixels - matrixEdge, y =>
                {
                    var ptrCurrentLineTarget = ptrFirstPixelTarget + (y * targetBitmapData.Stride);

                    for (var x = matrixEdge * bytesPerPixel; x < widthInBytes - matrixEdge; x = x + bytesPerPixel)
                    {
                        //
                        //  1 1 0
                        //  1 x 0    -> x is the average of all values around.
                        //  0 0 0
                        //
                        int[,] matrixColors = new int[matrixSize ,matrixSize];
                        for (int matrixX = 0; matrixX < matrixSize; matrixX++)
                        {
                            for (int matrixY = 0; matrixY < matrixSize; matrixY++)
                            {
                                int currentY = y - matrixEdge + matrixY;
                                int currentX = x - matrixEdge + matrixX;

                                byte* ptrMatrixLine = ptrFirstPixelOriginal + (currentY * originalBitmapData.Stride);
                                matrixColors[matrixX, matrixY] = (ptrMatrixLine[currentX] > 0) ? 1 : 0;
                            }
                        }

                        int average = AverageColorInMatrix(matrixColors);

                        var newColor = average == 1 ? Color.White : Color.Black;

                        SetColor(ptrCurrentLineTarget, x, newColor);
                    }
                });
            }

            original.UnlockBits(originalBitmapData);
            target.UnlockBits(targetBitmapData);

            return target;
        }

        private static int AverageColorInMatrix(int[,] matrixColors)
        {
            int sum = 0;

            for (int i = 0; i < matrixColors.GetLength(0); i++)
            {
                for (int j = 0; j < matrixColors.GetLength(1); j++)
                {
                    sum += matrixColors[i, j];
                }
            }

            int numberOfValues = matrixColors.GetLength(0) * matrixColors.GetLength(1);

            return sum > (numberOfValues / 2) ? 1 : 0;
        }

        private static unsafe Color GetColor(byte* ptrCurrentLine, int x)
        {
            return Color.FromArgb(
                ptrCurrentLine[x + 2],
                ptrCurrentLine[x + 1],
                ptrCurrentLine[x]);
        }

        private static unsafe void SetColor(byte* ptrCurrentLine, int x, Color c)
        {
            ptrCurrentLine[x + 2] = c.R;
            ptrCurrentLine[x + 1] = c.G;
            ptrCurrentLine[x] = c.B;
        }
    }
}