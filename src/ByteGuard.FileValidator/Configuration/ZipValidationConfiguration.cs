namespace ByteGuard.FileValidator.Configuration
{
    /// <summary>
    /// Defines the scope of ZIP validation within the file validator.
    /// </summary>
    /// <remarks>
    /// <em>Internal for now as this is only used in preflight validation of ZIP-based file formats.
    /// This enum allows for future expansion with a complete ZIP file validation procedure.</em>
    /// </remarks>
    [Flags]
    internal enum ZipValidationScope
    {
        /// <summary>
        /// No ZIP validation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Validate ZIP-based formats (.docx, .xlsx, .odt, etc).
        /// </summary>
        ZipBasedFormats = 1,

        /// <summary>
        /// Validate ZIP files.
        /// </summary>
        ZipFiles = 2,

        /// <summary>
        /// Validate both ZIP based formats (.docx, .xlsx, .odt, etc.) and ZIP files.
        /// </summary>
        All = ZipBasedFormats | ZipFiles
    }

    /// <summary>
    /// Configuration class for the internal ZIP validator.
    /// </summary>
    public class ZipValidationConfiguration
    {
        /// <summary>
        /// Whether ZIP validation is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Scope of ZIP validation.
        /// </summary>
        /// <remarks>
        /// Defines the scope of ZIP validation. Defaults to <see cref="ZipValidationScope.All"/>.
        /// <para><em>Internal for now as this is only used in preflight validation of ZIP-based file formats.
        /// This enum allows for future expansion with a complete ZIP file validation procedure.</em></para>
        /// </remarks>
        internal ZipValidationScope Scope { get; set; } = ZipValidationScope.All;

        /// <summary>
        /// Max entries within a given ZIP-archive.
        /// </summary>
        /// <remarks>
        /// Defaults to 10.000. Use <c>null</c> for no limit.
        /// </remarks>
        public int? MaxEntries { get; set; } = 10_000;

        /// <summary>
        /// Whether <see cref="MaxEntries"/> is enabled based on its value.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> if <see cref="MaxEntries"/> is set to <c>null</c>.
        /// </remarks>
        internal bool MaxEntriesEnabled => MaxEntries.HasValue;

        /// <summary>
        /// Max allowed total uncompressed size.
        /// </summary>
        /// <remarks>
        /// Defaults to 512MB. Use <c>null</c> for no limit.
        /// </remarks>
        public long? TotalUncompressedSizeLimit { get; set; } = ByteSize.MegaBytes(512);

        /// <summary>
        /// Whether <see cref="TotalUncompressedSizeLimit"/> is enabled based on its value.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> if <see cref="TotalUncompressedSizeLimit"/> is set to <c>null</c>.
        /// </remarks>
        internal bool TotalUncompressedSizeLimitEnabled => TotalUncompressedSizeLimit.HasValue;

        /// <summary>
        /// Max allowed uncompressed size for each entry within the ZIP-archive.
        /// </summary>
        /// <remarks>
        /// Defaults to 128MB. Use <c>null</c> for no limit.
        /// </remarks>
        public long? EntryUncompressedSizeLimit { get; set; } = ByteSize.MegaBytes(128);

        /// <summary>
        /// Whether <see cref="EntryUncompressedSizeLimit"/> is enabled based on its value.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> if <see cref="EntryUncompressedSizeLimit"/> is set to <c>null</c>.
        /// </remarks>
        internal bool EntryUncompressedSizeLimitEnabled => EntryUncompressedSizeLimit.HasValue;

        /// <summary>
        /// Max allowed compression rate.
        /// </summary>
        /// <remarks>
        /// Defaults to 200:1. Use <c>null</c> for no limit.
        /// </remarks>
        public double? CompressionRateLimit { get; set; } = 200.0; // 200:1

        /// <summary>
        /// Whether <see cref="CompressionRateLimit"/> is enabled based on its value.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> if <see cref="CompressionRateLimit"/> is set to <c>null</c>.
        /// </remarks>
        internal bool CompressionRateLimitEnabled => CompressionRateLimit.HasValue;

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
