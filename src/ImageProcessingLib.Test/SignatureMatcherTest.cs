namespace ImageProcessingLib
{
    using System;
    using System.Drawing;
    using System.IO;

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

            TestDataResources.calculatedSign360_01.Save(@"C:\devNull\output\original_signature01.png");
            signature01Image.ToBitmap().Save(@"C:\devNull\output\test_signature01.png");
            template01Image.ToBitmap().Save(@"C:\devNull\output\test_template01Image.png");
            template02Image.ToBitmap().Save(@"C:\devNull\output\test_template02Image.png");

            bool firstResult = SignatureMatcher.IsMatch(signature01Image, template01Image);
            bool secondResult = SignatureMatcher.IsMatch(signature01Image, template02Image);

            firstResult.Should().BeTrue("signature of number 1 should match template of number 1");
            secondResult.Should().BeFalse("signature of number 1 must not match template of number 2");
        }
    }
}