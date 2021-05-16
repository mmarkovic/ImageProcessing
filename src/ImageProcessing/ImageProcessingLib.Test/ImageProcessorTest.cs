namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Drawing;

    using FluentAssertions;

    using Xunit;

    public class ImageProcessorTest
    {
        [Fact]
        public void MatchesMatrixToStructuringElement()
        {
            var structuringElement = new StructuringElement(
                new[,]
                {
                    { 0, 0, 0 },
                    { -1, -1, -1 },
                    { 1, 1, 1 }
                });

            var matchingMatrix = new BinaryMatrix(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 1, 0, 0 },
                    { 1, 1, 1 }
                });

            bool result = ImageProcessor.MatchMatrixToStructuringElement(
                matchingMatrix,
                structuringElement);

            result.Should().BeTrue();
        }

        [Fact]
        public void AppliesThinningOnImage()
        {
            var structuringElement = new StructuringElement(
                new[,]
                {
                    { 0, 0, -1 },
                    { 1, 1, -1 },
                    { -1, -1, -1 }
                });

            var inputImage = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 1, 1, 1 },
                    { 1, 1, 1 }
                });

            var expected = BinaryImage.FromByteArray(
                new byte[,]
                {
                    { 0, 0, 0 },
                    { 0, 1, 1 },
                    { 0, 0, 0 }
                });

            var structuringElements = new []{ structuringElement };
            var result = ImageProcessor.Thinning(inputImage, structuringElements);
            result.Should().Be(
                expected,
                $"result:\r\n{result.ToMatrixString()}\r\n should be as expected:\r\n{expected.ToMatrixString()}");
        }

        [Theory]
        [MemberData(nameof(NeighbourMatrixTestData))]
        public void GetsNeighbourMatrixFromPosition(
            BinaryImage binaryImage,
            MatrixPosition positionInImage,
            Size sizeOfNeighbourMatrix,
            BinaryMatrix expectedResult)
        {
            var result = binaryImage.GetNeighbourMatrixFromPosition(positionInImage, sizeOfNeighbourMatrix);

            result.Should().Be(expectedResult);
        }

        public static IEnumerable<object[]> NeighbourMatrixTestData =>
            new List<object[]>
            {
                new object[]
                {
                    BinaryImage.FromByteArray(
                        new byte[,]
                        {
                            { 1, 1, 1 },
                            { 1, 0, 1 },
                            { 1, 1, 1 },
                        }),
                    new MatrixPosition(0, 0),
                    new Size(3, 3),
                    new BinaryMatrix(
                        new byte[,]
                        {
                            { 0, 0, 0 },
                            { 0, 1, 1 },
                            { 0, 1, 0 }
                        })
                },
                new object[]
                {
                    BinaryImage.FromByteArray(
                        new byte[,]
                        {
                            { 1, 1, 1 },
                            { 1, 0, 1 },
                            { 1, 1, 1 },
                        }),
                    new MatrixPosition(0, 1),
                    new Size(3, 3),
                    new BinaryMatrix(
                        new byte[,]
                        {
                            { 0, 0, 0 },
                            { 1, 1, 1 },
                            { 1, 0, 1 }
                        })
                },
                new object[]
                {
                    BinaryImage.FromByteArray(
                        new byte[,]
                        {
                            { 1, 1, 1 },
                            { 1, 0, 1 },
                            { 1, 1, 1 },
                        }),
                    new MatrixPosition(0, 2),
                    new Size(3, 3),
                    new BinaryMatrix(
                        new byte[,]
                        {
                            { 0, 0, 0 },
                            { 1, 1, 0 },
                            { 0, 1, 0 }
                        })
                }
            };
    }
}