namespace ImageProcessingLib
{
    using FluentAssertions;

    using Xunit;

    public class ThinnerTest
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

            bool result = Thinner.MatchMatrixToStructuringElement(
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
                    { 1, 0, 0 },
                    { 1, 1, 1 }
                });

            var structuringElements = new[] { structuringElement };
            var result = Thinner.Thinning(inputImage, structuringElements);
            result.Should().Be(
                expected,
                $"result:\r\n{result.ToMatrixString()}\r\n should be as expected:\r\n{expected.ToMatrixString()}");
        }

        [Fact]
        public void AppliesThinningMultipleTimes()
        {
            var inputImage = TestHelper.InterpretToBinaryImage(
                    new[]
                    {
                        "###########",
                        "#########  ",
                        "#########  ",
                        "#########  ",
                        "###  ####  "
                    });

            var expected = TestHelper.InterpretToBinaryImage(
                    new[]
                    {
                        "#       ###",
                        "##     ##  ",
                        " #######   ",
                        "##     #   ",
                        "#      ##  "
                    });

            var result = Thinner.Thinning(inputImage, 2);

            result.Should().Be(
                expected,
                $"result:\r\n{result.ToMatrixString()}\r\n should be as expected:\r\n{expected.ToMatrixString()}");
        }

        [Fact]
        public void ApplyStructuralElementsStepByStep()
        {
            var originalImage = new[]
            {
                "###########",
                "#########  ",
                "#########  ",
                "#########  ",
                "###  ####  "
            };

            var expectedA1 = new[]
            {
                "#       ###",
                "#########  ",
                "#########  ",
                "#########  ",
                "###  ####  "
            };

            var expectedA2 = expectedA1; // no changes

            var expectedA3 = new[]
            {
                "#       ###",
                "#########  ",
                "########   ",
                "########   ",
                "###  ####  "
            };

            var expectedA4 = new[]
            {
                "#       ###",
                "#########  ",
                "########   ",
                "########   ",
                "##   ####  "
            };

            var expectedA5 = new[]
            {
                "#       ###",
                "#########  ",
                "########   ",
                "### ####   ",
                "#      ##  "
            };

            var expectedA6 = new[]
            {
                "#       ###",
                "#########  ",
                "########   ",
                "###  ###   ",
                "#      ##  "
            };

            var expectedA7 = new[]
            {
                "#       ###",
                "#########  ",
                " #######   ",
                "###  ###   ",
                "#      ##  "
            };

            var expectedA8 = expectedA7; // no changes

            var expectedA9 = new[]
            {
                "#       ###",
                "##     ##  ",
                " #######   ",
                "###  ###   ",
                "#      ##  "
            };

            var expectedA12 = new[]
            {
                "#       ###",
                "##     ##  ",
                " #######   ",
                "##   ###   ",
                "#      ##  "
            };

            var expectedA13 = new[]
            {
                "#       ###",
                "##     ##  ",
                " #######   ",
                "##    ##   ",
                "#      ##  "
            };

            var expectedA14 = new[]
            {
                "#       ###",
                "##     ##  ",
                " #######   ",
                "##     #   ",
                "#      ##  "
            };

            var inputImage = TestHelper.InterpretToBinaryImage(originalImage);

            var expectedA1Image = TestHelper.InterpretToBinaryImage(expectedA1);
            var imageA1 = Thinner.Thinning(inputImage, new[] { Thinner.B1 });
            imageA1.Should().Be(expectedA1Image);

            var expectedA2Image = TestHelper.InterpretToBinaryImage(expectedA2);
            var imageA2 = Thinner.Thinning(imageA1, new[] { Thinner.B2 });
            imageA2.Should().Be(expectedA2Image);

            var expectedA3Image = TestHelper.InterpretToBinaryImage(expectedA3);
            var imageA3 = Thinner.Thinning(imageA2, new[] { Thinner.B3 });
            imageA3.Should().Be(expectedA3Image);

            var imageA3Sequence = Thinner.Thinning(inputImage, new[] { Thinner.B1, Thinner.B2, Thinner.B3 });
            imageA3Sequence.Should().Be(expectedA3Image);

            var expectedA4Image = TestHelper.InterpretToBinaryImage(expectedA4);
            var imageA4 = Thinner.Thinning(imageA3, new[] { Thinner.B4 });
            imageA4.Should().Be(expectedA4Image);

            var imageA4Sequence = Thinner.Thinning(inputImage, new[] { Thinner.B1, Thinner.B2, Thinner.B3, Thinner.B4 });
            imageA4Sequence.Should().Be(expectedA4Image,
                $"result:\r\n{imageA4Sequence.ToMatrixString()}\r\nshould be:\r\n{expectedA4Image.ToMatrixString()}");

            var expectedA5Image = TestHelper.InterpretToBinaryImage(expectedA5);
            var imageA5 = Thinner.Thinning(imageA4, new[] { Thinner.B5 });
            imageA5.Should().Be(expectedA5Image);

            var expectedA6Image = TestHelper.InterpretToBinaryImage(expectedA6);
            var imageA6 = Thinner.Thinning(imageA5, new[] { Thinner.B6 });
            imageA6.Should().Be(expectedA6Image);

            var expectedA7Image = TestHelper.InterpretToBinaryImage(expectedA7);
            var imageA7 = Thinner.Thinning(imageA6, new[] { Thinner.B7 });
            imageA7.Should().Be(expectedA7Image);

            var expectedA8Image = TestHelper.InterpretToBinaryImage(expectedA8);
            var imageA8 = Thinner.Thinning(imageA7, new[] { Thinner.B8 });
            imageA8.Should().Be(expectedA8Image);

            var expectedA9Image = TestHelper.InterpretToBinaryImage(expectedA9);
            var imageA9 = Thinner.Thinning(imageA8, new[] { Thinner.B1 });
            imageA9.Should().Be(expectedA9Image);

            var imageA10 = Thinner.Thinning(imageA9, new[] { Thinner.B2 });
            imageA10.Should().Be(expectedA9Image);

            var imageA11 = Thinner.Thinning(imageA10, new[] { Thinner.B3 });
            imageA11.Should().Be(expectedA9Image);

            var expectedA12Image = TestHelper.InterpretToBinaryImage(expectedA12);
            var imageA12 = Thinner.Thinning(imageA11, new[] { Thinner.B4 });
            imageA12.Should().Be(expectedA12Image);

            var expectedA13Image = TestHelper.InterpretToBinaryImage(expectedA13);
            var imageA13 = Thinner.Thinning(imageA12, new[] { Thinner.B5 });
            imageA13.Should().Be(expectedA13Image);

            var expectedA14Image = TestHelper.InterpretToBinaryImage(expectedA14);
            var imageA14 = Thinner.Thinning(imageA13, new[] { Thinner.B6 });
            imageA14.Should().Be(
                expectedA14Image,
                $"result:\r\n{imageA14.ToMatrixString()}\r\nshould be:\r\n{expectedA14Image.ToMatrixString()}");
        }

        [Fact]
        public void ApplyStructuralElementsInSequence()
        {
            var originalImage = new[]
            {
                "###########",
                "#########  ",
                "#########  ",
                "#########  ",
                "###  ####  "
            };

            var expectedA4 = new[]
            {
                "#       ###",
                "#########  ",
                "########   ",
                "########   ",
                "##   ####  "
            };

            var expectedA8 = new[]
            {
                "#       ###",
                "#########  ",
                " #######   ",
                "###  ###   ",
                "#      ##  "
            };

            var inputImage = TestHelper.InterpretToBinaryImage(originalImage);
            var expectedA8Image = TestHelper.InterpretToBinaryImage(expectedA8);

            var imageA8 = Thinner.Thinning(inputImage);
            var structuringElements = new[]
            {
                Thinner.B1, Thinner.B2, Thinner.B3, Thinner.B4, Thinner.B5, Thinner.B6, Thinner.B7, Thinner.B8
            };

            var imageA4 = Thinner.Thinning(inputImage, new[] { Thinner.B1, Thinner.B2, Thinner.B3, Thinner.B4 });
            var expectedA4Image = TestHelper.InterpretToBinaryImage(expectedA4);
            imageA4.Should().Be(expectedA4Image);

            // same operation, but implicit with all structuring elements as parameter
            var secondImageA8 = Thinner.Thinning(inputImage, structuringElements);

            imageA8.Should().Be(expectedA8Image,
                $"result:\r\n{imageA8.ToMatrixString()}\r\nshould be:\r\n{expectedA8Image.ToMatrixString()}");
            secondImageA8.Should().Be(imageA8);
        }
    }
}