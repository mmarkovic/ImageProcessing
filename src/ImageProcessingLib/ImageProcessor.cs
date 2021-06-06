namespace ImageProcessingLib
{
    using System.Drawing;
    using System.Threading.Tasks;

    public static class ImageProcessor
    {
        /// <summary>
        /// Applies a smooth filter on <paramref name="binaryImage"/> to remove noise and create
        /// more distinguishable lines.
        /// </summary>
        public static BinaryImage Smoothing(BinaryImage binaryImage)
        {
            // Defines the size of the matrix used to remove the noises in a black and white image.
            // NOTE: the size should be at least 3 and an odd number
            const int MatrixSize = 7;

            // Needed for edge cases of the image border.
            const int MatrixRadius = MatrixSize / 2;

            var img = binaryImage.Clone();

            Parallel.For(
                MatrixRadius,
                img.Size.Height - MatrixRadius,
                m =>
                {
                    for (int n = MatrixRadius; n < img.Size.Width - MatrixRadius; n++)
                    {
                        //  1 1 0
                        //  1 x 0    -> x is the average of all values around.
                        //  0 0 0
                        var matrixColors = new BinaryMatrix(new Size(MatrixSize, MatrixSize));
                        for (var matrixM = 0; matrixM < MatrixSize; matrixM++)
                        {
                            for (var matrixN = 0; matrixN < MatrixSize; matrixN++)
                            {
                                int imageM = m - MatrixRadius + matrixM;
                                int imageN = n - MatrixRadius + matrixN;

                                matrixColors[matrixM, matrixN] = img[imageM, imageN];
                            }
                        }

                        float average = matrixColors.GetAverageValue();

                        img[m, n] = average >= 0.5f ? BinaryImage.Black : BinaryImage.White;
                    }
                });

            return img;
        }

        public static BinaryImage CropAroundFigures(BinaryImage binaryImage)
        {
            return Cropper.CropAroundFigures(binaryImage);
        }

        public static BinaryImage DownSizeToHalf(BinaryImage binaryImage)
        {
            return Shrinker.ShrinkByHalf(binaryImage);
        }

        public static BinaryImage Thinning(BinaryImage binaryImage)
        {
            return Thinner.Thinning(binaryImage);
        }

        public static BinaryImage GetSignatureIn(BinaryImage binaryImage, int samplingRate = 180)
        {
            var signature = SignatureReader.GetSignature(binaryImage, samplingRate);
            return SignaturePlotter.Plot(signature);
        }

        public static bool VerifyIfSignatureMatchesToTemplate(BinaryImage signatureImage, BinaryImage templateImage)
        {
            return SignatureMatcher.IsMatch(signatureImage, templateImage);
        }
    }
}