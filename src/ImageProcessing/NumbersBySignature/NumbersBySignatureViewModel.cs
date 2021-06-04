namespace ImageProcessing.NumbersBySignature
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using ImageProcessing.NumbersBySignature.img;

    using ImageProcessingLib;

    using SignatureTemplate;

    public class NumbersBySignatureViewModel : INotifyPropertyChanged
    {
        private readonly IAppConfig appConfig;
        private readonly SignatureTemplateViewModel[] signatureTemplateViewModels;

        private string identifiedNumberResult;
        private BitmapImage signatureImage;

        public NumbersBySignatureViewModel() : this(new AppConfig())
        {
        }

        public NumbersBySignatureViewModel(IAppConfig appConfig)
        {
            this.appConfig = appConfig;
            this.signatureImage = new BitmapImage();
            this.signatureTemplateViewModels = new SignatureTemplateViewModel[10];

            for (int i = 0; i < 10; i++)
            {
                var numberLabel = i.ToString();
                object templateImageObject = ImageResource.ResourceManager.GetObject($"signTemplate_{i}")
                    ?? throw new InvalidOperationException($"Signature Template for number {i} not found");
                var signatureTemplateImage = ((Bitmap)templateImageObject).ToBitmapImage();

                this.signatureTemplateViewModels[i] = new SignatureTemplateViewModel(numberLabel, signatureTemplateImage);
            }

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

        public IReadOnlyList<SignatureTemplateViewModel> SignatureTemplateViewModels
            => this.signatureTemplateViewModels;

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

            var image = binaryImage.ToBitmapImage(BinaryImageColorSettings.Default);
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

            this.SignatureImage = calculateSignatureOfImage.ToBitmapImage(
                BinaryImageColorSettings.TransparentBackground);

            var redForeground = new BinaryImageColorSettings(Color.Red, Color.Transparent);
            var signatureImageOnTemplate = calculateSignatureOfImage.ToBitmapImage(
                redForeground);

            foreach (var signatureTemplateViewModel in this.signatureTemplateViewModels)
            {
                signatureTemplateViewModel.CalculatedSignatureImage = signatureImageOnTemplate;
            }

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