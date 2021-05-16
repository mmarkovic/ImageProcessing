namespace ImageProcessingLib
{
    /// <summary>
    /// Represents a position in a matrix.
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    /// Positions in a matrix are determined by their row index (m) and their column index (n).
    ///
    /// a[m,n] -> position a at row index (m) and column index (n)
    ///
    ///   ------------------------------------> x, n (zero based column index)
    ///   | a[0,0]  a[0,1]  a[0,2]  ..  a[0, n]
    ///   | a[1,0]  a[1,1]  a[1,2]  ..  a[1, n]
    ///   | a[2,0]  a[2,1]  a[2,2]  ..  a[2, n]
    ///   | .       .       .       ..  .
    ///   | a[m,0]  a[m,1]  a[m,2]  ..  a[m,n]
    ///   v
    ///  y, m (zero based row index)
    /// ]]>
    /// </remarks>
    public class MatrixPosition
    {
        public MatrixPosition(int m, int n)
        {
            this.M = m;
            this.N = n;
        }

        /// <summary>
        /// Gets the row index of a position within a matrix.
        /// </summary>
        public int M { get; }

        /// <summary>
        /// Gets the column index of a position within a matrix.
        /// </summary>
        public int N { get; }

        public override string ToString()
        {
            return $"[{this.M},{this.N}]";
        }
    }
}