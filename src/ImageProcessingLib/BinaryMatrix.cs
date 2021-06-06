namespace ImageProcessingLib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a matrix consisting of only the values 0 and 1.
    /// </summary>
    /// <remarks>
    /// A matrix is defined by it's columns (m) and rows (n).
    /// <![CDATA[
    /// Example of a 3x2 (m=3, n=2) matrix:
    /// +-----+
    /// | 0 1 |
    /// | 1 0 |
    /// | 1 1 |
    /// +-----+
    ///
    /// The values can be access through their (m,n) zero based coordinates:
    /// +-------------+
    /// | (0,0) (1,0) |
    /// | (1,0) (1,1) |
    /// | (2,0) (2,1) |
    /// +-------------+
    /// ]]>
    /// </remarks>
    public sealed class BinaryMatrix : IEquatable<BinaryMatrix>
    {
        public static readonly byte WhiteValue = Convert.ToByte(White);
        public static readonly byte BlackValue = Convert.ToByte(Black);

        public const bool White = BinaryImage.White;
        public const bool Black = BinaryImage.Black;

        private readonly bool[,] value;

        public Size Size { get; }

        public bool this[int m, int n]
        {
            get => this.value[m, n];
            set => this.value[m, n] = value;
        }

        public BinaryMatrix(byte[,] value)
        {
            int height = value.GetLength(0);
            int width = value.GetLength(1);
            bool[,] binaryArray = new bool[height, width];
            Parallel.For(
                0,
                height,
                m =>
                {
                    for (int n = 0; n < width; n++)
                    {
                        binaryArray[m, n] = value[m, n] > 0 ? Black : White;
                    }
                });

            this.value = binaryArray;
            this.Size = new Size(value.GetLength(1), value.GetLength(0));
        }

        public BinaryMatrix(bool[,] value)
        {
            this.value = value;
            this.Size = new Size(value.GetLength(1), value.GetLength(0));
        }

        public BinaryMatrix(Size size)
        {
            this.value = new bool[size.Height, size.Width];
            this.Size = size;
        }

        public static bool operator ==(BinaryMatrix left, BinaryMatrix right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BinaryMatrix left, BinaryMatrix right)
        {
            return !Equals(left, right);
        }

        public static BinaryMatrix FromMatrixString(string matrixString)
        {
            char[] characters = matrixString
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace(" ", string.Empty)
                .ToCharArray();

            var imageAsTwoDimensionalList = new List<List<bool>>();
            var currentLine = new List<bool>();

            foreach (char character in characters)
            {
                switch (character)
                {
                    case '1':
                        currentLine.Add(Black);
                        break;
                    case '0':
                        currentLine.Add(White);
                        break;
                    case '\n':
                        imageAsTwoDimensionalList.Add(currentLine);
                        currentLine = new List<bool>();
                        break;
                    default:
                        throw new ArgumentException($"Invalid character: {character}");
                }
            }

            // add last line, that doesn't end with '\n'
            if (currentLine.Count > 0)
            {
                imageAsTwoDimensionalList.Add(currentLine);
            }

            var imageAsByteArray = new bool[currentLine.Count, imageAsTwoDimensionalList.Count];
            for (var m = 0; m < imageAsTwoDimensionalList.Count; m++)
            {
                for (var n = 0; n < currentLine.Count; n++)
                {
                    imageAsByteArray[m, n] = imageAsTwoDimensionalList[m][n];
                }
            }

            return new BinaryMatrix(imageAsByteArray);
        }

        public bool Equals(BinaryMatrix? other)
        {
            return !ReferenceEquals(null, other)
                   && (ReferenceEquals(this, other) || this.value.IsSequenceEqualTo(other.value));
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is BinaryMatrix other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// Calculates the average values of the current matrix.
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
        public float GetAverageValue()
        {
            var sum = 0;

            for (var m = 0; m < this.Size.Height; m++)
            {
                for (var n = 0; n < this.Size.Width; n++)
                {
                    sum += this.value[m, n] == Black ? BlackValue : WhiteValue;
                }
            }

            int numberOfValues = this.Size.Height * this.Size.Width;

            return sum / (float)numberOfValues;
        }

        public bool[,] To2DBoolArray()
        {
            return this.value;
        }

        public override string ToString()
        {
            const int MaxCharactersToPrint = 150;
            var numberOfCharactersPrinted = 0;

            var stringBuilder = new StringBuilder();

            for (var m = 0; m < this.Size.Height; m++)
            {
                for (var n = 0; n < this.Size.Width; n++)
                {
                    stringBuilder.Append(this.value[m, n] == Black ? BlackValue : WhiteValue);
                    numberOfCharactersPrinted++;

                    if (numberOfCharactersPrinted > MaxCharactersToPrint)
                    {
                        return stringBuilder + $"... ({this.Size.Height}x{this.Size.Width})";
                    }
                }

                // only add a new line if it's not the last row
                if (m + 1 < this.Size.Height)
                {
                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }
    }
}