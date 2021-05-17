namespace ImageProcessing.NumbersBySignature
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    using ImageProcessing.NumbersBySignature.img;

    using ImageProcessingLib;

    public class NumbersBySignatureViewModel
    {
        public NumbersBySignatureViewModel()
        {
            this.RawNumbers = new ImagesModel();
            this.NumbersAsBinaryImage = new ImagesModel();
            this.NumbersAsSmoothedBinaryImages = new ImagesModel();
            this.NumbersAsThinnedBinaryImages = new ImagesModel();
        }

        public async Task ProcessImagesAsync()
        {
            var rawBitmapImages = this.LoadImages();
            var binaryImages = await this.ProcessImagesToBinaryImagesAsync(rawBitmapImages);
            var smoothBitmaps = await this.ApplySmoothingAsync(binaryImages);
            await this.ApplyThinningAsync(smoothBitmaps);
        }

        public ImagesModel RawNumbers { get; }

        public ImagesModel NumbersAsBinaryImage { get; }

        public ImagesModel NumbersAsSmoothedBinaryImages { get; }

        public ImagesModel NumbersAsThinnedBinaryImages { get; }

        private BitmapImage[] LoadImages()
        {
            var rawImages = new List<byte[]>
            {
                ImageResource._0,
                ImageResource._1,
                ImageResource._2,
                ImageResource._3,
                ImageResource._4,
                ImageResource._5,
                ImageResource._6,
                ImageResource._7,
                ImageResource._8,
                ImageResource._9
            };

            for (var i = 0; i < rawImages.Count; i++)
            {
                this.RawNumbers[i] = rawImages[i].ToBitmapImage();
            }

            return this.RawNumbers.GetImages().ToArray();
        }

        private async Task<BinaryImage[]> ApplyThinningAsync(IEnumerable<BinaryImage> binaryImages)
        {
            var binaryImagesDict = binaryImages.ToIndexedDictionary();

            var thinnedBitmapImages = await Task.WhenAll(
                binaryImagesDict
                    .Select(
                        kvp =>
                        {
                            return Task.Run(
                                () =>
                                {
                                    (int index, var binaryImage) = kvp;
                                    var thinnedImg = ImageProcessor.Thinning(binaryImage);
                                    this.NumbersAsThinnedBinaryImages[index] = thinnedImg.ToBitmapImage();
                                    return thinnedImg;
                                });
                        }));

            return thinnedBitmapImages;
        }

        private async Task<BinaryImage[]> ApplySmoothingAsync(IEnumerable<BinaryImage> binaryImages)
        {
            var binaryImagesDict = binaryImages.ToIndexedDictionary();

            var smoothBitmapImages = await Task.WhenAll(
                binaryImagesDict
                    .Select(
                        kvp =>
                        {
                            return Task.Run(
                                () =>
                                {
                                    (int index, var binaryImage) = kvp;
                                    var thinnedImg = ImageProcessor.Smoothing(binaryImage);
                                    this.NumbersAsSmoothedBinaryImages[index] = thinnedImg.ToBitmapImage();
                                    return thinnedImg;
                                });
                        }));

            return smoothBitmapImages;
        }

        private async Task<BinaryImage[]> ProcessImagesToBinaryImagesAsync(IEnumerable<BitmapImage> bitmapImages)
        {
            var binaryImagesDict = bitmapImages.ToIndexedDictionary();

            var binaryImages = await Task.WhenAll(
                binaryImagesDict
                    .Select(
                        kvp =>
                        {
                            return Task.Run(
                                () =>
                                {
                                    (int index, var bitmapImage) = kvp;
                                    var binaryImage = BinaryImage.FromImage(bitmapImage.ToBitmap());
                                    this.NumbersAsBinaryImage[index] = binaryImage.ToBitmapImage();
                                    return binaryImage;
                                });
                        }));

            return binaryImages;
        }
    }
}