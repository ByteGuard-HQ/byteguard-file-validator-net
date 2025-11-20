using System;
using System.Runtime.Serialization;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file exceeds the configured file size limit. 
    /// </summary>
    public class InvalidFileSizeException : Exception
    {
        public InvalidFileSizeException()
            : base("File size is larger than the configured file size limit.")
        {
        }

        public InvalidFileSizeException(string message) : base(message)
        {
        }
    }
}