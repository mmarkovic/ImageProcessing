namespace ImageProcessing.NumbersBySignature
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Contains all images of the numbers (0-9) for processing.
    /// </summary>
    public class ImagesModel
    {
        public ImagesModel(IReadOnlyList<BitmapImage> numberImages)
        {
            this.Number0Image = numberImages[0];
            this.Number1Image = numberImages[1];
            this.Number2Image = numberImages[2];
            this.Number3Image = numberImages[3];
            this.Number4Image = numberImages[4];
            this.Number5Image = numberImages[5];
            this.Number6Image = numberImages[6];
            this.Number7Image = numberImages[7];
            this.Number8Image = numberImages[8];
            this.Number9Image = numberImages[9];
            this.Number9Image = numberImages[9];
        }

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