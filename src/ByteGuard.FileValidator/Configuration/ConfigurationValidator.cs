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

            ValidateZipValidationConfiguration(configuration);
        }

        /// <summary>
        /// Validate the ZIP validation options on the configuration object.
        /// </summary>
        /// <param name="configuration">File validator configuration object.</param>
        private static void ValidateZipValidationConfiguration(FileValidatorConfiguration configuration)
        {
            var zipConfig = configuration.ZipValidationConfiguration
                ?? throw new ArgumentNullException(
                    nameof(configuration.ZipValidationConfiguration),
                    $"{nameof(configuration.ZipValidationConfiguration)} cannot be null. Disable ZIP validation using 'Enabled' if unwanted.");

            if (zipConfig.Enabled)
            {
                if (zipConfig.MaxEntries.HasValue && zipConfig.MaxEntries.Value <= 0)
                {
                    throw new ArgumentException("MaxEntries on ZIP validation configuration is invalid. Either set a valid positive value or use 'null' for no limit.", nameof(zipConfig.MaxEntries));
                }

                if (zipConfig.TotalUncompressedSizeLimit.HasValue && zipConfig.TotalUncompressedSizeLimit.Value <= 0)
                {
                    throw new ArgumentException(
                        "TotalUncompressedSizeLimit on ZIP validation configuration is invalid. Either set a valid positive value or use 'null' for no limit.",
                        nameof(zipConfig.TotalUncompressedSizeLimit));
                }

                if (zipConfig.EntryUncompressedSizeLimit.HasValue && zipConfig.EntryUncompressedSizeLimit.Value <= 0)
                {
                    throw new ArgumentException(
                        "EntryUncompressedSizeLimit on ZIP validation configuration is invalid. Either set a valid positive value or use 'null' for no limit.",
                        nameof(zipConfig.EntryUncompressedSizeLimit));
                }

                // Ensure EntryUncompressedSizeLimit isn't greater than the TotalUncompressedSizeLimit if defined.
                if (zipConfig.EntryUncompressedSizeLimitEnabled && zipConfig.TotalUncompressedSizeLimitEnabled
                    && zipConfig.EntryUncompressedSizeLimit > zipConfig.TotalUncompressedSizeLimit)
                {
                    throw new ArgumentException(
                        "EntryUncompressedSizeLimit cannot exceed TotalUncompressedSizeLimit.",
                        nameof(zipConfig.EntryUncompressedSizeLimit));
                }

                if (zipConfig.CompressionRateLimit.HasValue &&
                    (double.IsNaN(zipConfig.CompressionRateLimit.Value) || double.IsInfinity(zipConfig.CompressionRateLimit.Value)))
                {
                    throw new ArgumentException(
                        "CompressionRateLimit must be a finite number. Either set a valid positive value or use 'null' for no limit.",
                        nameof(zipConfig.CompressionRateLimit));
                }

                if (zipConfig.CompressionRateLimit.HasValue && zipConfig.CompressionRateLimit.Value <= 0)
                {
                    throw new ArgumentException(
                        "CompressionRateLimit on ZIP validation configuration is invalid. Either set a valid positive value or use 'null' for no limit.",
                        nameof(zipConfig.CompressionRateLimit));
                }
            }
        }
    }
}
