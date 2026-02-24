using ByteGuard.FileValidator.Configuration.Rules;
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
        /// <param name="rules">Open XML specific validation rules.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if document type is not supported (macros, templates).</exception>
        internal static bool IsValidWordDocument(Stream stream, OpenXmlRules rules)
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

                // Base structure validation
                if (wordDocument.MainDocumentPart == null)
                {
                    throw new InvalidOpenXmlFormatException("Unable to retrieve main document part in document.");
                }
                
                if (wordDocument.MainDocumentPart.Document == null)
                {
                    throw new InvalidOpenXmlFormatException("Document does not adhere to required format.");
                }

                // Validate specification conformance.
                if (rules.PerformConformanceValidation)
                {
                    var validator = new OpenXmlValidator(rules.ConformanceVersion);
                    if (validator.Validate(wordDocument).Any())
                    {
                        return false;
                    }   
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given content stream is a valid spreadsheet (Excel).
        /// </summary>
        /// <param name="stream">Stream in question.</param>
        /// <param name="rules">Open XML specific validation rules.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if document type is not supported (macros, add-ins, templates).</exception>
        internal static bool IsValidSpreadsheetDocument(Stream stream, OpenXmlRules rules)
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

                // Base structure validation
                if (spreadsheetDocument.WorkbookPart == null)
                {
                    throw new InvalidOpenXmlFormatException("Unable to retrieve workbook part in spreadsheet.");
                }
                
                if (spreadsheetDocument.WorkbookPart.Workbook == null)
                {
                    throw new InvalidOpenXmlFormatException("Spreadsheet does not adhere to required format.");
                }

                if (!spreadsheetDocument.WorkbookPart.WorksheetParts.Any())
                {
                    throw new InvalidOpenXmlFormatException("Spreadsheet does not contain any worksheets.");
                }

                // Validate specification conformance.
                if (rules.PerformConformanceValidation)
                {
                    var validator = new OpenXmlValidator(rules.ConformanceVersion);
                    if (validator.Validate(spreadsheetDocument).Any())
                    {
                        return false;
                    }   
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given content stream is a valid presentation (PowerPoint).
        /// </summary>
        /// <param name="stream">Stream in question.</param>
        /// <param name="rules">Open XML specific validation rules.</param>
        /// <returns><c>true</c> if valid, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided stream is null.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if document type is not supported (macros, add-ins, templates).</exception>
        internal static bool IsValidPresentationDocument(Stream stream, OpenXmlRules rules)
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

                // Base structure validation
                if (presentationDocument.PresentationPart == null)
                {
                    throw new InvalidOpenXmlFormatException("Unable to retrieve presentation part in presentation.");
                }

                if (!presentationDocument.PresentationPart.SlideParts.Any())
                {
                    throw new InvalidOpenXmlFormatException("Presentation does not contain any slides.");
                }

                // Validate specification conformance.
                if (rules.PerformConformanceValidation)
                {
                    var validator = new OpenXmlValidator(rules.ConformanceVersion);
                    if (validator.Validate(presentationDocument).Any())
                    {
                        return false;
                    }   
                }
            }

            return true;
        }
    }
}
