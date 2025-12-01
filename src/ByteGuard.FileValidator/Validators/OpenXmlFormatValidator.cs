using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using ByteGuard.FileValidator.Exceptions;

namespace ByteGuard.FileValidator.Validators
{
    /// <summary>
    /// Open XML format validator.
    /// </summary>
    /// <remarks>
    /// Common validation for Microsoft Open XML format documents incl. Word, Excel, and PowerPoint.
    /// </remarks>
    internal static class OpenXmlFormatValidator
    {
        /// <summary>
        /// Whether the given content stream is a valid Word document.
        /// </summary>
        /// <param name="stream">Stream in question.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if document type is not supported (macros, templates).</exception>
        internal static bool IsValidWordDocument(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var wordDocument = WordprocessingDocument.Open(stream, isEditable: false))
            {
                // Macros are never supported because of their inherent security risks.
                if (wordDocument.DocumentType == WordprocessingDocumentType.MacroEnabledDocument)
                {
                    throw new InvalidOpenXmlFormatException("Document contains macros.");
                }

                // Templates are not supported.
                if (wordDocument.DocumentType == WordprocessingDocumentType.MacroEnabledTemplate ||
                    wordDocument.DocumentType == WordprocessingDocumentType.Template)
                {
                    throw new InvalidOpenXmlFormatException("Document is a template.");
                }

                // Validate structure.
                var validator = new OpenXmlValidator();
                if (validator.Validate(wordDocument).Any())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given content stream is a valid spreadsheet (Excel).
        /// </summary>
        /// <param name="stream">Stream in question.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if document type is not supported (macros, add-ins, templates).</exception>
        internal static bool IsValidSpreadsheetDocument(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, isEditable: false))
            {
                // Macros are never supported because of their inherent security risks.
                if (spreadsheetDocument.DocumentType == SpreadsheetDocumentType.MacroEnabledWorkbook)
                {
                    throw new InvalidOpenXmlFormatException("Spreadsheet contains macros.");
                }

                // Add-ins are not supported.
                if (spreadsheetDocument.DocumentType == SpreadsheetDocumentType.AddIn)
                {
                    throw new InvalidOpenXmlFormatException("Add-ins are not supported.");
                }

                // Templates are not supported.
                if (spreadsheetDocument.DocumentType == SpreadsheetDocumentType.MacroEnabledTemplate ||
                    spreadsheetDocument.DocumentType == SpreadsheetDocumentType.Template)
                {
                    throw new InvalidOpenXmlFormatException("Spreadsheet is a template.");
                }

                // Validate structure.
                var validator = new OpenXmlValidator();
                if (validator.Validate(spreadsheetDocument).Any())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given content stream is a valid presentation (PowerPoint).
        /// </summary>
        /// <param name="stream">Stream in question.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if document type is not supported (macros, add-ins, templates).</exception>
        internal static bool IsValidPresentationDocument(Stream stream)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var presentationDocument = PresentationDocument.Open(stream, isEditable: false))
            {
                // Macros are never supported because of their inherent security risks.
                if (presentationDocument.DocumentType == PresentationDocumentType.MacroEnabledPresentation ||
                    presentationDocument.DocumentType == PresentationDocumentType.MacroEnabledSlideshow)
                {
                    throw new InvalidOpenXmlFormatException("Presentation contains macros.");
                }

                // Add-ins are not supported.
                if (presentationDocument.DocumentType == PresentationDocumentType.AddIn)
                {
                    throw new InvalidOpenXmlFormatException("Add-ins are not supported.");
                }

                // Templates are not supported.
                if (presentationDocument.DocumentType == PresentationDocumentType.MacroEnabledTemplate ||
                    presentationDocument.DocumentType == PresentationDocumentType.Template)
                {
                    throw new InvalidOpenXmlFormatException("Presentation is a template.");
                }

                // Validate structure.
                var validator = new OpenXmlValidator();
                if (validator.Validate(presentationDocument).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
