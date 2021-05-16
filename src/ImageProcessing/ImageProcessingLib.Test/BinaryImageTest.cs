namespace ImageProcessingLib
{
    using FluentAssertions;

    using Xunit;

    public class BinaryImageTest
    {
        [Fact]
        public void InterpretsMatrixCoordinatesCorrectly()
        {
            var sampleImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 0, 1 },
                    { 1, 0, 0 }
                });

            // first row
            int m = 0;
            sampleImage[m, 0].Should().Be(0);
            sampleImage[m, 1].Should().Be(1);
            sampleImage[m, 2].Should().Be(0);

            // second row: m=1
            m = 1;
            sampleImage[m, 0].Should().Be(1);
            sampleImage[m, 1].Should().Be(0);
            sampleImage[m, 2].Should().Be(1);

            // third row: m=2
            m = 2;
            sampleImage[m, 0].Should().Be(1);
            sampleImage[m, 1].Should().Be(0);
            sampleImage[m, 2].Should().Be(0);
        }

        [Fact]
        public void ComparesTwoImages()
        {
            var firstImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 }
                });

            var secondImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 0 }
                });

            firstImage.Should().Be(secondImage);
        }

        [Fact]
        public void ClonesImages()
        {
            var firstImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 1, 1, 0 },
                    { 0, 0, 1 }
                });

            var secondImage = firstImage.Clone();

            firstImage.Should().Be(secondImage);
        }

        [Fact]
        public void WritesImageToMatrixString()
        {
            const string Expected = "010\r\n110\r\n001";

            var sampleImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 0 },
                    { 0, 0, 1 }
                });

            string result = sampleImage.ToMatrixString();

            result.Should().Be(Expected);
        }
    }
}