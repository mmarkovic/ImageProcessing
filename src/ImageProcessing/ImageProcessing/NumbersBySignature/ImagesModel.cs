namespace ImageProcessing.NumbersBySignature
{
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Contains all images of the numbers (0-9) for processing.
    /// </summary>
    public class ImagesModel
    {
        public ImagesModel(
            BitmapImage number0Image,
            BitmapImage number1Image,
            BitmapImage number2Image,
            BitmapImage number3Image,
            BitmapImage number4Image,
            BitmapImage number5Image,
            BitmapImage number6Image,
            BitmapImage number7Image,
            BitmapImage number8Image,
            BitmapImage number9Image)
        {
            this.Number0Image = number0Image;
            this.Number1Image = number1Image;
            this.Number2Image = number2Image;
            this.Number3Image = number3Image;
            this.Number4Image = number4Image;
            this.Number5Image = number5Image;
            this.Number6Image = number6Image;
            this.Number7Image = number7Image;
            this.Number8Image = number8Image;
            this.Number9Image = number9Image;
        }

        public BitmapImage Number0Image { get; }

        public BitmapImage Number1Image { get; }

        public BitmapImage Number2Image { get; }

        public BitmapImage Number3Image { get; }

        public BitmapImage Number4Image { get; }

        public BitmapImage Number5Image { get; }

        public BitmapImage Number6Image { get; }

        public BitmapImage Number7Image { get; }

        public BitmapImage Number8Image { get; }

        public BitmapImage Number9Image { get; }
    }
}