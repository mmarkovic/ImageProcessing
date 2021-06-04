namespace ImageProcessingLib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

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
        private readonly byte[,] value;

        public Size Size { get; }

        public byte this[int m, int n]
        {
            get => this.value[m, n];
            set => this.value[m, n] = value;
        }

        public BinaryMatrix(byte[,] value)
        {
            this.value = value;
            this.Size = new Size(value.GetLength(1), value.GetLength(0));
        }

        public BinaryMatrix(Size size)
        {
            this.value = new byte[size.Height, size.Width];
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

            var imageAsTwoDimensionalList = new List<List<byte>>();
            var currentLine = new List<byte>();

            foreach (char character in characters)
            {
                switch (character)
                {
                    case '1':
                        currentLine.Add(1);
                        break;
                    case '0':
                        currentLine.Add(0);
                        break;
                    case '\n':
                        imageAsTwoDimensionalList.Add(currentLine);
                        currentLine = new List<byte>();
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

            var imageAsByteArray = new byte[currentLine.Count, imageAsTwoDimensionalList.Count];
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
                    sum += this.value[m, n];
                }
            }

            int numberOfValues = this.Size.Height * this.Size.Width;

            return sum / (float)numberOfValues;
        }

        public override string ToString()
        {
            const int MaxCharactersToPrint = 100;
            var numberOfCharactersPrinted = 0;

            var stringBuilder = new StringBuilder();

            for (var m = 0; m < this.Size.Height; m++)
            {
                for (var n = 0; n < this.Size.Width; n++)
                {
                    stringBuilder.Append(this.value[m, n]);
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