namespace ImageProcessing.NumbersBySignature.SignatureTemplate
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using ImageProcessingLib;

    using Color = System.Drawing.Color;

    public class SignatureTemplateViewModelAsync : INotifyPropertyChanged
    {
        private BitmapImage calculatedSignatureImage;
        private bool? isMatch;
        private string numberLabel;
        private ImageSource signatureTemplateImage;

        public SignatureTemplateViewModelAsync()
            : this(string.Empty, new BitmapImage())
        {
        }

        public SignatureTemplateViewModelAsync(string numberLabel, ImageSource signatureTemplateImage)
        {
            this.isMatch = null;
            this.numberLabel = numberLabel;
            this.signatureTemplateImage = signatureTemplateImage;
            this.calculatedSignatureImage = new BitmapImage();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string NumberLabel
        {
            get => this.numberLabel;
            set
            {
                if (value != this.numberLabel)
                {
                    this.numberLabel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ImageSource SignatureTemplateImage
        {
            get => this.signatureTemplateImage;
            set
            {
                this.signatureTemplateImage = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage CalculatedSignatureImage
        {
            get => this.calculatedSignatureImage;
            private set
            {
                this.calculatedSignatureImage = value;
                this.OnPropertyChanged();
            }
        }

        public bool? IsMatch
        {
            get => this.isMatch;
            private set
            {
                if (value != this.isMatch)
                {
                    this.isMatch = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public async Task EvaluateSignatureToTemplateAsync(BinaryImage calculatedSignatureToVerify)
        {
            var redForeground = new BinaryImageColorSettings(Color.Red, Color.Transparent);
            var calculatedSignatureToVerifyImage = calculatedSignatureToVerify.ToBitmapImage(redForeground);
            this.CalculatedSignatureImage = calculatedSignatureToVerifyImage;

            var templateImage = this.SignatureTemplateImage.ToBinaryImage();

            this.IsMatch = await Task.Run(
                () => ImageProcessor.VerifyIfSignatureMatchesToTemplate(
                    calculatedSignatureToVerify, templateImage));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}