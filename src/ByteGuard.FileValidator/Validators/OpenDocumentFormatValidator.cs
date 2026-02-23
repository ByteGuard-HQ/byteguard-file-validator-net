using System.IO.Compression;
using System.Text;
using System.Xml;
using ByteGuard.FileValidator.Configuration.Rules;

namespace ByteGuard.FileValidator.Validators
{
    /// <summary>
    /// Open Document Format (ODF) validator.
    /// </summary>
    /// <remarks>
    /// Common validation for Open Document Format (ODF) files such as .odt, .ods, .odp.
    /// </remarks>
    internal static class OpenDocumentFormatValidator
    {
        private const string ManifestEntryName = "META-INF/manifest.xml";
        private const string ContentEntryName = "content.xml";
        private const string MimetypeEntryName = "mimetype";

        private static Dictionary<string, string> OdfMimetypeMappings = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".odt", "application/vnd.oasis.opendocument.text" },
            { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
            { ".odp", "application/vnd.oasis.opendocument.presentation" },
        };

        /// <summary>
        /// Whether the given content stream is a valid ODF file.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="stream">Content stream.</param>
        /// <param name="rules">ODF validation rules.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <throws cref="ArgumentNullException">Thrown if the provided <paramref name="stream"/> is <c>null</c> or empty.</throws>
        internal static bool IsValidOpenDocumentFormatFile(string fileName, Stream stream, OdfValidationRules rules)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                return PerformOdfValidation(fileName, archive, rules);
            }
        }

        /// <summary>
        /// Perform ODF validation.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="archive">ODF ZIP archive.</param>
        /// <param name="rules">ODF validation rules.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        private static bool PerformOdfValidation(string fileName, ZipArchive archive, OdfValidationRules rules)
        {
            var fileExtension = Path.GetExtension(fileName);
            if (!OdfMimetypeMappings.ContainsKey(fileExtension))
            {
                // Unsupported ODF file extension
                return false;
            }

            var manifest = archive.GetEntry(ManifestEntryName);
            if (manifest is null)
            {
                // Must contain a manifest.xml entry
                return false;
            }

            if (!archive.Entries.Any(e => e.FullName.Equals(ContentEntryName, StringComparison.InvariantCultureIgnoreCase)))
            {
                // Must contain a content.xml entry
                return false;
            }

            // Validate mimetype entry
            if (!IsValidMimetype(fileName, archive, rules))
            {
                return false;
            }

            // Validate manifest if present
            if (!IsBasicValidManifest(manifest))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Whether the mimetype entry is valid.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="archive">ODF ZIP archive.</param>
        /// <param name="rules">ODF validation rules.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        private static bool IsValidMimetype(string fileName, ZipArchive archive, OdfValidationRules rules)
        {
            var mimetypeEntry = archive.GetEntry(MimetypeEntryName);
            if (mimetypeEntry is null)
            {
                if (rules.RequireMimetype)
                {
                    // Mimetype entry is required but missing
                    return false;
                }
            }
            else
            {
                if (archive.Entries.Count > 0 && !string.Equals(archive.Entries[0].FullName, MimetypeEntryName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // The mimetype entry must be the first entry in the ZIP archive
                    return false;
                }

                if (mimetypeEntry.Length != mimetypeEntry.CompressedLength)
                {
                    // The mimetype entry must not be compressed
                    return false;
                }

                // Read and validate mimetype value
                var mimetypeContent = Read(mimetypeEntry, 128);
                if (mimetypeContent is not null)
                {
                    var expectedMimetype = OdfMimetypeMappings[Path.GetExtension(fileName)];
                    var actual = mimetypeContent.Trim();
                    if (!string.Equals(actual, expectedMimetype, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the manifest entry is valid baased on basic checks.
        /// </summary>
        /// <param name="manifestEntry">Manifest entry to validate.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        private static bool IsBasicValidManifest(ZipArchiveEntry manifestEntry)
        {
            try
            {
                using var manifestStream = manifestEntry.Open();
                using var xmlReader = XmlReader.Create(manifestStream, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true,
                });

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    var fullPath = xmlReader.GetAttribute("full-path") ?? xmlReader.GetAttribute("manifest:full-path");
                    if (string.IsNullOrEmpty(fullPath))
                    {
                        continue;
                    }

                    fullPath = fullPath.Replace('\\', '/');

                    if (fullPath.StartsWith("/", StringComparison.Ordinal))
                    {
                        // OK: package root entry
                        continue;
                    }

                    // Prevent aboslute paths, path traversal, and scheme/drive letters
                    if (fullPath.StartsWith("../", StringComparison.Ordinal)
                        || fullPath.IndexOf("..\\", StringComparison.Ordinal) >= 0
                        || fullPath.IndexOf(":", StringComparison.Ordinal) >= 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                // Failed to parse manifest.xml
                return false;
            }
        }

        /// <summary>
        /// Read the content of the given ZIP archive entry as UTF-8 string up to the specified maximum number of bytes.
        /// </summary>
        /// <param name="entry">ZIP archive entry to read.</param>
        /// <param name="maxBytes">Maximum number of bytes to read.</param>
        /// <returns>Content of the ZIP archive entry as UTF-8 string, or null if reading failed or exceeded maxBytes.</returns>
        private static string? Read(ZipArchiveEntry entry, int maxBytes)
        {
            try
            {
                using var entryStream = entry.Open();
                using var memoryStream = new MemoryStream();
                var buffer = new byte[4096];

                int read;
                int total = 0;

                while ((read = entryStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    total += read;
                    if (total > maxBytes)
                    {
                        return null;
                    }

                    memoryStream.Write(buffer, 0, read);
                }

                return Encoding.ASCII.GetString(memoryStream.ToArray());
            }
            catch
            {
                return null;
            }
        }
    }
}
