namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file is invalid based on signature.
    /// </summary>
    public class InvalidSignatureException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="InvalidSignatureException"/> to indicate that the provided file signature does not match the file extension.
        /// </summary>
        public InvalidSignatureException()
            : base("File signature does not match the expected signature for the given file type.")
        {
        }

        /// <summary>
        /// Construct a new <see cref="InvalidSignatureException"/> to indicate that the provided file signature does not match the file extension.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public InvalidSignatureException(string message) : base(message)
        {
        }
    }
}
