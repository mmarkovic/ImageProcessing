namespace ImageProcessingLib
{
    /// <summary>
    /// Verifies if a given image (signature) matches on a template image (template).
    /// </summary>
    internal static class SignatureMatcher
    {
        internal static bool IsMatch(BinaryImage signatureImage, BinaryImage templateImage)
        {
            for (int m = 0; m < signatureImage.Size.Height && m < templateImage.Size.Height; m++)
            {
                for (int n = 0; n < signatureImage.Size.Width && n < templateImage.Size.Width; n++)
                {
                    // if a black pixel of the signature overlaps with a black pixel of the
                    // template, then the signature doesn't match with the template.
                    if (signatureImage[m, n] == BinaryImage.Black
                        && templateImage[m, n] == BinaryImage.Black)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}