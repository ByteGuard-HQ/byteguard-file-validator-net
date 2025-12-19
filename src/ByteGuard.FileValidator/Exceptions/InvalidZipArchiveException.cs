namespace ByteGuard.FileValidator.Exceptions;

/// <summary>
/// Exception type used specifically when a given file is invalid based on the ZIP preflight validation.
/// </summary>
public class InvalidZipArchiveException : Exception
{
    /// <summary>
    /// Construct a new <see cref="InvalidZipArchiveException"/> to indicate that the internal
    /// ZIP-archive structure in the provided file is not valid based on the defined preflight validation configurations.
    /// </summary>
    public InvalidZipArchiveException()
        : base("ZIP-archive is invalid based on the defined preflight configurations.")
    {
    }

    /// <summary>
    /// Construct a new <see cref="InvalidZipArchiveException"/> to indicate that the internal
    /// ZIP-archive structure in the provided file is not valid based on the defined preflight validation configurations.
    /// </summary>
    /// <param name="message">Custom exception message.</param>
    public InvalidZipArchiveException(string message) : base(message)
    {
    }
}
