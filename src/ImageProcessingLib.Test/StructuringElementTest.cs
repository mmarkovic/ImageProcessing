namespace ImageProcessingLib
{
    using FluentAssertions;

    using Xunit;

    public class StructuringElementTest
    {
        [Fact]
        public void DisplaysContentAsMatrixString()
        {
            const string Expected = "x10\r\nx01\r\nx1x";
            var sampleStructuringElement = new StructuringElement(
                new[,]
                {
                    { -1, 1, 0 },
                    { -1, 0, 1 },
                    { -1, 1, -1 }
                });

            var result = sampleStructuringElement.ToString();

            result.Should().Be(Expected);
        }
    }
}