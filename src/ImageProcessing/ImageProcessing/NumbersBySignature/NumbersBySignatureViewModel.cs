namespace ImageProcessing.NumbersBySignature
{
    using System;
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
            this.CroppedBinaryImages = new ImagesModel();
            this.DownSizedBinaryImages = new ImagesModel();
            this.NumbersAsSmoothedBinaryImages = new ImagesModel();
            this.NumbersAsThinnedBinaryImages = new ImagesModel { DisplayIteration = true };
        }

        public async Task ProcessImagesAsync()
        {
            var rawBitmapImages = this.LoadImages();
            var binaryImages = await this.ProcessImagesToBinaryImagesAsync(rawBitmapImages);
            var croppedImages = await this.CropImagesAsync(binaryImages);
            var downSizedImages = await this.DownSizeImagesAsync(croppedImages);
            var smoothedImages = await this.ApplySmoothingAsync(downSizedImages);
            await this.ApplyThinningAsync(smoothedImages);
        }

        public ImagesModel RawNumbers { get; }

        public ImagesModel NumbersAsBinaryImage { get; }

        public ImagesModel CroppedBinaryImages { get; }

        public ImagesModel DownSizedBinaryImages { get; }

        public ImagesModel NumbersAsSmoothedBinaryImages { get; }

        public ImagesModel NumbersAsThinnedBinaryImages { get; }

        private static async Task<BinaryImage[]> ExecuteFunctionOnImagesAsync<T>(
            IEnumerable<T> images,
            Func<int, T, BinaryImage> processingFunction)
        {
            var binaryImagesDict = images.ToIndexedDictionary();

            var processedImages = await Task.WhenAll(
                binaryImagesDict
                    .Select(
                        kvp =>
                        {
                            return Task.Run(
                                () =>
                                {
                                    (int index, T binaryImage) = kvp;
                                    return processingFunction(index, binaryImage);
                                });
                        }));

            return processedImages;
        }

        private static async Task<BinaryImage[]> ExecuteFunctionOnImagesAsync(
            IEnumerable<BinaryImage> binaryImages,
            Func<int, BinaryImage, Task<BinaryImage>> processingFunctionAsync)
        {
            var binaryImagesDict = binaryImages.ToIndexedDictionary();

            var processedImages = await Task.WhenAll(
                binaryImagesDict
                    .Select(
                        kvp =>
                        {
                            return Task.Run(
                                async () =>
                                {
                                    (int index, BinaryImage binaryImage) = kvp;
                                    return await processingFunctionAsync(index, binaryImage);
                                });
                        }));

            return processedImages;
        }

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

        private async Task<BinaryImage[]> ProcessImagesToBinaryImagesAsync(IEnumerable<BitmapImage> bitmapImages)
        {
            BinaryImage ConvertingFunction(int index, BitmapImage bitmapImage)
            {
                var processedImage = BinaryImage.FromImage(bitmapImage.ToBitmap());
                this.NumbersAsBinaryImage[index] = processedImage.ToBitmapImage();
                return processedImage;
            }

            return await ExecuteFunctionOnImagesAsync(
                bitmapImages,
                ConvertingFunction);
        }

        private async Task<BinaryImage[]> CropImagesAsync(IEnumerable<BinaryImage> binaryImages)
        {
            BinaryImage CroppingFunction(int index, BinaryImage binaryImage)
            {
                var processedImage = ImageProcessor.CropAroundFigures(binaryImage);
                this.CroppedBinaryImages[index] = processedImage.ToBitmapImage();
                return processedImage;
            }

            return await ExecuteFunctionOnImagesAsync(
                binaryImages,
                CroppingFunction);
        }

        private async Task<BinaryImage[]> DownSizeImagesAsync(IEnumerable<BinaryImage> binaryImages)
        {
            BinaryImage ShrinkingFunction(int index, BinaryImage binaryImage)
            {
                var processedImage = ImageProcessor.DownSizeToHalf(binaryImage);
                this.DownSizedBinaryImages[index] = processedImage.ToBitmapImage();
                return processedImage;
            }

            return await ExecuteFunctionOnImagesAsync(
                binaryImages,
                ShrinkingFunction);
        }

        private async Task<BinaryImage[]> ApplySmoothingAsync(IEnumerable<BinaryImage> binaryImages)
        {
            BinaryImage SmoothingFunction(int index, BinaryImage binaryImage)
            {
                var processedImage = ImageProcessor.Smoothing(binaryImage);
                this.NumbersAsSmoothedBinaryImages[index] = processedImage.ToBitmapImage();
                return processedImage;
            }

            return await ExecuteFunctionOnImagesAsync(
                binaryImages,
                SmoothingFunction);
        }

        private async Task<BinaryImage[]> ApplyThinningAsync(IEnumerable<BinaryImage> binaryImages)
        {
            async Task<BinaryImage> ThinningFunctionAsync(int index, BinaryImage binaryImage)
            {
                BinaryImage img = binaryImage;
                for (int i = 0; i < 10; i++)
                {
                    img = await Task.Run(
                        () =>
                        {
                            var thinnedImg = ImageProcessor.Thinning(img);
                            this.NumbersAsThinnedBinaryImages[index] = thinnedImg.ToBitmapImage();
                            this.NumbersAsThinnedBinaryImages.SetProcessingIteration(index, i);
                            return thinnedImg;
                        });
                }

                return img;
            }

            return await ExecuteFunctionOnImagesAsync(
                binaryImages,
                ThinningFunctionAsync);
        }
    }
}