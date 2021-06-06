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

        private void NumbersBySignatureView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as NumbersBySignatureViewModel;
            vm?.LoadSignatureTemplates();
        }
    }
}
