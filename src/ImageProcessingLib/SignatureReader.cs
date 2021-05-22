namespace ImageProcessingLib
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Get's the signature of an object in an image.
    /// </summary>
    /// <remarks>
    /// The signature of an object is calculated by sampling its edge in clockwise order. The sampling will
    /// be performed by using predefined sampling steps, which determines the angle from the center of the
    /// object.
    /// <example>
    /// Here is an example of a sampling using 8 sampling points.
    /// <![CDATA[
    /// ^
    /// |   s1     s2      s3          c = center of the object
    /// |                              si = sampling pont i (where i is the zero based index of s)
    /// |   s0      c      s4
    /// |
    /// |   s7     s6      s5
    /// . -------------------->
    ///
    /// Result of a rectangle:
    /// sampling point  | angle | radius
    /// --------------- | ----- | ------
    ///  s0             |    0  |  6.0
    ///  s1             |   45  |  7.5
    ///  s2             |   90  |  3.0
    ///  s3             |  135  |  7.5
    ///  s4             |  180  |  6.0
    ///  s5             |  225  |  7.5
    ///  s6             |  270  |  3.0
    ///  s7             |  315  |  7.5
    ///
    /// From these sampling points the following signature can be rendered:
    ///     ^
    /// 7.5 |     X     X     X     X
    ///     |   /  \   / \   / \   / \
    /// 6.0 |  X           X
    ///     |       \ /         \ /
    ///     |
    /// 3.0 |        X           X
    ///     |
    ///     |
    ///     .---------------------------> sampling point
    ///        0  1  2  3  4  5  6  7
    /// ]]>
    /// </example>
    /// </remarks>
    internal static class SignatureReader
    {
        /// <summary>
        /// Gets the signature of an object in <paramref name="image"/> by using 16 sampling points.
        /// </summary>
        /// <returns>
        /// The coordinates of the signature points in an ordered list.
        /// The key of the dictionary represents the index of the sampling point.
        /// The value represents the radius from the center of all pixels found.
        /// </returns>
        internal static IDictionary<int, IReadOnlyList<int>> GetSignature(BinaryImage image, int numberOfSamplingPoints)
        {
            CheckNumberOfSamplingPointsValueInRange(numberOfSamplingPoints);

            int angleIncrement = 360 / numberOfSamplingPoints;
            var points = new Dictionary<int, IReadOnlyList<int>>();

            var center = GetCenterOfObjectIn(image);

            for (int samplingPoint = 0; samplingPoint < 16; samplingPoint++)
            {
                // value of the angle, at which the sampling from the center will be taken.
                int angle = samplingPoint * angleIncrement;

                List<int> pointsInAngle = FindAllPointFromCenterInAngle(image, center, angle);

                points.Add(samplingPoint, pointsInAngle);
            }

            return points;
        }

        /// <summary>
        /// Gets the center of the object in <paramref name="image"/>.
        /// </summary>
        internal static MatrixPosition GetCenterOfObjectIn(BinaryImage image)
        {
            int leftMostPoint = image.FindLeftMostPixelIn();
            int rightMostPoint = image.FindRightMostPixelIn();
            int centerNPoint = (leftMostPoint + rightMostPoint) / 2;

            int topMostPoint = image.FindTopMostPixelPosition();
            int bottomLinePoint = image.FindBottomLinePixelIn();
            int centerMPoint = (topMostPoint + bottomLinePoint) / 2;

            return new MatrixPosition(centerMPoint, centerNPoint);
        }

        /// <summary>
        /// Draws a line from the <paramref name="center"/> to the edge of the <paramref name="image"/> using the
        /// <paramref name="angle"/> and returns the distance to the center (the radius) of all Points found on
        /// this line.
        /// </summary>
        /// <remarks>
        /// <example>
        /// Here's an example using the angle of 45 degrees.
        /// <![CDATA[
        /// ^
        /// |   E                         E = edge of the image
        /// |    \                        X = point found in image on the line
        /// |      \                      c = center
        /// |        X
        /// |          \
        /// |            \
        /// |              c
        /// |
        /// .
        /// .
        /// . --------------------------->
        /// ]]>
        /// </example>
        /// </remarks>
        private static List<int> FindAllPointFromCenterInAngle(BinaryImage image, MatrixPosition center, int angle)
        {
            var radiusOfPointsFound = new List<int>();
            MatrixPosition nextPoint = center;

            do
            {
                // TODO
                nextPoint = new MatrixPosition(nextPoint.M + 1, nextPoint.N + 1);
            } while (IsPositionWithinImage(image, nextPoint));

            return radiusOfPointsFound;
        }

        private static bool IsPositionWithinImage(BinaryImage image, MatrixPosition position)
        {
            return position.M >= 0
                   && position.N >= 0
                   && position.M < image.Size.Height
                   && position.N < image.Size.Width;
        }

        private static void CheckNumberOfSamplingPointsValueInRange(int numberOfSamplingPoints)
        {
            if (numberOfSamplingPoints is < 4 or > 360)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfSamplingPoints),
                    "The number of sampling points must be between 4 and 360.");
            }
        }
    }
}