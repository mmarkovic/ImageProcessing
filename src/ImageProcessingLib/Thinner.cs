namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class Thinner
    {
        // 0 stands for white
        // 1 stands for black
        // -1 will be ignored
        internal static readonly StructuringElement B1 = new(new[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } });
        internal static readonly StructuringElement B2 = new(new[,] { { -1, 0, 0 }, { 1, 1, 0 }, { 1, 1, -1 } });
        internal static readonly StructuringElement B3 = new(new[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } });
        internal static readonly StructuringElement B4 = new(new[,] { { 1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } });
        internal static readonly StructuringElement B5 = new(new[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } });
        internal static readonly StructuringElement B6 = new(new[,] { { -1, 1, 1 }, { 0, 1, 1 }, { 0, 0, -1 } });
        internal static readonly StructuringElement B7 = new(new[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } });
        internal static readonly StructuringElement B8 = new(new[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, 1 } });

        /// <remarks>
        /// source: https://homepages.inf.ed.ac.uk/rbf/HIPR2/thin.htm
        /// </remarks>
        internal static BinaryImage Thinning(BinaryImage binaryImage)
        {
            var structuringElements = new[] { B1, B2, B3, B4, B5, B6, B7, B8 };

            return Thinning(
                binaryImage,
                structuringElements);
        }

        internal static BinaryImage Thinning(BinaryImage binaryImage, int numberOfIterations)
        {
            var thinnedImage = binaryImage;

            for (int i = 0; i < numberOfIterations; i++)
            {
                thinnedImage = Thinning(thinnedImage.Clone());
            }

            return thinnedImage;
        }

        internal static BinaryImage Thinning(
            BinaryImage binaryImage,
            IEnumerable<StructuringElement> structuringElements)
        {
            var thinnedImage = binaryImage;

            var strEl = structuringElements.ToArray();

            foreach (var structuringElement in strEl)
            {
                var hitAndMissImg = thinnedImage.Clone();
                var image = thinnedImage;

                Parallel.For(
                    0,
                    hitAndMissImg.Size.Height,
                    m =>
                    {
                        for (var n = 0; n < hitAndMissImg.Size.Width; n++)
                        {
                            var positionInImage = new MatrixPosition(m, n);
                            var neighbourMatrix = image.GetNeighborMatrixFromPosition(
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

        internal static bool MatchMatrixToStructuringElement(BinaryMatrix matrix, StructuringElement structuringElement)
        {
            for (var m = 0; m < matrix.Size.Height; m++)
            {
                for (var n = 0; n < matrix.Size.Width; n++)
                {
                    if (structuringElement[m, n] == -1)
                    {
                        continue;
                    }

                    byte matrixValue = matrix[m, n] == BinaryMatrix.Black
                        ? BinaryMatrix.BlackValue
                        : BinaryMatrix.WhiteValue;

                    if (structuringElement[m, n] != matrixValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}