namespace ImageProcessingLib
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a image in its binary form.
    /// The value 1 represents the color 'black' and the value 0 the color 'white'.
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
        public const byte White = 0;
        public const byte Black = 1;

        private readonly byte[,] image;

        public Size Size { get; }

        public byte this[int m, int n]
        {
            get => this.image[m, n];
            set => this.image[m, n] = value;
        }

        public BinaryImage(int width, int height)
        {
            this.image = new byte[height, width];
            this.Size = new Size(width, height);
        }

        private BinaryImage(byte[,] image)
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
            return new(image);
        }

        public static BinaryImage FromImage(Bitmap bmp)
        {
            // Defines the threshold which determines if a pixel should be painted in white or black.
            const int ThresholdValue = 120;
            const int BitsPerByte = 8;

            var image = new byte[bmp.Height, bmp.Width];
            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(rectangle, ImageLockMode.ReadOnly, bmp.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / BitsPerByte;

            var heightInPixels = bitmapData.Height;
            var widthInPixels = bitmapData.Width;

            unsafe
            {
                var ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(
                    0,
                    heightInPixels,
                    m =>
                    {
                        var ptrCurrentLine = ptrFirstPixel + (m * bitmapData.Stride);

                        for (var n = 0; n < widthInPixels; n++)
                        {
                            var byteX = n * bytesPerPixel;

                            // here the color white has the value of rgb(255, 255, 255).
                            if (ptrCurrentLine[byteX + 2] > ThresholdValue // red
                                || ptrCurrentLine[byteX + 1] > ThresholdValue // green
                                || ptrCurrentLine[byteX] > ThresholdValue) // blue
                            {
                                image[m, n] = White;
                            }
                            else
                            {
                                image[m, n] = Black;
                            }
                        }
                    });
            }

            bmp.UnlockBits(bitmapData);

            return new BinaryImage(image);
        }

        public BinaryImage Clone()
        {
            return new((byte[,])this.image.Clone());
        }

        public Bitmap ToBitmap()
        {
            const PixelFormat PixelFormat = PixelFormat.Format32bppRgb;
            var bmp = new Bitmap(this.Size.Width, this.Size.Height);
            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat) / 8;

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
                            var colorValue = (byte)(this.image[m, n] == White ? 255 : 0);

                            ptrCurrentLine[byteX] = colorValue; // blue
                            ptrCurrentLine[byteX + 1] = colorValue; // green
                            ptrCurrentLine[byteX + 2] = colorValue; // red
                        }
                    });
            }

            bmp.UnlockBits(bitmapData);

            return bmp;
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
        public BinaryMatrix GetNeighbourMatrixFromPosition(MatrixPosition positionInImage, Size matrixSize)
        {
            var neighbourMatrix = new BinaryMatrix(matrixSize);
            int fromLeftEdgeToCenterOffset = matrixSize.Width / 2;
            int fromTopEdgeToCenterOffset = matrixSize.Height / 2;

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

        public string ToMatrixString()
        {
            return new BinaryMatrix(this.image).ToString();
        }

        public bool Equals(BinaryImage other)
        {
            return other switch
            {
                null => false,
                _ => ReferenceEquals(this, other) || this.image.Equals(other.image)
            };
        }

        public override bool Equals(object obj)
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