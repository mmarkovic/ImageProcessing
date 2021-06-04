namespace ImageProcessing
{
    public interface IAppConfig
    {
        /// <summary>
        /// The applied sampling rate for calculating the signatures.
        /// </summary>
        /// <remarks>
        /// The value should be in the range between 90 and 360.
        /// </remarks>
        int SignatureSamplingRate { get; }

        /// <summary>
        /// Determines if all processed images should be written on the disk.
        /// </summary>
        /// <remarks>
        /// This is useful, to check every processing steps for debugging reasons and to
        /// verify the applied algorithm. But if the application behaves as expected, it
        /// can produced undesired noise and should therefore be deactivated.
        /// </remarks>
        bool WriteProcessedImagesToDisk { get; }
    }
}