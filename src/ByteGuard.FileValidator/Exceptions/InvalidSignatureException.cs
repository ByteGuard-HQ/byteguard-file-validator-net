using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file is invalid based on signature.
    /// </summary>
    public class InvalidSignatureException : Exception
    {
        public InvalidSignatureException()
            : base("File signature does not match the expected signature for the given file type.")
        {
        }

        public InvalidSignatureException(string message) : base(message)
        {
        }
    }
}