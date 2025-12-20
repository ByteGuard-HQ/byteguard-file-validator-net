using System.IO.Compression;
using ByteGuard.FileValidator.Configuration;
using ByteGuard.FileValidator.Exceptions;

namespace ByteGuard.FileValidator.Validators;

internal static class ZipValidator
{
    /// <summary>
    /// Validate ZIP archive.
    /// </summary>
    /// <param name="zipStream">ZIP content stream.</param>
    /// <param name="options">ZIP validation configuration.</param>
    public static void Validate(Stream zipStream, ZipValidationConfiguration options)
    {
        // Only perform validation if enabled.
        if (!options.Enabled) return;

        if (zipStream is null || zipStream.Length == 0)
        {
            throw new ArgumentNullException(nameof(zipStream), "Stream cannot be null or empty when validating file signature.");
        }

        if (!zipStream.CanRead)
        {
            throw new InvalidOperationException("Stream is not readable.");
        }

        if (!zipStream.CanSeek)
        {
            throw new InvalidOperationException("Stream is not seekable.");
        }

        zipStream.Seek(0, SeekOrigin.Begin);

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true))
        {
            // Validat maximum number of entries.
            if (options.MaxEntriesEnabled && archive.Entries.Count > options.MaxEntries)
            {
                throw new InvalidZipArchiveException($"The total count of entries exceeds the defined maximum of {options.MaxEntries}");
            }

            long totalUncompressed = 0;
            foreach (var entry in archive.Entries)
            {
                // Validate suspicious paths.
                if (options.RejectSuspiciousPaths && IsSuspiciousZipPath(entry.FullName))
                {
                    throw new InvalidZipArchiveException($"Suspicious ZIP entry path: '{entry.FullName}'");
                }

                long uncompressed = entry.Length;
                long compressed = entry.CompressedLength;

                if (uncompressed < 0 || compressed < 0)
                {
                    throw new InvalidZipArchiveException($"'{entry.FullName}' has invalid size metadata.");
                }

                // Validate uncompressed size.
                if (options.EntryUncompressedSizeLimitEnabled && uncompressed > options.EntryUncompressedSizeLimit)
                {
                    throw new InvalidZipArchiveException($"'{entry.FullName}' too large uncompressed ({compressed} > {options.EntryUncompressedSizeLimit}).");
                }

                // Validate total uncompressed size.
                totalUncompressed += uncompressed;
                if (options.TotalUncompressedSizeLimitEnabled && totalUncompressed > options.TotalUncompressedSizeLimit)
                {
                    throw new InvalidZipArchiveException($"ZIP total uncompressed too large ({totalUncompressed} > {options.TotalUncompressedSizeLimit})");
                }

                // Validate compression rate.
                if (uncompressed > 0)
                {
                    if (compressed == 0)
                    {
                        throw new InvalidZipArchiveException($"Entry {entry.FullName} has 0 compressed bytes but {uncompressed} bytes.");
                    }

                    var ratio = (double)uncompressed / compressed;
                    if (options.CompressionRateLimitEnabled && ratio > options.CompressionRateLimit)
                    {
                        throw new InvalidZipArchiveException($"Entry {entry.FullName} compression rate to high ({ratio:0.0}:1 > {options.CompressionRateLimit:0.0}:1).");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check whether the given name of a ZIP-archive entry looks suspicious.
    /// </summary>
    /// <remarks>
    /// Checks root (e.g. <c>/</c>, <c>\\</c>), drive letters (e.g. <c>C:</c>, <c>D:</c>) and path traversal (e.g., <c>../</c>, <c>\\..</c>, <c>..</c>)
    /// </remarks>
    /// <param name="fullName">Entry fulle name.</param>
    /// <returns><c>true</c> if the name looks suspicious, <c>false</c> otherwise.</returns>
    private static bool IsSuspiciousZipPath(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return true;

        // Absolute paths.
        if (fullName.StartsWith("/", StringComparison.Ordinal) ||
            fullName.StartsWith("\\", StringComparison.Ordinal))
            return true;

        // Drive letters (Windows).
        if (fullName.Length >= 2 && char.IsLetter(fullName[0]) && fullName[1] == ':')
            return true;

        // Path traversal.
        var normalized = fullName.Replace('\\', '/');
        if (normalized.Contains("../", StringComparison.Ordinal) ||
            normalized.Contains("/..", StringComparison.Ordinal) ||
            normalized.StartsWith("..", StringComparison.Ordinal))
            return true;

        return false;
    }
}
