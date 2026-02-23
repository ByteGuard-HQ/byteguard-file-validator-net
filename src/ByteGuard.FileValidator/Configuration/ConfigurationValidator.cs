using ByteGuard.FileValidator.Exceptions;

namespace ByteGuard.FileValidator.Configuration
{
    /// <summary>
    /// Class used to validate a given file validator configuration instance.
    /// </summary>
    public static class ConfigurationValidator
    {
        /// <summary>
        /// Validate configuration and throw exceptions if invalid.
        /// </summary>
        /// <param name="configuration">Configuration instance to validate.</param>
        /// <exception cref="ArgumentNullException">Throw if any required objects on the configuration object is <c>null</c>, or if the configuration object itself is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if any of the configuration values are invalid.</exception>
        /// <exception cref="UnsupportedFileException">Thrown if any of the provided supported file types are unsupported by the file validator.</exception>
        public static void ThrowIfInvalid(FileValidatorConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");
            }

            if (configuration.SupportedFileTypes == null || configuration.SupportedFileTypes.Count == 0)
            {
                throw new ArgumentException("At least one supported file type must be provided in the configuration.", nameof(FileValidatorConfiguration.SupportedFileTypes));
            }

            foreach (var fileType in configuration.SupportedFileTypes)
            {
                // Validate file type format.
                if (string.IsNullOrWhiteSpace(fileType) || !fileType.StartsWith("."))
                {
                    throw new ArgumentException($"Invalid file type '{fileType}'. File types must start with a dot (e.g., '.pdf', '.jpg', '.png').");
                }

                // Validate file type is supported by the current version of FileValidator.
                if (!FileValidator.SupportedFileDefinitions.Any(f => f.FileType.Equals(fileType)))
                {
                    throw new UnsupportedFileException($"File type '{fileType}' is not supported in the current version of FileValidator.");
                }
            }

            if (configuration.FileSizeLimit <= 0)
            {
                throw new ArgumentException("File size limit must be greater than zero.", nameof(configuration.FileSizeLimit));
            }

            ValidateOdfValidationConfiguration(configuration);
        }

        /// <summary>
        /// Validate the ODF validation options on the configuration object.
        /// </summary>
        /// <param name="configuration">File validator configuration object.</param>
        private static void ValidateOdfValidationConfiguration(FileValidatorConfiguration configuration)
        {
            if (configuration.FileTypeRules.OdfRules == null)
            {
                throw new ArgumentNullException(
                    nameof(configuration.FileTypeRules.OdfRules),
                    $"{nameof(configuration.FileTypeRules.OdfRules)} cannot be null.");
            }
        }
    }
}
