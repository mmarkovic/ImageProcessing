namespace ImageProcessingLib
{
    /// <summary>
    /// Helper class containing functionality useful for test.
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// Interprets a <see cref="BinaryImage"/> from the <paramref name="input"/>.
        /// </summary>
        internal static BinaryImage InterpretToBinaryImage(string[] input)
        {
            int height = input.Length;
            int width = input[0].Length;
            var result = new byte[height, width];

            for (var m = 0; m < height; m++)
            {
                char[] charactersInRow = input[m].ToCharArray();
                for (var n = 0; n < width; n++)
                {
                    result[m, n] = charactersInRow[n] == '#' ? (byte)1 : (byte)0;
                }
            }

            return BinaryImage.FromByteArray(result);
        }
    }
}