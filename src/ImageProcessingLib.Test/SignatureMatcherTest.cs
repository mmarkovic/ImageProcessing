namespace ImageProcessingLib
{
    using FluentAssertions;

    using ImageProcessingLib.TestData;

    using Xunit;

    public class SignatureMatcherTest
    {
        [Fact]
        public void VerifiesIfSignatureMatchesOnTemplate()
        {
            var signatureImage = TestHelper.InterpretToBinaryImage(
                new[]
                {
                    "           ",
                    "    ###    ",
                    "###########",
                    "    ###    ",
                    "           "
                }
            );

            var templateImage = TestHelper.InterpretToBinaryImage(
                new[]
                {
                    "###########",
                    "####   ####",
                    "           ",
                    "####   ####",
                    "###########"
                }
            );

            var result = SignatureMatcher.IsMatch(signatureImage, templateImage);

            result.Should().BeTrue("signature matches template exactly");
        }

        [Fact]
        public void VerifySignatureWith360SamplingPointsOfNumber1MatchesFittingTemplate()
        {
            var signature01Image = BinaryImage.FromImage(TestDataResources.calculatedSign360_01);
            var template01Image = BinaryImage.FromImage(TestDataResources.signatureTemplate360_01);
            var template02Image = BinaryImage.FromImage(TestDataResources.signatureTemplate360_02);

            bool firstResult = SignatureMatcher.IsMatch(signature01Image, template01Image);
            bool secondResult = SignatureMatcher.IsMatch(signature01Image, template02Image);

            firstResult.Should().BeTrue("signature of number 1 should match template of number 1");
            secondResult.Should().BeFalse("signature of number 1 must not match template of number 2");
        }
    }
}