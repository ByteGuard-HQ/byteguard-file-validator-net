using System;

namespace ByteGuard.FileValidator.Exceptions
{
    /// <summary>
    /// Exception type used specifically when a given file, which is expected to be an Open XML file, does not adhere
    /// to the expected internal Open XML format structure, or if the file contains macros.
    /// </summary>
    public class InvalidOpenXmlFormatException : Exception
    {
        /// <summary>
        /// Construct a new <see cref="InvalidOpenXmlFormatException"/> to indicate that the provided file does not adhere to the Microsoft Open XML format specification.
        /// </summary>
        public InvalidOpenXmlFormatException()
            : base("Invalid Open XML file.")
        {
        }

        /// <summary>
        /// Construct a new <see cref="InvalidOpenXmlFormatException"/> to indicate that the provided file does not adhere to the Microsoft Open XML format specification.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        public InvalidOpenXmlFormatException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct a new <see cref="InvalidOpenXmlFormatException"/> to indicate that the provided file does not adhere to the Microsoft Open XML format specification.
        /// </summary>
        /// <param name="message">Custom exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public InvalidOpenXmlFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
