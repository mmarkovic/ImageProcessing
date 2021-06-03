namespace ImageProcessing.NumbersBySignature
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using ImageProcessing.Annotations;
    using ImageProcessing.NumbersBySignature.img;

    using ImageProcessingLib;

    public class NumbersBySignatureViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The applied sampling rate for calculating the signatures.
        /// </summary>
        /// <remarks>
        /// The value should be in the range between 90 and 360.
        /// </remarks>
        private const int SamplingRate = 180;

        /// <summary>
        /// Determines if all processed images should be written on the disk.
        /// </summary>
        /// <remarks>
        /// This is useful, to check every processing steps for debugging reasons and to
        /// verify the applied algorithm. But if the application behaves as expected, it
        /// can produced undesired noise and should therefore be deactivated.
        /// </remarks>
        private const bool WriteCalculatedImagesToDisk = false;

        private string identifiedNumberResult;
        private BitmapImage signatureImage;

        public NumbersBySignatureViewModel()
        {
            this.signatureImage = new BitmapImage();
            this.IdentifyImageCommand = new AsyncRelayCommand(async _ => await this.IdentifyImageAsync());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand IdentifyImageCommand { get; }

        public BitmapImage SignatureImage
        {
            get => this.signatureImage;
            set
            {
                this.signatureImage = value;
                this.OnPropertyChanged();
            }
        }

        public string IdentifiedNumberResult
        {
            get => this.identifiedNumberResult;
            set
            {
                if (value != this.identifiedNumberResult)
                {
                    this.identifiedNumberResult = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        private static void WriteImageToAppDirectory(BinaryImage binaryImage, string imageName)
        {
            if (!WriteCalculatedImagesToDisk)
            {
                return;
            }

            var image = binaryImage.ToBitmapImage();
            string currentDirectory = Environment.CurrentDirectory;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            string filePath = Path.Combine(currentDirectory, imageName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }

        private static BinaryImage ProcessImageToBinaryImage(BitmapImage bitmapImage)
        {
            var processedImage = BinaryImage.FromImage(bitmapImage.ToBitmap());

            WriteImageToAppDirectory(processedImage, "binary.png");

            return processedImage;
        }

        private static BinaryImage CropImage(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.CropAroundFigures(binaryImage);

            WriteImageToAppDirectory(processedImage, "cropped.png");

            return processedImage;
        }

        private static BinaryImage DownSizeImage(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.DownSizeToHalf(binaryImage);

            WriteImageToAppDirectory(processedImage, "downSized.png");

            return processedImage;
        }

        private static BinaryImage ApplySmoothing(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.Smoothing(binaryImage);

            WriteImageToAppDirectory(processedImage, $"smoothed.png");

            return processedImage;
        }

        private static BinaryImage ApplyThinning(BinaryImage binaryImage)
        {
            const int MaxNumberOfIterations = 35;

            BinaryImage img = binaryImage;
            BinaryImage previousImg = binaryImage;

            for (int i = 0; i < MaxNumberOfIterations; i++)
            {
                img = ImageProcessor.Thinning(img);

                if (img.Equals(previousImg))
                {
                    // no changes after last thinning -> abort
                    return img;
                }

                previousImg = img;
            }

            return img;
        }

        private static BinaryImage CalculateSignature(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.GetSignatureIn(binaryImage, SamplingRate);

            WriteImageToAppDirectory(processedImage, $"signature{SamplingRate}.png");

            return processedImage;
        }

        private static BitmapImage LoadImage()
        {
            return ImageResource._1.ToBitmapImage();
        }

        private async Task IdentifyImageAsync()
        {
            var calculateSignatureOfImage = await Task.Run(this.CalculateSignatureOfImage);
            SignatureImage = calculateSignatureOfImage.ToBitmapImage();
            this.IdentifiedNumberResult = new Random().Next(0, 9).ToString();
        }

        private BinaryImage CalculateSignatureOfImage()
        {
            var rawBitmapImage = LoadImage();
            var binaryImage = ProcessImageToBinaryImage(rawBitmapImage);
            var croppedImage = CropImage(binaryImage);
            var downSizedImage = DownSizeImage(croppedImage);
            var smoothedImage = ApplySmoothing(downSizedImage);
            var thinnedImage = ApplyThinning(smoothedImage);

            return CalculateSignature(thinnedImage);
        }
    }
}