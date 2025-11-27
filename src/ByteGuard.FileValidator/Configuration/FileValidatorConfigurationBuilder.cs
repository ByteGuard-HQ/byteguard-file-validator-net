using ByteGuard.FileValidator.Scanners;

namespace ByteGuard.FileValidator.Configuration
{
    /// <summary>
    /// File validator configurations fluent API builder.
    /// </summary>
    public class FileValidatorConfigurationBuilder
    {
        private readonly List<string> supportedFileTypes = new List<string>();
        private bool throwOnInvalidFiles = true;
        private long fileSizeLimit = ByteSize.MegaBytes(25);
        private IAntimalwareScanner? antimalwareScanner = null;

        /// <summary>
        /// Allow specific file types (extensions) to be validated.
        /// </summary>
        /// <param name="allowedFileTypes">File types to allow.</param>
        public FileValidatorConfigurationBuilder AllowFileTypes(params string[] allowedFileTypes)
        {
            if (allowedFileTypes == null) return this;

            supportedFileTypes.AddRange(allowedFileTypes);
            return this;
        }

        /// <summary>
        /// Set whether to throw an exception when an invalid file is encountered.
        /// </summary>
        /// <param name="shouldThrow">Whether to throw an exception when an invalid file is encountered.</param>
        public FileValidatorConfigurationBuilder ThrowOnInvalidFiles(bool shouldThrow = true)
        {
            throwOnInvalidFiles = shouldThrow;
            return this;
        }

        /// <summary>
        /// Set max file size limit.
        /// </summary>
        /// <param name="inFileSizeLimit">New max file size limit.</param>
        public FileValidatorConfigurationBuilder SetFileSizeLimit(long inFileSizeLimit)
        {
            fileSizeLimit = inFileSizeLimit;
            return this;
        }

        /// <summary>
        /// Add an antimalware scanner.
        /// </summary>
        /// <param name="scanner">Antimalware scanner to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided scanner is null.</exception>
        public FileValidatorConfigurationBuilder AddAntimalwareScanner(IAntimalwareScanner scanner)
        {
            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            antimalwareScanner = scanner;
            return this;
        }

        /// <summary>
        /// Build configuration.
        /// </summary>
        /// <returns>File validator configurations object.</returns>
        public FileValidatorConfiguration Build()
        {
            var configuration = new FileValidatorConfiguration
            {
                SupportedFileTypes = supportedFileTypes,
                ThrowExceptionOnInvalidFile = throwOnInvalidFiles,
                FileSizeLimit = fileSizeLimit,
                AntimalwareScanner = antimalwareScanner
            };

            ConfigurationValidator.ThrowIfInvalid(configuration);

            return configuration;
        }
    }
}
