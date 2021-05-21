namespace ImageProcessingLib
{
    using FluentAssertions;

    using Xunit;

    public class BinaryMatrixTest
    {
        [Fact]
        public void CreatesNewFromMatrixString()
        {
            const string SampleMatrixString = "010\r\n101\r\n110";
            var expected = new BinaryMatrix(
                new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 0, 1 },
                    { 1, 1, 0 }
                });

            var result = BinaryMatrix.FromMatrixString(SampleMatrixString);

            result.Should().Be(expected);
        }

        [Fact]
        public void DisplaysContentAsMatrixString()
        {
            const string Expected = "010\r\n101\r\n110";
            var sampleMatrix = new BinaryMatrix(
                new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 0, 1 },
                    { 1, 1, 0 }
                });

            var result = sampleMatrix.ToString();

            result.Should().Be(Expected);
        }
    }
}