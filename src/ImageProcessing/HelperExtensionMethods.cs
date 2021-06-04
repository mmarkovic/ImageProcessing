﻿namespace ImageProcessing
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;

    using ImageProcessingLib;

    internal static class HelperExtensionMethods
    {
        internal static BitmapImage ToBitmapImage(
            this BinaryImage binaryImage,
            BackgroundSettings background = BackgroundSettings.White)
        {
            return binaryImage.ToBitmap(background).ToBitmapImage();
        }

        internal static BitmapImage ToBitmapImage(this byte[] array)
        {
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        internal static Bitmap ToBitmap(this BitmapSource bitmapImage)
        {
            using var outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);

            return new Bitmap(outStream);
        }

        private static BitmapImage ToBitmapImage(this Image bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}