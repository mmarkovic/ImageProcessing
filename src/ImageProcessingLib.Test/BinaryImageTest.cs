namespace ImageProcessingLib
{
    using System;

    using FluentAssertions;

    using ImageProcessingLib.TestData;

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
            sampleImage[m, 0].Should().Be(BinaryImage.White);
            sampleImage[m, 1].Should().Be(BinaryImage.Black);
            sampleImage[m, 2].Should().Be(BinaryImage.White);

            // second row: m=1
            m = 1;
            sampleImage[m, 0].Should().Be(BinaryImage.Black);
            sampleImage[m, 1].Should().Be(BinaryImage.White);
            sampleImage[m, 2].Should().Be(BinaryImage.Black);

            // third row: m=2
            m = 2;
            sampleImage[m, 0].Should().Be(BinaryImage.Black);
            sampleImage[m, 1].Should().Be(BinaryImage.White);
            sampleImage[m, 2].Should().Be(BinaryImage.White);
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

        [Fact]
        public void GetNeighborMatrixFromPosition()
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

            sampleImage.GetNeighborMatrixFromPosition(0, 0, 2).ToString().Should().Be("00\r\n01");
            sampleImage.GetNeighborMatrixFromPosition(1, 0, 2).ToString().Should().Be("01\r\n01");
            sampleImage.GetNeighborMatrixFromPosition(0, 1, 2).ToString().Should().Be("00\r\n11");
            sampleImage.GetNeighborMatrixFromPosition(1, 1, 2).ToString().Should().Be("11\r\n11");
        }

        [Fact]
        public void CanConvertImagesToBinaryImageAndBack()
        {
            var signature01Image = BinaryImage.FromImage(TestDataResources.calculatedSign360_01);
            signature01Image.IsEmpty().Should().BeFalse("signature must not be empty");

            var converted = signature01Image.ToBitmap();

            var convertedBackSignature01Image = BinaryImage.FromImage(converted);
            convertedBackSignature01Image.IsEmpty().Should().BeFalse("signature must not be empty");

            signature01Image.Should().Be(convertedBackSignature01Image);
        }

        [Fact]
        public void CanConvertSignatureTemplateImagesToBinaryImageAndBack()
        {
            var templateImage = BinaryImage.FromImage(TestDataResources.signatureTemplate360_00);
            templateImage.IsEmpty().Should().BeFalse("signature template must not be empty");

            var converted = templateImage.ToBitmap();

            var convertedBackImage = BinaryImage.FromImage(converted);
            convertedBackImage.IsEmpty().Should().BeFalse("signature template must not be empty");

            templateImage.Should().Be(convertedBackImage);
        }

        [Fact]
        public void CanReadSignatureTemplates()
        {
            var binaryImage = BinaryImage.FromImage(TestDataResources.sampleSignTemplate);

            string test = binaryImage.ToMatrixString();

            test.Should().Be(
                "1111100" + Environment.NewLine +
                "1111000" + Environment.NewLine +
                "1111000" + Environment.NewLine +
                "1111000" + Environment.NewLine +
                "1110000" + Environment.NewLine +
                "1110000" + Environment.NewLine +
                "1100000" + Environment.NewLine +
                "1100000" + Environment.NewLine +
                "1000000");
        }
    }
}