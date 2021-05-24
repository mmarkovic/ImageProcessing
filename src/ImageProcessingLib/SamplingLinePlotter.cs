namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Plots the sampling lines on top of an image.
    /// </summary>
    internal class SamplingLinePlotter
    {
        private static readonly Color Miss = Color.LightBlue;
        private static readonly Color Hit = Color.Firebrick;

        internal static Bitmap Plot(BinaryImage sourceImage, IEnumerable<SamplingLine> samplingLines)
        {
            const int BitsPerByte = 8;
            var sourceImageBmp = sourceImage.ToBitmap();
            var rectangle = new Rectangle(0, 0, sourceImageBmp.Width, sourceImageBmp.Height);
            var bitmapData = sourceImageBmp.LockBits(
                rectangle,
                ImageLockMode.WriteOnly,
                sourceImageBmp.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(sourceImageBmp.PixelFormat) / BitsPerByte;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                foreach (var samplingLine in samplingLines)
                {
                    foreach (var samplingPoint in samplingLine.SamplingPoints)
                    {
                        int m = samplingPoint.Position.M;
                        int n = samplingPoint.Position.N;
                        var byteX = n * bytesPerPixel;
                        var ptrCurrentLine = ptrFirstPixel + (m * bitmapData.Stride);

                        if (sourceImage[m, n] == BinaryImage.Black)
                        {
                            ptrCurrentLine[byteX] = Hit.B; // blue
                            ptrCurrentLine[byteX + 1] = Hit.G; // green
                            ptrCurrentLine[byteX + 2] = Hit.R; // red
                        }
                        else
                        {
                            ptrCurrentLine[byteX] = Miss.B; // blue
                            ptrCurrentLine[byteX + 1] = Miss.G; // green
                            ptrCurrentLine[byteX + 2] = Miss.R; // red
                        }
                    }
                }
            }

            sourceImageBmp.UnlockBits(bitmapData);

            return sourceImageBmp;
        }
    }
}