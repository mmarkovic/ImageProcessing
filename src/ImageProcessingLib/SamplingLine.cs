namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a collection of points (a line) that is used to get the signature of an image object.
    /// </summary>
    internal class SamplingLine
    {
        private readonly List<SamplingPoint> samplingPoints = new();

        public void Add(SamplingPoint point)
        {
            this.samplingPoints.Add(point);
        }

        public bool ContainsPosition(MatrixPosition position)
        {
            return this.samplingPoints.Any(x => x.Position == position);
        }

        public IReadOnlyList<SamplingPoint> SamplingPoints => this.samplingPoints;
    }
}