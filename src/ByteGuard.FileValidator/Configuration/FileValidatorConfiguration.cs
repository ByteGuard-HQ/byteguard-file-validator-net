using ByteGuard.FileValidator.Scanners;

namespace ByteGuard.FileValidator.Configuration
{
    /// <summary>
    /// Configuration class for the file validator service.
    /// </summary>
    public class FileValidatorConfiguration
    {
        /// <summary>
        /// Supported file types.
        /// </summary>
        /// <remarks>
        /// Setup which file types are supported by the file validator.
        /// File types should be specified with their leading dot, e.g. <c>.jpg</c>, <c>.png</c>, <c>.pdf</c>, etc.
        /// Currently supported file types can be found in <see cref="FileExtensions"/>.
        /// </remarks>
        public List<string> SupportedFileTypes { get; set; } = new List<string>();

        /// <summary>
        /// Maximum file size limit in bytes.
        /// </summary>
        /// <remarks>
        /// Defines the file size limit of files. See <see cref="ByteSize"/> for conversion help.
        /// </remarks>
        public long FileSizeLimit { get; set; } = -1;

        /// <summary>
        /// Whether to throw an exception if an unsupported/invalid file is encountered. Defaults to <c>true</c>.
        /// </summary>
        public bool ThrowExceptionOnInvalidFile { get; set; } = true;

        /// <summary>
        /// ZIP preflight configuration.
        /// </summary>
        /// <remarks>
        /// These settings apply to internal ZIP preflight validation for ZIP-archive based formats such as .docx, .xlsx, .pptx, and .odt.
        /// </remarks>
        public ZipPreflightConfiguration ZipPreflightConfiguration { get; set; } = new();
    }

    /// <summary>
    /// Configuration class for the internal ZIP preflight validator.
    /// </summary>
    /// <remarks>
    /// These settings apply to internal ZIP preflight validation for ZIP-archive based formats such as .docx, .xlsx, .pptx, and .odt.
    /// </remarks>
    public class ZipPreflightConfiguration
    {
        /// <summary>
        /// Whether the ZIP preflight validation is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Max entries within a given ZIP-archive.
        /// </summary>
        /// <remarks>
        /// Defaults to 10.000. Use <c>-1</c> for no limit.
        /// </remarks>
        public int MaxEntries { get; set; } = 10_000;

        internal bool MaxEntriesEnabled => MaxEntries > 0;

        /// <summary>
        /// Max allowed total uncompressed size.
        /// </summary>
        /// <remarks>
        /// Defaults to 512MB. Use <c>-1</c> for no limit.
        /// </remarks>
        public long TotalUncompressedSizeLimit { get; set; } = ByteSize.MegaBytes(512);

        internal bool TotalUncompressedSizeLimitEnabled => TotalUncompressedSizeLimit > 0;

        /// <summary>
        /// Max allowed uncompressed size for each entry within the ZIP-archive.
        /// </summary>
        /// <remarks>
        /// Defaults to 128MB. Use <c>-1</c> for no limit.
        /// </remarks>
        public long EntryUncompressedSizeLimit { get; set; } = ByteSize.MegaBytes(128);

        internal bool EntryUncompressedSizeLimitEnabled => EntryUncompressedSizeLimit > 0;

        /// <summary>
        /// Max allowed compression rate.
        /// </summary>
        /// <remarks>
        /// Defaults to 200:1. Use <c>-1</c> for no limit.
        /// </remarks>
        public double CompressionRateLimit { get; set; } = 200.0; // 200:1

        internal bool CompressionRateLimitEnabled => CompressionRateLimit > 0;

        /// <summary>
        /// Whether to reject suspicious paths within the ZIP-archive.
        /// </summary>
        /// <remarks>
        /// Will handle the following paths as being suspicious:
        /// <ul>
        ///   <li><c>/</c></li>
        ///   <li><c>\\</c></li>
        ///   <li>Drive-letters (e.g. <c>C:</c> and <c>D:</c>)</li>
        ///   <li>Path traversal (e.g. <c>../</c>, <c>\\..</c>, <c>..</c>)</li>
        /// </ul>
        /// </remarks>
        public bool RejectSuspiciousPaths { get; set; } = true;
    }
}
