namespace ImageProcessing.NumbersBySignature
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using ImageProcessing.NumbersBySignature.img;

    using ImageProcessingLib;

    public class NumbersBySignatureViewModel : INotifyPropertyChanged
    {
        private readonly IAppConfig appConfig;

        private string identifiedNumberResult;
        private BitmapImage signatureImage;

        public NumbersBySignatureViewModel() : this(new AppConfig())
        {
        }

        public NumbersBySignatureViewModel(IAppConfig appConfig)
        {
            this.appConfig = appConfig;
            this.signatureImage = new BitmapImage();
            this.identifiedNumberResult = "";
            this.IdentifyImageCommand = new AsyncRelayCommand(async _ => await this.IdentifyImageAsync());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand IdentifyImageCommand { get; }

        public BitmapImage SignatureImage
        {
            get => this.signatureImage;
            private set
            {
                this.signatureImage = value;
                this.OnPropertyChanged();
            }
        }

        public string IdentifiedNumberResult
        {
            get => this.identifiedNumberResult;
            private set
            {
                if (value != this.identifiedNumberResult)
                {
                    this.identifiedNumberResult = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private BinaryImage ProcessImageToBinaryImage(BitmapImage bitmapImage)
        {
            var processedImage = BinaryImage.FromImage(bitmapImage.ToBitmap());

            this.WriteImageToAppDirectory(processedImage, "binary.png");

            return processedImage;
        }

        private BinaryImage CropImage(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.CropAroundFigures(binaryImage);

            this.WriteImageToAppDirectory(processedImage, "cropped.png");

            return processedImage;
        }

        private BinaryImage DownSizeImage(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.DownSizeToHalf(binaryImage);

            this.WriteImageToAppDirectory(processedImage, "downSized.png");

            return processedImage;
        }

        private BinaryImage ApplySmoothing(BinaryImage binaryImage)
        {
            var processedImage = ImageProcessor.Smoothing(binaryImage);

            this.WriteImageToAppDirectory(processedImage, "smoothed.png");

            return processedImage;
        }

        private BinaryImage ApplyThinning(BinaryImage binaryImage)
        {
            const int MaxNumberOfIterations = 35;

            BinaryImage img = binaryImage;
            BinaryImage previousImg = binaryImage;

            for (int i = 0; i < MaxNumberOfIterations; i++)
            {
                img = ImageProcessor.Thinning(img);

                this.WriteImageToAppDirectory(img, $"thinned_itr{i:00}.png");

                if (img.Equals(previousImg))
                {
                    // no changes after last thinning -> abort
                    return img;
                }

                previousImg = img;
            }

            return img;
        }

        private BinaryImage CalculateSignature(BinaryImage binaryImage)
        {
            int samplingRate = this.appConfig.SignatureSamplingRate;
            var processedImage = ImageProcessor.GetSignatureIn(binaryImage, samplingRate);

            this.WriteImageToAppDirectory(processedImage, $"signature{samplingRate}.png");

            return processedImage;
        }

        private void WriteImageToAppDirectory(BinaryImage binaryImage, string imageName)
        {
            if (!this.appConfig.WriteProcessedImagesToDisk)
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

        private static BitmapImage LoadImage()
        {
            return ImageResource._1.ToBitmapImage();
        }

        private async Task IdentifyImageAsync()
        {
            BinaryImage calculateSignatureOfImage = await Task.Run(this.CalculateSignatureOfImage);
            this.SignatureImage = calculateSignatureOfImage.ToBitmapImage(BackgroundSettings.Transparent);
            this.IdentifiedNumberResult = new Random().Next(0, 9).ToString();
        }

        private BinaryImage CalculateSignatureOfImage()
        {
            var rawBitmapImage = LoadImage();
            var binaryImage = this.ProcessImageToBinaryImage(rawBitmapImage);
            var croppedImage = this.CropImage(binaryImage);
            var downSizedImage = this.DownSizeImage(croppedImage);
            var smoothedImage = this.ApplySmoothing(downSizedImage);
            var thinnedImage = this.ApplyThinning(smoothedImage);

            return this.CalculateSignature(thinnedImage);
        }
    }
}