namespace ImageProcessingLib
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Compares two multidimensional byte arrays and returns true, if their values are exactly the same;
        /// otherwise false.
        /// </summary>
        public static bool IsSequenceEqualTo(this bool[,] left, bool[,] right)
        {
            int width = left.GetLength(0);
            if (width != right.GetLength(0))
            {
                return false;
            }

            int height = left.GetLength(1);
            if (height != right.GetLength(1))
            {
                return false;
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (left[x, y] != right[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}