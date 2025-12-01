namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file does not contain any content.
    /// </summary>
    public class EmptyFileException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="EmptyFileException"/> to indicate that the provided file does not have any content.
        /// </summary>
        public EmptyFileException()
            : this("The provided file does not have any content.")
        {
        }

        /// <summary>
        /// Construct a new <see cref="EmptyFileException"/> to indicate that the provided file does not have any content.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public EmptyFileException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct a new <see cref="EmptyFileException"/> to indicate that the provided file does not have any content.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public EmptyFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
