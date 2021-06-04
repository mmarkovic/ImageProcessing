namespace ImageProcessing.NumbersBySignature.SignatureTemplate
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class SignatureTemplateViewModel : INotifyPropertyChanged
    {
        private string numberLabel;
        private ImageSource signatureTemplateImage;
        private BitmapImage calculatedSignatureImage;

        public SignatureTemplateViewModel()
            : this(string.Empty, new BitmapImage())
        {
        }

        public SignatureTemplateViewModel(string numberLabel, ImageSource signatureTemplateImage)
        {
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
                if (value == this.numberLabel)
                {
                    return;
                }

                this.numberLabel = value;
                this.OnPropertyChanged();
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
            set
            {
                this.calculatedSignatureImage = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}