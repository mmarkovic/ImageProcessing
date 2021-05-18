namespace ImageProcessingLib
{
    using System.Collections.Generic;
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

                                matrixColors[matrixM, matrixN] = img[imageM, imageN] > 0
                                    ? BinaryImage.Black
                                    : BinaryImage.White;
                            }
                        }

                        float average = AverageValueInBinaryMatrix(matrixColors);

                        img[m, n] = average >= 0.5f ? BinaryImage.Black : BinaryImage.White;
                    }
                });

            return img;
        }

        /// <remarks>
        /// source: https://homepages.inf.ed.ac.uk/rbf/HIPR2/thin.htm
        /// </remarks>
        public static BinaryImage Thinning(BinaryImage binaryImage)
        {
            // 0 stands for white
            // 1 stands for black
            // -1 will be ignored
            var structuringElementB1 = new StructuringElement(new[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } });
            var structuringElementB2 = new StructuringElement(new[,] { { -1, 0, 0 }, { 1, 1, 0 }, { 1, 1, -1 } });
            var structuringElementB3 = new StructuringElement(new[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } });
            var structuringElementB4 = new StructuringElement(new[,] { { 1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } });
            var structuringElementB5 = new StructuringElement(new[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } });
            var structuringElementB6 = new StructuringElement(new[,] { { -1, 1, 1 }, { 0, 1, 1 }, { 0, 0, -1 } });
            var structuringElementB7 = new StructuringElement(new[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } });
            var structuringElementB8 = new StructuringElement(new[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, 1 } });

            var structuringElements = new[]
            {
                structuringElementB1,
                structuringElementB2,
                structuringElementB3,
                structuringElementB4,
                structuringElementB5,
                structuringElementB6,
                structuringElementB7,
                structuringElementB8
            };

            return Thinning(
                binaryImage,
                structuringElements);
        }

        public static BinaryImage Thinning(BinaryImage binaryImage, IEnumerable<StructuringElement> structuringElements)
        {
            var thinnedImage = binaryImage;

            foreach (var structuringElement in structuringElements)
            {
                var hitAndMissImg = thinnedImage.Clone();

                Parallel.For(
                    0,
                    hitAndMissImg.Size.Height,
                    m =>
                    {
                        for (var n = 0; n < hitAndMissImg.Size.Width; n++)
                        {
                            var positionInImage = new MatrixPosition(m, n);
                            var neighbourMatrix = binaryImage.GetNeighbourMatrixFromPosition(
                                positionInImage,
                                structuringElement.Size);

                            bool match = MatchMatrixToStructuringElement(neighbourMatrix, structuringElement);
                            if (match)
                            {
                                hitAndMissImg[m, n] = BinaryImage.White;
                            }
                        }
                    });

                thinnedImage = hitAndMissImg;
            }

            return thinnedImage;
        }

        public static bool MatchMatrixToStructuringElement(BinaryMatrix matrix, StructuringElement structuringElement)
        {
            for (var m = 0; m < matrix.Size.Height; m++)
            {
                for (var n = 0; n < matrix.Size.Width; n++)
                {
                    if (structuringElement[m, n] == -1)
                    {
                        continue;
                    }

                    if (structuringElement[m, n] != matrix[m, n])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static BinaryImage CropAroundFigures(BinaryImage binaryImage)
        {
            return Cropper.CropAroundFigures(binaryImage);
        }

        /// <summary>
        /// Calculates the average value of the center of the <paramref name="matrixValues"/>.
        /// </summary>
        /// <example> 
        /// <![CDATA[
        /// 1 1 1 1 0
        /// 1 1 1 1 0
        /// 1 1 1 0 0
        /// 1 1 0 0 0
        /// 1 0 0 0 0
        ///
        /// number of values = 5 * 5 = 25
        /// sum = 4+4+3+2+1= 14
        /// average = 14 / 25 = 0.56
        /// ]]>
        /// </example>
        private static float AverageValueInBinaryMatrix(BinaryMatrix matrixValues)
        {
            var sum = 0;

            for (var m = 0; m < matrixValues.Size.Height; m++)
            {
                for (var n = 0; n < matrixValues.Size.Width; n++)
                {
                    sum += matrixValues[m, n];
                }
            }

            int numberOfValues = matrixValues.Size.Height * matrixValues.Size.Width;

            return sum / (float)numberOfValues;
        }
    }
}