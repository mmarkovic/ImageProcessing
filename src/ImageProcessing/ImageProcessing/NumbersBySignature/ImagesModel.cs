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