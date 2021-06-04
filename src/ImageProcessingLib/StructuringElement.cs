namespace ImageProcessingLib
{
    using System.Drawing;
    using System.Text;

    /// <summary>
    /// Represents a structuring element matrix for masking a <see cref="BinaryImage"/>.
    /// The values for black and white are the same as for <see cref="BinaryImage.Black"/> and
    /// <see cref="BinaryImage.White"/>. Values, that should be ignored are represented by the <strong>-1</strong>
    /// respectively as a <strong>x</strong> in the string.
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    ///  Sample structuring element:
    ///   . - - - -> x, n
    ///   | x x x
    ///   | x 1 1
    ///   | x 0 0
    ///   v
    ///   y, m
    /// Note: The x in the string representation of the structuring element matrix has the numeric value (-1) and
    ///       indicates that this pixel/position should be ignored for evaluation.
    ///       The x achsis represents the whidth and
    ///       the y achsis represents the height.
    /// ]]>
    /// </remarks>
    public sealed class StructuringElement
    {
        private const int Ignore = -1;
        private const string IgnoreCharacter = "x";

        private readonly int[,] value;

        public Size Size => new(this.value.GetLength(0), this.value.GetLength(1));

        public int this[int m, int n] => this.value[m, n];

        public StructuringElement(int[,] value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            const int MaxCharactersToPrint = 20;
            var numberOfCharactersPrinted = 0;

            var stringBuilder = new StringBuilder();

            for (var m = 0; m < this.Size.Height; m++)
            {
                for (var n = 0; n < this.Size.Width; n++)
                {
                    string character = this.value[m, n] == Ignore ? IgnoreCharacter : this.value[m, n].ToString();

                    stringBuilder.Append(character);
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