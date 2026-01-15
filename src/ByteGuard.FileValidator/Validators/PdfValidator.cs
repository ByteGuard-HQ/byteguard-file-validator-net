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
    internal class PdfValidator : IDisposable
    {
        /// <summary>
        /// Inner stream of the file.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Whether the inner stream should be left open during dispose.
        /// </summary>
        private readonly bool _leaveOpen;

        /// <summary>
        /// Length of the header in which the correct signature can appear.
        /// </summary>
        /// <remarks>
        /// As many PDF files exists and not all correspond to the standard, a valid PDF just has to contain
        /// the signature somewhere within the first 1024 bytes.
        /// </remarks>
        private const int SignatureCheckLength = 1024;

        /// <summary>
        /// String representation of the valid signature of a PDF file (<c>%PDF-</c>).
        /// </summary>
        private const string ValidSignatureString = "%PDF-";

        /// <summary>
        /// Instantiate a new <see cref="PdfValidator"/> instance from a stream.
        /// </summary>
        /// <param name="stream">Content stream</param>
        /// <param name="leaveOpen">Whether the stream should be closed during dispose.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="stream"/> is <c>null</c> or empty.</exception>
        public PdfValidator(Stream stream, bool leaveOpen = true)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            _stream = stream;
            _leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Instantiate a new <see cref="PdfValidator"/> instance from a byte array.
        /// </summary>
        /// <param name="content">Byte array content.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="content"/> is <c>null</c> or empty.</exception>
        public PdfValidator(byte[] content)
            : this(new MemoryStream(content), leaveOpen: false)
        {
        }

        /// <summary>
        /// Whether the file signature is valid for the given PDF document.
        /// </summary>
        /// <remarks>
        /// Verifies that the file signature within the first 1024 bytes, contains the required %PDF- content.
        /// </remarks>
        /// <returns><c>true</c> if the signature is valid, <c>false</c> otherwise.</returns>
        /// <exception cref="EmptyFileException">Thrown if the provided stream is null or has no content.</exception>
        internal bool IsValidPdfSignature()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            // Only read the necessary bytes into a new byte[].
            var data = new byte[SignatureCheckLength];
            var length = _stream.Read(data, 0, SignatureCheckLength);

            // Look for the valid signature.
            var headerContent = Encoding.ASCII.GetString(data, 0, length);
            var isValid = headerContent.Contains(ValidSignatureString, StringComparison.Ordinal);

            // If not found (we didn't find %PDF- anywhere within the first 1024 bytes) this is not a valid PDF.
            return isValid;
        }

        public void Dispose()
        {
            if (!_leaveOpen)
            {
                _stream?.Dispose();
            }
        }
    }
}
