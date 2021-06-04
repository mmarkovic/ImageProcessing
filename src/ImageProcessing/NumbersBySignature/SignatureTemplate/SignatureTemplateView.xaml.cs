namespace ImageProcessing.NumbersBySignature.SignatureTemplate
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class SignatureTemplateView : UserControl
    {
        public static readonly DependencyProperty NumberLabelProperty =
            DependencyProperty.Register(
                "NumberLabel",
                typeof(string),
                typeof(SignatureTemplateView),
                new PropertyMetadata("", null));

        public static readonly DependencyProperty SignatureTemplateImageProperty =
            DependencyProperty.Register(
                "SignatureTemplateImage",
                typeof(ImageSource),
                typeof(SignatureTemplateView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CalculatedSignatureImageProperty =
            DependencyProperty.Register(
                "CalculatedSignatureImage",
                typeof(BitmapImage),
                typeof(SignatureTemplateView),
                new PropertyMetadata(null));

        public SignatureTemplateView()
        {
            this.InitializeComponent();
            //this.DataContext = new SignatureTemplateViewModel();
        }

        //public SignatureTemplateViewModel ViewModel => (SignatureTemplateViewModel)this.DataContext;

        public string NumberLabel
        {
            //get => this.ViewModel.NumberLabel;
            //set => this.ViewModel.NumberLabel = value;
            get => (string)this.GetValue(NumberLabelProperty);
            set => this.SetValue(NumberLabelProperty, value);
        }

        public ImageSource SignatureTemplateImage
        {
            //get => this.ViewModel.SignatureTemplateImage;
            //set => this.ViewModel.SignatureTemplateImage = value;
            get => (ImageSource)this.GetValue(SignatureTemplateImageProperty);
            set => this.SetValue(SignatureTemplateImageProperty, value);
        }

        public BitmapImage CalculatedSignatureImage
        {
            //get => this.ViewModel.CalculatedSignatureImage;
            //set => this.ViewModel.CalculatedSignatureImage = value;
            get => (BitmapImage)this.GetValue(CalculatedSignatureImageProperty);
            set
            {
                Console.WriteLine("Setting calculated image for " + this.NumberLabel);
                this.SetValue(CalculatedSignatureImageProperty, value);
            }
        }
    }
}