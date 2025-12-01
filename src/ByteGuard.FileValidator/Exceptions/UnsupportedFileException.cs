namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file is unsupported.
    /// </summary>
    public class UnsupportedFileException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="UnsupportedFileException"/> to indicate that the file is not supported as per the configured supported file extensions.
        /// </summary>
        public UnsupportedFileException()
            : base("File type is not supported.")
        {
        }

        /// <summary>
        /// Construct a new <see cref="UnsupportedFileException"/> to indicate that the file is not supported as per the configured supported file extensions.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public UnsupportedFileException(string message) : base(message)
        {
        }
    }
}
