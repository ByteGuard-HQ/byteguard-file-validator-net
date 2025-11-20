using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file does not contain any content.
    /// </summary>
    public class EmptyFileException : Exception
    {
        public EmptyFileException()
            : this("The provided file does not have any content.")
        {
        }

        public EmptyFileException(string message) : base(message)
        {
        }

        public EmptyFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}