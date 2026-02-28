namespace ByteGuard.FileValidator.Configuration.Rules;

/// <summary>
/// Validation rules for OpenDocument Format files.
/// </summary>
public class OdfRules
{
    /// <summary>
    /// Whether a valid mimetype file is required in the ODF package. Defaults to <c>true</c>.
    /// </summary>
    public bool RequireMimetype { get; set; } = true;
}
