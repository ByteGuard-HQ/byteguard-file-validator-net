using System.IO.Compression;

namespace ByteGuard.FileValidator.Validators
{
    /// <summary>
    /// Open Document Format (ODF) validator.
    /// </summary>
    /// <remarks>
    /// Common validation for Open Document Format (ODF) files such as .odt, .ods, .odp.
    /// <para>
    /// <em>For now the validation only checks for the presence of key files within the ODF ZIP archive structure.
    /// This should definitely be improved in the future to validate the actual content of these files to ensure they conform to ODF specifications.</em>
    /// </para>
    /// </remarks>
    internal static class OpenDocumentFormatValidator
    {
        /// <summary>
        /// Whether the given content stream is a valid ODF presentation (.odp) file.
        /// </summary>
        /// <param name="stream">Content stream.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <throws cref="ArgumentNullException">Thrown if the provided <paramref name="stream"/> is <c>null</c> or empty.</throws>
        internal static bool IsValidOpenDocumentPresentationDocument(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var mimetypeEntry = archive.GetEntry("mimetype");
                var contentXmlEntry = archive.GetEntry("content.xml");

                if (mimetypeEntry == null || contentXmlEntry == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given content stream is a valid ODF spreadsheet (.ods) file.
        /// </summary>
        /// <param name="stream">Content stream.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <throws cref="ArgumentNullException">Thrown if the provided <paramref name="stream"/> is <c>null</c> or empty.</throws>
        internal static bool IsValidOpenDocumentSpreadsheetDocument(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var mimetypeEntry = archive.GetEntry("mimetype");
                var contentXmlEntry = archive.GetEntry("content.xml");

                if (mimetypeEntry == null || contentXmlEntry == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given content stream is a valid ODF text (.odt) file.
        /// </summary>
        /// <param name="stream">Content stream.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <throws cref="ArgumentNullException">Thrown if the provided <paramref name="stream"/> is <c>null</c> or empty.</throws>
        internal static bool IsValidOpenDocumentTextDocument(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var mimetypeEntry = archive.GetEntry("mimetype");
                var contentXmlEntry = archive.GetEntry("content.xml");

                if (mimetypeEntry == null || contentXmlEntry == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
