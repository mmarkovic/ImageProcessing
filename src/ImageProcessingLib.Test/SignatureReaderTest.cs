namespace ImageProcessingLib
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Xunit;

    public class SignatureReaderTest
    {
        [Fact]
        public void GetsTheCenterOfAnObject()
        {
            var inputImage = TestHelper.InterpretToBinaryImage(
                new[]
                {
                    "     ###  ",
                    "    #   # ",
                    "   #     #",
                    "   #     #", // center: m = 3, n = 6
                    "   #     #",
                    "    #   # ",
                    "     ###  "
                });

            var result = SignatureReader.GetCenterOfObjectIn(inputImage);

            result.M.Should().Be(3);
            result.N.Should().Be(6);
        }

        [Fact]
        public void GetsTheSignatureOfAnObjectUsing8SamplePoints()
        {
            var inputImage = TestHelper.InterpretToBinaryImage(
                new[]
                {
                    "     ###  ",
                    "    #   # ",
                    "   #     #",
                    "   #     #", // center: m = 3, n = 6
                    "   #     #",
                    "    #   # ",
                    "     ###  "
                });
            int[] expectedSignature = { 3, 3, 3, 3, 3, 3, 3, 3 };

            var result = SignatureReader.GetSignature(inputImage, 8);

            result.Should().HaveCount(8);
            var radiusPoints = result.Values.Select(x => x.Any() ? x[0] : 0).ToList();
            radiusPoints.Should().ContainInOrder(expectedSignature);
        }

        [Theory]
        [MemberData(nameof(PixelCoordinatesByAngle))]
        public void GetsTheCoordinatesOfAllPixelsOnALineDefinedByAStartingPointAndAngle0(
            string testcaseName,
            int angle,
            MatrixPosition[] expectedResult)
        {
            //// [ (0,0)  (0,1)  (0,2)  (0,3)  (0,4)  (0,5)  (0,6)  (0,7)  (0,8)  (0,9) ]
            //// [ (1,0)  (1,1)  (1,2)  (1,3)  (1,4)  (1,5)  (1,6)  (1,7)  (1,8)  (1,9) ]
            //// [ (2,0)  (2,1)  (2,2)  (2,3)  (2,4)  (2,5)  (2,6)  (2,7)  (2,8)  (2,9) ]
            //// [ (3,0)  (3,1)  (3,2)  (3,3)  (3,4)  (3,5)  (3,6)  (3,7)  (3,8)  (3,9) ]
            //// [ (4,0)  (4,1)  (4,2)  (4,3)  (4,4)  (4,5)  (4,6)  (4,7)  (4,8)  (4,9) ]
            //// [ (5,0)  (5,1)  (5,2)  (5,3)  (5,4)    X    (5,6)  (5,7)  (5,8)  (5,9) ]   x = Starting Point (m,n)
            //// [ (6,0)  (6,1)  (6,2)  (6,3)  (6,4)  (6,5)  (6,6)  (6,7)  (6,8)  (6,9) ]
            //// [ (7,0)  (7,1)  (7,2)  (7,3)  (7,4)  (7,5)  (7,6)  (7,7)  (7,8)  (7,9) ]
            //// [ (8,0)  (8,1)  (8,2)  (8,3)  (8,4)  (8,5)  (8,6)  (8,7)  (8,8)  (8,9) ]
            //// [ (9,0)  (9,1)  (9,2)  (9,3)  (9,4)  (9,5)  (9,6)  (9,7)  (9,8)  (9,9) ]
            var inputImage = new BinaryImage(10, 10);
            var startingPoint = new MatrixPosition(5, 5);

            var result = SignatureReader.GetAllCoordinatesFromStartingPointToEdgeInAngle(
                inputImage,
                startingPoint,
                angle);
            var resultingPositions = result.Select(x => x.Position).ToArray();

            resultingPositions.Should().ContainInOrder(expectedResult, testcaseName);
            resultingPositions.Should().HaveCount(expectedResult.Length);
        }

        public static IEnumerable<object[]> PixelCoordinatesByAngle =>
            new List<object[]>
            {
                new object[]
                {
                    "straight left",
                    0,
                    new MatrixPosition[] { new(5, 4), new(5, 3), new(5, 2), new(5, 1), new(5, 0) }
                },
                new object[]
                {
                    "diagonal top left",
                    45,
                    new MatrixPosition[] { new(4, 4), new(3, 3), new(2, 2), new(1, 1), new(0, 0) }
                },
                new object[]
                {
                    "straight top",
                    90,
                    new MatrixPosition[] { new(4, 5), new(3, 5), new(2, 5), new(1, 5), new(0, 5) }
                },
                new object[]
                {
                    "diagonal top right",
                    135,
                    new MatrixPosition[] { new(4, 6), new(3, 7), new(2, 8), new(1, 9) }
                },
                new object[]
                {
                    "straight right",
                    180,
                    new MatrixPosition[] { new(5, 6), new(5, 7), new(5, 8), new(5, 9) }
                },
                new object[]
                {
                    "diagonal bottom right",
                    225,
                    new MatrixPosition[] { new(6, 6), new(7, 7), new(8, 8), new(9, 9) }
                },
                new object[]
                {
                    "straight down",
                    270,
                    new MatrixPosition[] { new(6, 5), new(7, 5), new(8, 5), new(9, 5) }
                },
                new object[]
                {
                    "diagonal bottom left",
                    315,
                    new MatrixPosition[] { new(6, 4), new(7, 3), new(8, 2), new(9, 1) }
                }
            };
    }
}