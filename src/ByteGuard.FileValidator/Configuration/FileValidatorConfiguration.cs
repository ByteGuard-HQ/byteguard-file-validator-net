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
        /// Optional antimalware scanner to use during file validation.
        /// </summary>
        public IAntimalwareScanner? AntimalwareScanner { get; set; } = null;
    }
}
