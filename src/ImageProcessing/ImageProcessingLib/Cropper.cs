namespace ImageProcessingLib
{
    /// <summary>
    /// Identifies the pixels within a <see cref="BinaryImage"/> which represents the edge of a
    /// figure and crops the image around those pixels with a small offset.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// Cropping offset: 2
    ///
    /// image before:                       image after:
    /// 0000000000000                       0000000
    /// 0000000000000                       0000000
    /// 00000▓▓▓00000                       00▓▓▓00
    /// 00000▓▓000000    -- cropping -->    00▓▓000
    /// 00000▓0000000                       00▓0000
    /// 0000000000000                       0000000
    /// 0000000000000                       0000000
    /// ]]>
    /// </example>
    internal static class Cropper
    {
        /// <summary>
        /// Number of pixels that will be kept around black pixels
        /// </summary>
        private const int CroppingOffset = 2;

        public static BinaryImage CropAroundFigures(BinaryImage binaryImage)
        {
            int topPixelRowMWithOffset = FindTopMostPixelIn(binaryImage) - CroppingOffset;
            if (topPixelRowMWithOffset == -1)
            {
                return binaryImage;
            }

            int bottomPixelRowMWithOffset = FindBottomLinePixelIn(binaryImage) + CroppingOffset;
            int leftPixelColumnNWithOffset = FindLeftMostPixelIn(binaryImage) - CroppingOffset;
            int rightPixelColumnNWithOffset = FindRightMostPixelIn(binaryImage) + CroppingOffset;

            int newTopM = topPixelRowMWithOffset > 0 ? topPixelRowMWithOffset : 0;
            int newBottomM = bottomPixelRowMWithOffset < binaryImage.Size.Height - 1
                ? bottomPixelRowMWithOffset
                : binaryImage.Size.Height - 1;
            int newLeftN = leftPixelColumnNWithOffset > 0 ? leftPixelColumnNWithOffset : 0;
            int newRightN = rightPixelColumnNWithOffset < binaryImage.Size.Width - 1
                ? rightPixelColumnNWithOffset
                : binaryImage.Size.Width - 1;

            int newHeight = newBottomM - newTopM + 1;
            int newWidth = newRightN - newLeftN + 1;

            var croppedImage = new BinaryImage(newWidth, newHeight);

            for (int m = 0; m < newHeight; m++)
            {
                int mPositionInOriginal = topPixelRowMWithOffset + m;

                for (int n = 0; n < newWidth; n++)
                {
                    int nPositionInOriginal = leftPixelColumnNWithOffset + n;

                    croppedImage[m, n] = binaryImage[mPositionInOriginal, nPositionInOriginal];
                }
            }

            return croppedImage;
        }

        private static int FindTopMostPixelIn(BinaryImage binaryImage)
        {
            for (int m = 0; m < binaryImage.Size.Height; m++)
            {
                for (int n = 0; n < binaryImage.Size.Width; n++)
                {
                    if (binaryImage[m, n] == BinaryImage.Black)
                    {
                        return m;
                    }
                }
            }

            return -1;
        }

        private static int FindBottomLinePixelIn(BinaryImage binaryImage)
        {
            for (int m = binaryImage.Size.Height - 1; m >= 0; m--)
            {
                for (int n = 0; n < binaryImage.Size.Width; n++)
                {
                    if (binaryImage[m, n] == BinaryImage.Black)
                    {
                        return m;
                    }
                }
            }

            return -1;
        }

        private static int FindLeftMostPixelIn(BinaryImage binaryImage)
        {
            for (int n = 0; n < binaryImage.Size.Width; n++)
            {
                for (int m = 0; m < binaryImage.Size.Height; m++)
                {
                    if (binaryImage[m, n] == BinaryImage.Black)
                    {
                        return n;
                    }
                }
            }

            return -1;
        }

        private static int FindRightMostPixelIn(BinaryImage binaryImage)
        {
            for (int n = binaryImage.Size.Width - 1; n >= 0; n--)
            {
                for (int m = 0; m < binaryImage.Size.Height; m++)
                {
                    if (binaryImage[m, n] == BinaryImage.Black)
                    {
                        return n;
                    }
                }
            }

            return -1;
        }
    }
}