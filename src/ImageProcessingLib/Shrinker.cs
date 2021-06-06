namespace ImageProcessingLib
{
    using System.Drawing;
    using System.Threading.Tasks;

    /// <summary>
    /// Shrinks an image to a smaller size.
    /// </summary>
    internal static class Shrinker
    {
        /// <summary>
        /// Shrinks the <paramref name="image"/> by half its size and returns the new image as result.
        /// </summary>
        internal static BinaryImage ShrinkByHalf(BinaryImage image)
        {
            var downSizedImage = new BinaryImage(image.Size.Width / 2, image.Size.Height / 2);
            var matrixSize = new Size(2, 2);

            Parallel.For(
                0,
                downSizedImage.Size.Height,
                downSizedM =>
                {
                    int originalM = downSizedM * 2;
                    for (int downSizedN = 0; downSizedN < downSizedImage.Size.Width; downSizedN++)
                    {
                        int originalN = downSizedN * 2;
                        var positionInImage = new MatrixPosition(originalM, originalN);
                        var neighbourMatrixFromPosition =
                            image.GetNeighborMatrixFromPosition(positionInImage, matrixSize);
                        float averageValue = neighbourMatrixFromPosition.GetAverageValue();
                        downSizedImage[downSizedM, downSizedN] = averageValue >= 0.75f
                            ? BinaryImage.Black
                            : BinaryImage.White;
                    }
                });

            return downSizedImage;
        }
    }
}