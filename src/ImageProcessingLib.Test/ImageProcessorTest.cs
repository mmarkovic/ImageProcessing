namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Drawing;

    using FluentAssertions;

    using Xunit;

    public class ImageProcessorTest
    {

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