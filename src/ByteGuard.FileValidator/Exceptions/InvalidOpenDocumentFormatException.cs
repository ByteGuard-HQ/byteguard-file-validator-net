using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file, which is expected to be an Open Document Format (ODF) file,
    /// does not adhere to the expected internal ODF structure.
    /// /// </summary>
    public class InvalidOpenDocumentFormatException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="InvalidOpenDocumentFormatException"/> to indicate that the provided file does not adhere to the OpenDocument format specification.
        /// </summary>
        public InvalidOpenDocumentFormatException()
            : base("Invalid Open Document Format file.")
        {
        }

        /// <summary>
        /// Construct a new <see cref="InvalidOpenDocumentFormatException"/> to indicate that the provided file does not adhere to the OpenDocument format specification.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public InvalidOpenDocumentFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct a new <see cref="InvalidOpenDocumentFormatException"/> to indicate that the provided file does not adhere to the OpenDocument format specification.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidOpenDocumentFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
