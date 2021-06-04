namespace ImageProcessingLib
{
    using System.Drawing;

    /// <summary>
    /// Defines how a binary image should be colorized.
    /// </summary>
    public class BinaryImageColorSettings
    {
        public BinaryImageColorSettings(Color foreground, Color background)
        {
            this.Foreground = foreground;
            this.Background = background;
        }

        /// <summary>
        /// Gets the foreground color, which will be used for colorizing a binary image.
        /// </summary>
        public Color Foreground { get; }

        /// <summary>
        /// Gets the background color, which will be used for colorizing a binary image.
        /// </summary>
        public Color Background { get; }

        /// <summary>
        /// Gets the default color settings for binary images. This will draw black pixels on a white background.
        /// </summary>
        public static BinaryImageColorSettings Default
            => new(Color.Black, Color.White);

        /// <summary>
        /// Draws black pixels an a transparent background.
        /// </summary>
        public static BinaryImageColorSettings TransparentBackground
            => new(Color.Black, Color.Transparent);

        /// <summary>
        /// Gets a value determining if <see cref="Color.Transparent"/> is used either for
        /// <see cref="Foreground"/> or <see cref="Background"/>.
        /// </summary>
        public bool UsesTransparent
            => this.Foreground == Color.Transparent || this.Background == Color.Transparent;
    }
}