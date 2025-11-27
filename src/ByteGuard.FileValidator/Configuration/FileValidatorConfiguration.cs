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
        /// Maximum file size limit in string representation (e.g. "25MB", "2 GB", etc.).
        /// </summary>
        /// <remarks>
        /// Defines the file size limit of files. See <see cref="ByteSize"/> for conversion help.
        /// Will be ignored if <see cref="FileSizeLimit"/> is defined.
        /// Spacing (<c>"25 MB"</c> vs. <c>"25MB"</c>) is irrelevant.
        /// <para>Supported string representation are:
        /// <ul>
        /// <li><c>B</c>: Bytes</li>
        /// <li><c>KB</c>: Kilobytes</li>
        /// <li><c>MB</c>: Megabytes</li>
        /// <li><c>GB</c>: Gigabytes</li>
        /// </ul>
        /// </para>
        /// </remarks>
        public string FriendlyFileSizeLimit { get; set; } = default!;

        /// <summary>
        /// Whether to throw an exception if an unsupported/invalid file is encountered. Defaults to <c>true</c>.
        /// </summary>
        public bool ThrowExceptionOnInvalidFile { get; set; } = true;

        /// <summary>
        /// Optional antimalware scanner to use during file validation.
        /// </summary>
        public IAntimalwareScanner? AntimalwareScanner { get; set; } = null;
    }
}
