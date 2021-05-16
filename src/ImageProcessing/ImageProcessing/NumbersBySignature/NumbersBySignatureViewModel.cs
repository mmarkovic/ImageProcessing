namespace ImageProcessing.NumbersBySignature
{
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Imaging;

    using ImageProcessing.Annotations;
    using ImageProcessing.NumbersBySignature.img;

    using ImageProcessingLib;

    public class NumbersBySignatureViewModel : INotifyPropertyChanged
    {
        private readonly BitmapImage[] rawBitmapImages;

        private ImagesModel numbersAsBinaryImage;
        private ImagesModel numbersAsSmoothedBinaryImages;
        private ImagesModel numbersAsThinnedBinaryImages;

        public NumbersBySignatureViewModel()
        {
            this.rawBitmapImages = new[]
            {
                ImageResource._0.ToBitmapImage(),
                ImageResource._1.ToBitmapImage(),
                ImageResource._2.ToBitmapImage(),
                ImageResource._3.ToBitmapImage(),
                ImageResource._4.ToBitmapImage(),
                ImageResource._5.ToBitmapImage(),
                ImageResource._6.ToBitmapImage(),
                ImageResource._7.ToBitmapImage(),
                ImageResource._8.ToBitmapImage(),
                ImageResource._9.ToBitmapImage()
            };

            this.RawNumbers = new ImagesModel(this.rawBitmapImages);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ProcessImages()
        {
            var binaryImages = this.rawBitmapImages
                .Select(x => x.ToBitmap())
                .Select(BinaryImage.FromImage)
                .ToArray();

            var binaryBitmapImages = binaryImages
                .Select(bi => bi.ToBitmap().ToBitmapImage())
                .ToArray();

            this.NumbersAsBinaryImage = new ImagesModel(binaryBitmapImages);

            var smoothBitmaps = binaryImages
                .Select(ImageProcessor.Smoothing)
                .ToArray();

            var smoothBitmapImages = smoothBitmaps
                .Select(bi => bi.ToBitmap().ToBitmapImage())
                .ToArray();

            this.NumbersAsSmoothedBinaryImages = new ImagesModel(smoothBitmapImages);
            //
            // var thinnedBitmaps = smoothBitmaps
            //     .Select(ImageProcessor.Thinning)
            //     .ToArray();

            var thinnedBitmap0 = ImageProcessor.Thinning(ImageProcessor.Thinning(smoothBitmaps[0]));
            var thinnedBitmap1 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap0));
            var thinnedBitmap2 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap1));
            var thinnedBitmap3 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap2));
            var thinnedBitmap4 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap3));
            var thinnedBitmap5 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap4));
            var thinnedBitmap6 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap5));
            var thinnedBitmap7 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap6));
            var thinnedBitmap8 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap7));
            var thinnedBitmap9 = ImageProcessor.Thinning(ImageProcessor.Thinning(thinnedBitmap8));

            var thinnedBitmapImages = new[]
            {
                thinnedBitmap0.ToBitmap().ToBitmapImage(),
                thinnedBitmap1.ToBitmap().ToBitmapImage(),
                thinnedBitmap2.ToBitmap().ToBitmapImage(),
                thinnedBitmap3.ToBitmap().ToBitmapImage(),
                thinnedBitmap4.ToBitmap().ToBitmapImage(),
                thinnedBitmap5.ToBitmap().ToBitmapImage(),
                thinnedBitmap6.ToBitmap().ToBitmapImage(),
                thinnedBitmap7.ToBitmap().ToBitmapImage(),
                thinnedBitmap8.ToBitmap().ToBitmapImage(),
                thinnedBitmap9.ToBitmap().ToBitmapImage()
            };

            this.NumbersAsThinnedBinaryImages = new ImagesModel(thinnedBitmapImages);
        }

        public ImagesModel RawNumbers { get; }

        public ImagesModel NumbersAsBinaryImage
        {
            get => this.numbersAsBinaryImage;
            private set
            {
                this.numbersAsBinaryImage = value;
                this.OnPropertyChanged();
            }
        }

        public ImagesModel NumbersAsSmoothedBinaryImages
        {
            get => this.numbersAsSmoothedBinaryImages;
            private set
            {
                this.numbersAsSmoothedBinaryImages = value;
                this.OnPropertyChanged();
            }
        }

        public ImagesModel NumbersAsThinnedBinaryImages
        {
            get => this.numbersAsThinnedBinaryImages;
            private set
            {
                this.numbersAsThinnedBinaryImages = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}