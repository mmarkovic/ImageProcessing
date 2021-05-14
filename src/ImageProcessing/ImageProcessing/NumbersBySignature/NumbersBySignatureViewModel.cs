namespace ImageProcessing.NumbersBySignature
{
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Windows.Media.Imaging;

    using ImageProcessing.NumbersBySignature.img;

    public class NumbersBySignatureViewModel
    {
        public NumbersBySignatureViewModel()
        {
            BitmapImage[] rawBitmapImages = 
            {
                ToImage(ImageResource._0),
                ToImage(ImageResource._1),
                ToImage(ImageResource._2),
                ToImage(ImageResource._3),
                ToImage(ImageResource._4),
                ToImage(ImageResource._5),
                ToImage(ImageResource._6),
                ToImage(ImageResource._7),
                ToImage(ImageResource._8),
                ToImage(ImageResource._9)
            };

            this.RawNumbers = new ImagesModel(rawBitmapImages);

            var rawBitmaps = rawBitmapImages.Select(ToBitmap).ToArray();
            var blackWhiteBitmaps = rawBitmaps.Select(ImageProcessor.ConvertToBlackAndWhite).ToArray();
            var blackWhiteBitmapImages = blackWhiteBitmaps.Select(ToImage).ToArray();

            this.BlackWhiteNumbers = new ImagesModel(blackWhiteBitmapImages);

            var sharpBitmaps = blackWhiteBitmaps.Select(ImageProcessor.RemoveNoise).ToArray();
            var sharpBitmapImages = sharpBitmaps.Select(ToImage).ToArray();

            this.SharpNumbers = new ImagesModel(sharpBitmapImages);
        }

        public ImagesModel RawNumbers { get; }

        public ImagesModel BlackWhiteNumbers { get; }

        public ImagesModel SharpNumbers { get; }

        private static BitmapImage ToImage(byte[] array)
        {
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        private static BitmapImage ToImage(Bitmap bitmap)
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

        private static Bitmap ToBitmap(BitmapSource bitmapImage)
        {
            using var outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);

            return new Bitmap(outStream);
        }
    }
}