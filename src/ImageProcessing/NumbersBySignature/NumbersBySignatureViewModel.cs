namespace ImageProcessing.NumbersBySignature
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    using ImageProcessing.NumbersBySignature.img;

    using ImageProcessingLib;

    public class NumbersBySignatureViewModel
    {
        private const int SamplingRate = 180;

        public NumbersBySignatureViewModel()
        {
            this.RawNumbers = new ImagesModel();
            this.NumbersAsBinaryImage = new ImagesModel();
            this.CroppedBinaryImages = new ImagesModel();
            this.DownSizedBinaryImages = new ImagesModel();
            this.NumbersAsSmoothedBinaryImages = new ImagesModel();
            this.NumbersAsThinnedBinaryImages = new ImagesModel { DisplayIteration = true };
            this.SampledImages = new ImagesModel();
            this.SignatureOfNumbers = new ImagesModel();
        }

        public async Task ProcessImagesAsync()
        {
            var rawBitmapImages = this.LoadImages();
            var binaryImages = await this.ProcessImagesToBinaryImagesAsync(rawBitmapImages);
            var croppedImages = await this.CropImagesAsync(binaryImages);
            var downSizedImages = await this.DownSizeImagesAsync(croppedImages);
            var smoothedImages = await this.ApplySmoothingAsync(downSizedImages);
            var thinnedImages = await this.ApplyThinningAsync(smoothedImages);
            await this.DrawSignatureSamplingLinesOn(thinnedImages);
            await this.CalculateSignatureAsync(thinnedImages);
        }

        public ImagesModel RawNumbers { get; }

        public ImagesModel NumbersAsBinaryImage { get; }

        public ImagesModel CroppedBinaryImages { get; }

        public ImagesModel DownSizedBinaryImages { get; }

        public ImagesModel NumbersAsSmoothedBinaryImages { get; }

        public ImagesModel NumbersAsThinnedBinaryImages { get; }

        public ImagesModel SampledImages { get; }

        public ImagesModel SignatureOfNumbers { get; }

        private static async Task<TReturn[]> ExecuteFunctionOnImagesAsync<TIn, TReturn>(
            IEnumerable<TIn> images,
            Func<int, TIn, TReturn> processingFunction)
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
                                    (int index, TIn binaryImage) = kvp;
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

        private static void WriteImageToAppDirectory(BitmapSource image, string imageName)
        {
            string currentDirectory = Environment.CurrentDirectory;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            string filePath = Path.Combine(currentDirectory, imageName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }

        private IEnumerable<BitmapImage> LoadImages()
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
                var processedBitmapImage = processedImage.ToBitmapImage();
                this.NumbersAsBinaryImage[index] = processedBitmapImage;
                WriteImageToAppDirectory(processedBitmapImage, $"binary_{index:00}.png");
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
                var processedBitmapImage = processedImage.ToBitmapImage();
                this.CroppedBinaryImages[index] = processedBitmapImage;
                WriteImageToAppDirectory(processedBitmapImage, $"cropped_{index:00}.png");
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
                var processedBitmapImage = processedImage.ToBitmapImage();
                this.DownSizedBinaryImages[index] = processedBitmapImage;
                WriteImageToAppDirectory(processedBitmapImage, $"downSized_{index:00}.png");
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
                var processedBitmapImage = processedImage.ToBitmapImage();
                this.NumbersAsSmoothedBinaryImages[index] = processedBitmapImage;
                WriteImageToAppDirectory(processedBitmapImage, $"smoothed_{index:00}.png");
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
                BinaryImage previousImg = binaryImage;
                for (int i = 0; i < 35; i++)
                {
                    img = await Task.Run(
                        () =>
                        {
                            var thinnedImg = ImageProcessor.Thinning(img);
                            this.NumbersAsThinnedBinaryImages[index] = thinnedImg.ToBitmapImage();
                            this.NumbersAsThinnedBinaryImages.SetProcessingIteration(index, i);
                            return thinnedImg;
                        });

                    if (img.Equals(previousImg))
                    {
                        // no changes after last thinning -> abort
                        return img;
                    }

                    previousImg = img;
                }

                return img;
            }

            return await ExecuteFunctionOnImagesAsync(
                binaryImages,
                ThinningFunctionAsync);
        }

        private async Task DrawSignatureSamplingLinesOn(IEnumerable<BinaryImage> binaryImages)
        {
            Bitmap FunctionAsync(int index, BinaryImage binaryImage)
            {
                var processedImage = ImageProcessor.DrawSignatureSamplingLinesOn(binaryImage, SamplingRate);
                var processedBitmapImage = processedImage.ToBitmapImage();
                this.SampledImages[index] = processedBitmapImage;
                WriteImageToAppDirectory(processedBitmapImage, $"sampledSignature{SamplingRate}_{index:00}.png");
                return processedImage;
            }

            await ExecuteFunctionOnImagesAsync(
                binaryImages,
                FunctionAsync);
        }

        private async Task CalculateSignatureAsync(IEnumerable<BinaryImage> binaryImages)
        {
            BinaryImage FunctionAsync(int index, BinaryImage binaryImage)
            {
                var processedImage = ImageProcessor.GetSignatureIn(binaryImage, SamplingRate);
                var processedBitmapImage = processedImage.ToBitmapImage();
                this.SignatureOfNumbers[index] = processedBitmapImage;
                WriteImageToAppDirectory(processedBitmapImage, $"signature{SamplingRate}_{index:00}.png");
                return processedImage;
            }

            await ExecuteFunctionOnImagesAsync(
                binaryImages,
                FunctionAsync);
        }
    }
}