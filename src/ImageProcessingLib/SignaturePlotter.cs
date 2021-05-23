namespace ImageProcessingLib
{
    using System.Linq;

    /// <summary>
    /// Plots an image based on the data of an object signature.
    /// </summary>
    internal static class SignaturePlotter
    {
        internal static BinaryImage Plot(ShapeSignature signature)
        {
            int[][] signaturePoints = signature.Get();
            int largestRadius = FindLargestRadiusIn(signature);

            var image = new BinaryImage(signature.NumberOfSamplingPoints, largestRadius + 3);

            for (int i = 0; i < signature.NumberOfSamplingPoints; i++)
            {
                int[] pointInAngle = signaturePoints[i];
                for (int j = 0; j < pointInAngle.Length; j++)
                {
                    int radius = pointInAngle[j];
                    image[radius, i] = BinaryImage.Black;
                }
            }

            return image;
        }

        private static int FindLargestRadiusIn(ShapeSignature signature)
        {
            int[][] signaturePoints = signature.Get();
            int largestRadius = 0;

            for (int i = 0; i < signature.NumberOfSamplingPoints; i++)
            {
                int[] signaturePointsAtPosition = signaturePoints[i];
                int largestRadiusAtIndex = signaturePointsAtPosition.Any() ? signaturePointsAtPosition.Max() : 0;
                if (largestRadiusAtIndex > largestRadius)
                {
                    largestRadius = largestRadiusAtIndex;
                }
            }

            return largestRadius;
        }
    }
}