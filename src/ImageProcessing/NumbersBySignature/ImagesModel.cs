namespace ImageProcessing.NumbersBySignature
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Imaging;

    using Annotations;

    /// <summary>
    /// Contains all images of the numbers (0-9) for processing.
    /// </summary>
    public class ImagesModel : INotifyPropertyChanged
    {
        private readonly BitmapImage[] numberImages;
        private readonly int[] processingIteration;

        private bool displayIteration;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImagesModel()
        {
            this.numberImages = new BitmapImage[10];
            this.processingIteration = new int[10];
        }

        public BitmapImage this[int index]
        {
            get => this.numberImages[index];
            set
            {
                this.numberImages[index] = value;
                this.OnPropertyChanged($"Number{index}Image");
            }
        }

        public bool DisplayIteration
        {
            get => this.displayIteration;
            set
            {
                this.displayIteration = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number0Image
        {
            get => this.numberImages[0];
            set
            {
                this.numberImages[0] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number1Image
        {
            get => this.numberImages[1];
            set
            {
                this.numberImages[1] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number2Image
        {
            get => this.numberImages[2];
            set
            {
                this.numberImages[2] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number3Image
        {
            get => this.numberImages[3];
            set
            {
                this.numberImages[3] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number4Image
        {
            get => this.numberImages[4];
            set
            {
                this.numberImages[4] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number5Image
        {
            get => this.numberImages[5];
            set
            {
                this.numberImages[5] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number6Image
        {
            get => this.numberImages[6];
            set
            {
                this.numberImages[6] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number7Image
        {
            get => this.numberImages[7];
            set
            {
                this.numberImages[7] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number8Image
        {
            get => this.numberImages[8];
            set
            {
                this.numberImages[8] = value;
                this.OnPropertyChanged();
            }
        }

        public BitmapImage Number9Image
        {
            get => this.numberImages[9];
            set
            {
                this.numberImages[9] = value;
                this.OnPropertyChanged();
            }
        }

        public int Number0ProcessingIteration => this.processingIteration[0];
        public int Number1ProcessingIteration => this.processingIteration[1];
        public int Number2ProcessingIteration => this.processingIteration[2];
        public int Number3ProcessingIteration => this.processingIteration[3];
        public int Number4ProcessingIteration => this.processingIteration[4];
        public int Number5ProcessingIteration => this.processingIteration[5];
        public int Number6ProcessingIteration => this.processingIteration[6];
        public int Number7ProcessingIteration => this.processingIteration[7];
        public int Number8ProcessingIteration => this.processingIteration[8];
        public int Number9ProcessingIteration => this.processingIteration[9];

        public IEnumerable<BitmapImage> GetImages()
        {
            return this.numberImages;
        }

        public void SetProcessingIteration(int imageIndex, int iteration)
        {
            this.processingIteration[imageIndex] = iteration;
            this.OnPropertyChanged($"Number{imageIndex}ProcessingIteration");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}