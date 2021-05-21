namespace ImageProcessing.NumbersBySignature
{
    using System.Windows;
    using System.Windows.Controls;

    public partial class NumbersBySignatureView : UserControl
    {
        public NumbersBySignatureView()
        {
            this.InitializeComponent();
            this.DataContext = new NumbersBySignatureViewModel();
        }

        private async void NumbersBySignatureView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is NumbersBySignatureViewModel viewModel)
            {
                await viewModel.ProcessImagesAsync();
            }
        }
    }
}
