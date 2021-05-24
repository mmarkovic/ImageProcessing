namespace ImageProcessingLib
{
    /// <summary>
    /// Represents a pixel within a image that lies on the sampling line for determining the signature
    /// with its respectively distance (radius) to the center point.
    /// </summary>
    internal class SamplingPoint
    {
        internal SamplingPoint(MatrixPosition position, int radius)
        {
            this.Position = position;
            this.Radius = radius;
        }

        internal MatrixPosition Position { get; }

        internal int Radius { get; }
    }
}