using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file is unsupported.
    /// </summary>
    public class UnsupportedFileException : Exception
    {

        public UnsupportedFileException()
            : base("File type is not supported.")
        {
        }

        public UnsupportedFileException(string message) : base(message)
        {
        }
    }
}