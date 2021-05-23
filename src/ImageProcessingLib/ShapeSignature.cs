namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains the signature of a shape.
    /// </summary>
    public class ShapeSignature
    {
        private readonly IDictionary<int, IReadOnlyList<int>> value;

        public ShapeSignature(int numberOfSamplingPoints)
        {
            this.NumberOfSamplingPoints = numberOfSamplingPoints;
            this.value = new Dictionary<int, IReadOnlyList<int>>(numberOfSamplingPoints);
        }

        /// <summary>
        /// Gets the number of sampling points used for calculating the signature.
        /// The more points used, the more precise is the signature. The Maximal number is 360.
        /// </summary>
        public int NumberOfSamplingPoints { get; }

        public int[][] Get()
        {
            int[][] result = new int[this.NumberOfSamplingPoints][];

            for (int i = 0; i < this.NumberOfSamplingPoints; i++)
            {
                result[i] = this.value[i].ToArray();
            }

            return result;
        }

        internal void Add(int samplingPointIndex, int[] pointsInAngle)
        {
            this.value.Add(samplingPointIndex, pointsInAngle);
        }
    }
}