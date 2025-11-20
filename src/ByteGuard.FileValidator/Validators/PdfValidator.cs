using System;
using System.IO;
using System.Linq;
using System.Text;
using ByteGuard.FileValidator.Exceptions;

namespace ByteGuard.FileValidator.Validators
{
    /// <summary>
    /// PDF specific validator.
    /// </summary>
    /// <remarks>
    /// Contains all logic specific to validation of PDF files.
    /// </remarks>
    internal static class PdfValidator
    {
        /// <summary>
        /// Length of the header in which the correct signature can appear.
        /// </summary>
        /// <remarks>
        /// As many PDF files exists and not all correspond to the standard, a valid PDF just has to contain
        /// the signature somewhere within the first 1024 bytes.
        /// </remarks>
        private const int SignatureCheckLength = 1024;

        /// <summary>
        /// Valid signature of a PDF file (<c>%PDF-</c>).
        /// </summary>
        private const string ValidSignature = "%PDF-";

        /// <summary>
        /// Whether the file signature is valid for the given PDF document.
        /// </summary>
        /// <remarks>
        /// Verifies that the file signature within the first 1024 bytes, contains the required %PDF- content.
        /// </remarks>
        /// <param name="content">Byte content.</param>
        /// <returns><c>true</c> if the signature is valid, <c>false</c> otherwise.</returns>
        /// <exception cref="EmptyFileException">Thrown if the provided stream is null or has no content.</exception>
        internal static bool IsValidPdfSignature(byte[] content)
        {
            if (content == null || content.Length == 0)
            {
                throw new EmptyFileException();
            }

            // Only read the necessary bytes into a new byte[].
            var data = content.Take(SignatureCheckLength).ToArray();

            // Look for the valid signature.
            var headerContent = Encoding.UTF8.GetString(data);
            var index = headerContent.IndexOf(ValidSignature, StringComparison.Ordinal);

            // If not found (we didn't find "%PDF-" anywhere in the first 1024 bytes) this is not a valid PDF.
            if (index < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Whether the file signature is valid for the given PDF document.
        /// </summary>
        /// <remarks>
        /// Verifies that the file signature within the first 1024 bytes, contains the required %PDF- content.
        /// </remarks>
        /// <param name="contentStream">Stream.</param>
        /// <returns><c>true</c> if the signature is valid, <c>false</c> otherwise.</returns>
        /// <exception cref="EmptyFileException">Thrown if the provided stream is null or has no content.</exception>
        internal static bool IsValidPdfSignature(Stream contentStream)
        {
            if (contentStream == null || contentStream.Length == 0)
            {
                throw new EmptyFileException();
            }

            contentStream.Seek(0, SeekOrigin.Begin);

            // Only read the necessary bytes into a new byte[].
            var data = new byte[SignatureCheckLength];
            _ = contentStream.Read(data, 0, SignatureCheckLength);

            return IsValidPdfSignature(data);
        }
    }
}
