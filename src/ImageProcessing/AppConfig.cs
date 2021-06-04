namespace ImageProcessing
{
    using System.Configuration;

    /// <summary>
    /// Gets the configuration values from the application configuration file.
    /// </summary>
    internal class AppConfig : IAppConfig
    {
        /// <inheritdoc />
        public int SignatureSamplingRate
        {
            get
            {
                string? signatureSamplingRate = ConfigurationManager.AppSettings["SignatureSamplingRate"];
                return !string.IsNullOrWhiteSpace(signatureSamplingRate)
                    ? int.Parse(signatureSamplingRate)
                    : 180;
            }
        }

        /// <inheritdoc />
        public bool WriteProcessedImagesToDisk
        {
            get
            {
                string? writeProcessedImagesToDisk = ConfigurationManager.AppSettings["WriteProcessedImagesToDisk"];
                return !string.IsNullOrWhiteSpace(writeProcessedImagesToDisk)
                       && bool.Parse(writeProcessedImagesToDisk);
            }
        }
    }
}