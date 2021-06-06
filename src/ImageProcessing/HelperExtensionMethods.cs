namespace ImageProcessing
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using ImageProcessingLib;

    internal static class HelperExtensionMethods
    {
        internal static BitmapImage ToBitmapImage(
            this BinaryImage binaryImage,
            BinaryImageColorSettings colorSettings)
        {
            return binaryImage.ToBitmap(colorSettings).ToBitmapImage();
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

        internal static BitmapImage ToBitmapImage(this Image bitmap)
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

        internal static BinaryImage ToBinaryImage(this ImageSource image)
        {
            var bitmapImage = image as BitmapImage;
            var bmp = bitmapImage!.ToBitmap();
            return BinaryImage.FromImage(bmp);
        }

        internal static void SaveToAppDirectory(this BitmapImage image, string imageFileName)
        {
            string currentDirectory = Environment.CurrentDirectory;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            string filePath = Path.Combine(currentDirectory, imageFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }
    }
}