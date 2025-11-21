using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file, which is expected to be an Open Document Format (ODF) file,
    /// does not adhere to the expected internal ODF structure.
    /// /// </summary>
    public class InvalidOpenDocumentFormatException : Exception
    {
        public InvalidOpenDocumentFormatException()
            : base("Invalid Open Document Format file.")
        {
        }

        public InvalidOpenDocumentFormatException(string message) : base(message)
        {
        }

        public InvalidOpenDocumentFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
