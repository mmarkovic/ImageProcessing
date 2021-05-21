namespace ImageProcessingLib
{
    using FluentAssertions;

    using Xunit;

    public class CropperTest
    {
        [Fact]
        public void CropsLargeImages()
        {
            var sampleImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 1, 1, 1, 0, 0 },
                    { 0, 0, 0, 1, 1, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 1, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 0, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                });

            var expected = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 1, 1, 0, 0 },
                    { 0, 0, 0, 1, 1, 1, 0, 0 },
                    { 0, 0, 1, 1, 1, 1, 0, 0 },
                    { 0, 0, 0, 1, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0 }
                });

            var result = Cropper.CropAroundFigures(sampleImage);

            result.Should().Be(expected);
        }
    }
}