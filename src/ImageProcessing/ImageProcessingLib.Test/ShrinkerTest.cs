namespace ImageProcessingLib
{
    using FluentAssertions;

    using Xunit;

    public class ShrinkerTest
    {
        [Fact]
        public void ShrinksImagesToHalfItsSize()
        {
            var sampleImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 1, 1, 1, 0 },
                    { 0, 1, 1, 1, 1, 0 },
                    { 0, 1, 1, 1, 1, 0 },
                    { 0, 1, 1, 1, 1, 0 },
                    { 0, 0, 0, 0, 0, 0 },
                });

            var expected = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 },
                });

            var result = Shrinker.ShrinkByHalf(sampleImage);

            result.Should().Be(expected,
                $"result:\r\n{result.ToMatrixString()}\r\n should be as expected:\r\n{expected.ToMatrixString()}");
        }
    }
}