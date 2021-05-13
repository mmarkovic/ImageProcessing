namespace ImageProcessing.NumbersBySignature
{
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Media.Imaging;

    using ImageProcessing.NumbersBySignature.img;

    public class NumbersBySignatureViewModel
    {
        public NumbersBySignatureViewModel()
        {
            var number0Image = ToImage(ImageResource._0);
            var number1Image = ToImage(ImageResource._1);
            var number2Image = ToImage(ImageResource._2);
            var number3Image = ToImage(ImageResource._3);
            var number4Image = ToImage(ImageResource._4);
            var number5Image = ToImage(ImageResource._5);
            var number6Image = ToImage(ImageResource._6);
            var number7Image = ToImage(ImageResource._7);
            var number8Image = ToImage(ImageResource._8);
            var number9Image = ToImage(ImageResource._9);

            this.RawNumbers = new ImagesModel(
                number0Image,
                number1Image,
                number2Image,
                number3Image,
                number4Image,
                number5Image,
                number6Image,
                number7Image,
                number8Image,
                number9Image);

            var number0Bitmap = ToBitmap(number0Image);
            var number1Bitmap = ToBitmap(number1Image);
            var number2Bitmap = ToBitmap(number2Image);
            var number3Bitmap = ToBitmap(number3Image);
            var number4Bitmap = ToBitmap(number4Image);
            var number5Bitmap = ToBitmap(number5Image);
            var number6Bitmap = ToBitmap(number6Image);
            var number7Bitmap = ToBitmap(number7Image);
            var number8Bitmap = ToBitmap(number8Image);
            var number9Bitmap = ToBitmap(number9Image);

            var blackWhite0Bitmap = ImageProcessor.ConvertToBlackAndWhite(number0Bitmap);
            var blackWhite1Bitmap = ImageProcessor.ConvertToBlackAndWhite(number1Bitmap);
            var blackWhite2Bitmap = ImageProcessor.ConvertToBlackAndWhite(number2Bitmap);
            var blackWhite3Bitmap = ImageProcessor.ConvertToBlackAndWhite(number3Bitmap);
            var blackWhite4Bitmap = ImageProcessor.ConvertToBlackAndWhite(number4Bitmap);
            var blackWhite5Bitmap = ImageProcessor.ConvertToBlackAndWhite(number5Bitmap);
            var blackWhite6Bitmap = ImageProcessor.ConvertToBlackAndWhite(number6Bitmap);
            var blackWhite7Bitmap = ImageProcessor.ConvertToBlackAndWhite(number7Bitmap);
            var blackWhite8Bitmap = ImageProcessor.ConvertToBlackAndWhite(number8Bitmap);
            var blackWhite9Bitmap = ImageProcessor.ConvertToBlackAndWhite(number9Bitmap);

            this.BlackWhiteNumbers = new ImagesModel(
                ToImage(blackWhite0Bitmap),
                ToImage(blackWhite1Bitmap),
                ToImage(blackWhite2Bitmap),
                ToImage(blackWhite3Bitmap),
                ToImage(blackWhite4Bitmap),
                ToImage(blackWhite5Bitmap),
                ToImage(blackWhite6Bitmap),
                ToImage(blackWhite7Bitmap),
                ToImage(blackWhite8Bitmap),
                ToImage(blackWhite9Bitmap));
        }

        public ImagesModel RawNumbers { get; }

        public ImagesModel BlackWhiteNumbers { get; }

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