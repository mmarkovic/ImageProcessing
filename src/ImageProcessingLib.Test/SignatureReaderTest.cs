namespace ImageProcessingLib
{
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
        public void GetsTheSignatureOfAnObjectUsing16SamplePoints()
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

            var result = SignatureReader.GetSignature(inputImage, 16);

            result.Should().HaveCount(16);
        }
    }
}