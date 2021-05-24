namespace ImageProcessingLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        /// Gets the signature of an object in <paramref name="image"/> by using a variable number of sampling lines.
        /// </summary>
        /// <returns>
        /// The coordinates of the signature points in an ordered list.
        /// The key of the dictionary represents the index of the sampling point.
        /// The value represents the radius from the center of all pixels found.
        /// </returns>
        internal static ShapeSignature GetSignature(BinaryImage image, int numberOfSamplingLines)
        {
            CheckNumberOfSamplingLinesIsInRange(numberOfSamplingLines);

            var signature = new ShapeSignature(numberOfSamplingLines);
            int samplingIndex = 0;

            var samplingLines = GetSamplingLines(image, numberOfSamplingLines);
            foreach (var samplingLine in samplingLines)
            {
                int[] pointsInAngle = FindAllBlackPixelsOnSamplingLine(image, samplingLine).ToArray();
                signature.Add(samplingIndex, pointsInAngle);
                samplingIndex++;
            }

            return signature;
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

        internal static IReadOnlyCollection<SamplingLine> GetSamplingLines(BinaryImage image, int numberOfSamplingLines)
        {
            CheckNumberOfSamplingLinesIsInRange(numberOfSamplingLines);

            double angleIncrement = 360d / numberOfSamplingLines;
            var samplingLines = new SamplingLine[numberOfSamplingLines];

            var center = GetCenterOfObjectIn(image);

            for (int samplingIndex = 0; samplingIndex < numberOfSamplingLines; samplingIndex++)
            {
                // value of the angle, at which the sampling from the center will be taken.
                int angle = (int)Math.Round(samplingIndex * angleIncrement, 0, MidpointRounding.ToZero);
                var samplingLine = GetSamplingLineForAngle(image, center, angle);

                samplingLines[samplingIndex] = samplingLine;
            }

            return samplingLines;
        }

        /// <summary>
        /// Returns the coordinates of the pixels that lie on the sampling line, which is defined by the
        /// <paramref name="startingPosition"/> and its <paramref name="angle"/> to the edge of the
        /// <paramref name="image"/>. The <paramref name="startingPosition"/> will not be returned in the
        /// result set.
        /// </summary>
        /// <remarks>
        /// <example>
        /// Here's an example using the angle of 45 degrees.
        /// <![CDATA[
        /// ^
        /// |   E                         E = edge of the image
        /// |    \                        \ = sampling line
        /// |      \                      c = center
        /// |        \
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
        internal static SamplingLine GetSamplingLineForAngle(
            BinaryImage image,
            MatrixPosition startingPosition,
            int angle)
        {
            const int MaxIterationLimit = 50000;

            var samplingLine = new SamplingLine();

            MatrixPosition nextPosition;
            double phi = (angle + 180d) * Math.PI / 180d;
            int r = 1;

            do
            {
                var deltaN = (int)Math.Round(r * Math.Cos(phi), 0, MidpointRounding.ToZero);
                var deltaM = (int)Math.Round(r * Math.Sin(phi), 0, MidpointRounding.ToZero);
                nextPosition = new MatrixPosition(startingPosition.M + deltaM, startingPosition.N + deltaN);
                if (nextPosition != startingPosition
                    // ReSharper disable once SimplifyLinqExpressionUseAll
                    && !samplingLine.ContainsPosition(nextPosition)
                    && IsPositionWithinImage(image, nextPosition))
                {
                    var pointOfInterest = new SamplingPoint(nextPosition, r);
                    samplingLine.Add(pointOfInterest);
                }

                r++;
            } while (IsPositionWithinImage(image, nextPosition) && r < MaxIterationLimit);

            return samplingLine;
        }

        private static IEnumerable<int> FindAllBlackPixelsOnSamplingLine(
            BinaryImage image,
            SamplingLine samplingLine)
        {
            return from samplingPoints in samplingLine.SamplingPoints
                where image[samplingPoints.Position] == BinaryImage.Black
                select samplingPoints.Radius;
        }

        private static bool IsPositionWithinImage(BinaryImage image, MatrixPosition position)
        {
            return position.M >= 0
                   && position.N >= 0
                   && position.M < image.Size.Height
                   && position.N < image.Size.Width;
        }

        private static void CheckNumberOfSamplingLinesIsInRange(int numberOfSamplingLines)
        {
            if (numberOfSamplingLines is < 4 or > 360)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfSamplingLines),
                    "The number of sampling points must be between 4 and 360.");
            }
        }
    }
}