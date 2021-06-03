namespace ImageProcessing.NumbersBySignature
{
    using System.Windows.Controls;

    public partial class NumbersBySignatureView : UserControl
    {
        public NumbersBySignatureView()
        {
            this.InitializeComponent();
            this.DataContext = new NumbersBySignatureViewModel();
        }
    }
}
