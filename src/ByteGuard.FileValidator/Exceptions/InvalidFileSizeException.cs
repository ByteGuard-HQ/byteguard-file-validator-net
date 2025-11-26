using System;
using System.Runtime.Serialization;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file exceeds the configured file size limit. 
    /// </summary>
    public class InvalidFileSizeException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="InvalidFileSizeException"/> to indicate that the provided file does not adhere to the configured limit.
        /// </summary>
        public InvalidFileSizeException()
            : base("File size is larger than the configured file size limit.")
        {
        }

        /// <summary>
        /// Construct a new <see cref="InvalidFileSizeException"/> to indicate that the provided file does not adhere to the configured limit.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public InvalidFileSizeException(string message) : base(message)
        {
        }
    }
}
