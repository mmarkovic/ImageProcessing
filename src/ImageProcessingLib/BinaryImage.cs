namespace ImageProcessingLib
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a image in its binary form.
    /// The value 1/true represents the color 'black' and the value 0/false the color 'white'.
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    ///  Sample image content:
    ///   . - - - - - - - -> x
    ///   | 0 0 0 0 0 0 0
    ///   | 0 0 1 1 1 0 0
    ///   | 0 1 1 1 1 1 0
    ///   | 0 0 0 0 0 0 0
    /// y v
    ///
    /// Note: The coordinates start from the top left corner of the image.
    ///       The x achsis represents the whidth and
    ///       the y achsis represents the height.
    /// ]]>
    /// </remarks>
    public sealed class BinaryImage : IEquatable<BinaryImage>
    {
        public const bool White = false;
        public const bool Black = true;

        private readonly bool[,] image;

        public Size Size { get; }

        public bool this[int m, int n]
        {
            get => this.image[m, n];
            set => this.image[m, n] = value;
        }

        public bool this[MatrixPosition position]
        {
            get => this.image[position.M, position.N];
            set => this.image[position.M, position.N] = value;
        }

        public BinaryImage(int width, int height)
        {
            this.image = new bool[height, width];
            this.Size = new Size(width, height);
        }

        private BinaryImage(bool[,] image)
        {
            this.image = image;
            this.Size = new Size(image.GetLength(1), image.GetLength(0));
        }

        public static bool operator ==(BinaryImage left, BinaryImage right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BinaryImage left, BinaryImage right)
        {
            return !Equals(left, right);
        }

        public static BinaryImage FromByteArray(byte[,] image)
        {
            var binaryMatrix = new BinaryMatrix(image);
            return new BinaryImage(binaryMatrix.To2DBoolArray());
        }

        public static BinaryImage FromImage(Bitmap bmp)
        {
            return bmp.PixelFormat == PixelFormat.Format8bppIndexed
                ? FromIndexedImage(bmp)
                : FromNonIndexedImage(bmp);
        }

        private static BinaryImage FromNonIndexedImage(Bitmap bmp)
        {
            const int BitsPerByte = 8;

            var image = new bool[bmp.Height, bmp.Width];
            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(rectangle, ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / BitsPerByte;

            int heightInPixels = bitmapData.Height;
            int widthInPixels = bitmapData.Width;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(
                    0,
                    heightInPixels,
                    m =>
                    {
                        byte* ptrCurrentLine = ptrFirstPixel + (m * bitmapData.Stride);

                        for (var n = 0; n < widthInPixels; n++)
                        {
                            var byteX = n * bytesPerPixel;

                            var color = Color.FromArgb(
                                ptrCurrentLine[byteX + 2],
                                ptrCurrentLine[byteX + 1],
                                ptrCurrentLine[byteX]);

                            image[m, n] = ThresholdFunction(color);
                        }
                    });
            }

            bmp.UnlockBits(bitmapData);

            return new BinaryImage(image);
        }

        /// <remarks>
        /// Indexed images use only a reference value for representing the value of a color. The referenced value
        /// is just an index (a pointer) to the actual color value, which can be retrieved from the image color
        /// palette (<see cref="Image.Palette"/>).
        /// </remarks>
        private static BinaryImage FromIndexedImage(Bitmap indexedImage)
        {
            var image = new bool[indexedImage.Height, indexedImage.Width];

            var rectangle = new Rectangle(0, 0, indexedImage.Width, indexedImage.Height);
            var bitmapData = indexedImage.LockBits(rectangle, ImageLockMode.ReadOnly, indexedImage.PixelFormat);
            int stride = bitmapData.Stride < 0 ? -bitmapData.Stride : bitmapData.Stride;

            int heightInPixels = bitmapData.Height;
            int widthInPixels = bitmapData.Width;
            var colorPalette = indexedImage.Palette.Entries;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(
                    0,
                    heightInPixels,
                    m =>
                    {
                        byte* ptrCurrentLine = ptrFirstPixel + (m * stride);

                        for (var n = 0; n < widthInPixels; n++)
                        {
                            byte colorReferenceIndex = ptrCurrentLine[n];
                            Color color = colorPalette[colorReferenceIndex];
                            image[m, n] = ThresholdFunction(color);
                        }
                    });
            }

            indexedImage.UnlockBits(bitmapData);

            return new BinaryImage(image);
        }

        private static bool ThresholdFunction(Color colorValue)
        {
            // Defines the threshold which determines if a pixel should be painted in white or black.
            const int ThresholdValue = 50;

            return colorValue.R < ThresholdValue
                   || colorValue.G < ThresholdValue
                   || colorValue.B < ThresholdValue
                ? Black
                : White;
        }

        public BinaryImage Clone()
        {
            return new((bool[,])this.image.Clone());
        }

        public Bitmap ToBitmap()
        {
            return this.ToBitmap(BinaryImageColorSettings.Default);
        }

        public Bitmap ToBitmap(BinaryImageColorSettings colorSettings)
        {
            var pixelFormat = colorSettings.UsesTransparent
                ? PixelFormat.Format32bppArgb
                : PixelFormat.Format32bppRgb;

            var bmp = new Bitmap(this.Size.Width, this.Size.Height, pixelFormat);
            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(rectangle, ImageLockMode.WriteOnly, pixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(pixelFormat) / 8;

            int heightInPixels = bitmapData.Height;
            int widthInPixels = bitmapData.Width;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(
                    0,
                    heightInPixels,
                    m =>
                    {
                        byte* ptrCurrentLine = ptrFirstPixel + (m * bitmapData.Stride);
                        for (var n = 0; n < widthInPixels; n++)
                        {
                            int byteX = n * bytesPerPixel;
                            bool isForeground = this.image[m, n] == Black;
                            var color = isForeground ? colorSettings.Foreground : colorSettings.Background;

                            ptrCurrentLine[byteX] = color.B; // blue
                            ptrCurrentLine[byteX + 1] = color.G; // green
                            ptrCurrentLine[byteX + 2] = color.R; // red

                            if (colorSettings.UsesTransparent)
                            {
                                ptrCurrentLine[byteX + 3] = color.A; // alpha
                            }
                        }
                    });
            }

            bmp.UnlockBits(bitmapData);

            return bmp;
        }

        /// <summary>
        /// Gets a value indicating if the binary images is empty, meaning if there is no Black pixel present.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the image has no Black pixels; otherwise <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            return this.FindLeftMostPixelIn() == -1;
        }

        public BinaryMatrix GetNeighborMatrixFromPosition(int m, int n, int matrixSize)
        {
            return this.GetNeighborMatrixFromPosition(
                new MatrixPosition(m, n),
                new Size(matrixSize, matrixSize));
        }

        /// <summary>
        /// Gets the neighbour matrix from the position <paramref name="positionInImage"/> from the image.
        /// The size of the neighbour matrix is defined by <paramref name="matrixSize"/>.
        /// The <paramref name="positionInImage"/> becomes the center of the neighbour matrix.
        /// If the neighbour matrix is created at the edge of the image, the pixels exceeding the image border will
        /// be assumed as 'white'.
        /// </summary>
        /// <remarks>
        /// <![CDATA[
        /// Neighbour matrix example:
        /// Size = 3x3
        /// X = center of the neighbour matrix and the Position within the image.
        ///   . - - - -> x, n
        ///   | 0 0 0
        ///   | 0 X 0    -> X = (m,n) coordinate
        ///   | 0 0 0
        ///   v
        ///  y, m
        /// ]]>
        /// </remarks>
        public BinaryMatrix GetNeighborMatrixFromPosition(MatrixPosition positionInImage, Size matrixSize)
        {
            var neighbourMatrix = new BinaryMatrix(matrixSize);
            int fromLeftEdgeToCenterOffset = matrixSize.Width > 2 ? matrixSize.Width / 2 : 0;
            int fromTopEdgeToCenterOffset = matrixSize.Height > 2 ? matrixSize.Height / 2 : 0;

            for (var m = 0; m < matrixSize.Height; m++)
            {
                for (var n = 0; n < matrixSize.Width; n++)
                {
                    int pixelPositionM = positionInImage.M - fromTopEdgeToCenterOffset + m;
                    int pixelPositionN = positionInImage.N - fromLeftEdgeToCenterOffset + n;

                    if (pixelPositionM >= 0
                        && pixelPositionM < this.Size.Height
                        && pixelPositionN >= 0
                        && pixelPositionN < this.Size.Width)
                    {
                        // pixel within image
                        neighbourMatrix[m, n] = this.image[pixelPositionM, pixelPositionN];
                    }
                    else
                    {
                        // pixel outside the image
                        neighbourMatrix[m, n] = White;
                    }
                }
            }

            return neighbourMatrix;
        }

        /// <summary>
        /// Gets the n coordinate of the left most pixel in the image; otherwise -1.
        /// </summary>
        public int FindLeftMostPixelIn()
        {
            for (int n = 0; n < this.Size.Width; n++)
            {
                for (int m = 0; m < this.Size.Height; m++)
                {
                    if (this.image[m, n] == Black)
                    {
                        return n;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the m coordinate of the top most pixel in the image; otherwise -1.
        /// </summary>
        public int FindTopMostPixelPosition()
        {
            for (int m = 0; m < this.Size.Height; m++)
            {
                for (int n = 0; n < this.Size.Width; n++)
                {
                    if (this.image[m, n] == Black)
                    {
                        return m;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the n coordinate of the right most pixel in the image; otherwise -1.
        /// </summary>
        public int FindRightMostPixelIn()
        {
            for (int n = this.Size.Width - 1; n >= 0; n--)
            {
                for (int m = 0; m < this.Size.Height; m++)
                {
                    if (this.image[m, n] == Black)
                    {
                        return n;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the m coordinate of the bottom most pixel in the image; otherwise -1.
        /// </summary>
        public int FindBottomLinePixelIn()
        {
            for (int m = this.Size.Height - 1; m >= 0; m--)
            {
                for (int n = 0; n < this.Size.Width; n++)
                {
                    if (this.image[m, n] == Black)
                    {
                        return m;
                    }
                }
            }

            return -1;
        }

        public string ToMatrixString()
        {
            return new BinaryMatrix(this.image).ToString();
        }

        public bool Equals(BinaryImage? other)
        {
            return other switch
            {
                null => false,
                _ => ReferenceEquals(this, other) || this.image.IsSequenceEqualTo(other.image)
            };
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.image.IsSequenceEqualTo(((BinaryImage)obj).image);
        }

        public override int GetHashCode()
        {
            return this.image.GetHashCode();
        }
    }
}