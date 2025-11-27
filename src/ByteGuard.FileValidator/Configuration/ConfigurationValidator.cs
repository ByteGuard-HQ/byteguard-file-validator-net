using ByteGuard.FileValidator.Exceptions;

namespace ByteGuard.FileValidator.Configuration
{
    /// <summary>
    /// Class used to validate the given configuration.
    /// </summary>
    public static class ConfigurationValidator
    {
        /// <summary>
        /// Validate configuration and throw exceptions if invalid.
        /// </summary>
        /// <param name="configuration">Configuration instance to validate.</param>
        /// <exception cref="ArgumentNullException">Throw if the configuration instance is null.</exception>
        /// <exception cref="ArgumentException">Thrown is no supported file types have been set, there's an error with any of the provided file types (missing "." prefix), or the file size limit is less than or equal to 0.</exception>
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
        }
    }
}
