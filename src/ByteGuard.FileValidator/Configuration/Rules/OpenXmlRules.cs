using DocumentFormat.OpenXml;

namespace ByteGuard.FileValidator.Configuration.Rules;

/// <summary>
/// Validation rules for Open XML files.
/// </summary>
public class OpenXmlRules
{
    /// <summary>
    /// Whether to perform conformance validation. Defaults to <c>true</c>.
    /// </summary>
    public bool PerformConformanceValidation { get; set; } = true;

    /// <summary>
    /// Version to use for conformance validation if enabled. Defaults to <c>Office2007</c>.
    /// </summary>
    /// <remarks>
    /// See <see cref="FileFormatVersions"/> form the <c>DocumentFormat.OpenXml</c> NuGet package.
    /// </remarks>
    public FileFormatVersions ConformanceVersion { get; set; } = FileFormatVersions.Office2010;
}
