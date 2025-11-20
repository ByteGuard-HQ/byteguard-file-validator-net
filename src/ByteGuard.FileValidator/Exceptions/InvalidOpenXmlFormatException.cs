using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file, which is expected to be an Open XML file, does not adhere
    /// to the expected internal Open XML format structure, or if the file contains macros.
    /// </summary>
    public class InvalidOpenXmlFormatException : Exception
    {
        public InvalidOpenXmlFormatException()
            : base("Invalid Open XML file.")
        {
        }

        public InvalidOpenXmlFormatException(string message) : base(message)
        {
        }

        public InvalidOpenXmlFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}