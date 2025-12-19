using DocumentFormat.OpenXml.Packaging;
using ByteGuard.FileValidator.Configuration;
using ByteGuard.FileValidator.Exceptions;
using ByteGuard.FileValidator.Models;
using ByteGuard.FileValidator.Validators;
using ByteGuard.FileValidator.Scanners;

namespace ByteGuard.FileValidator
{
    /// <summary>
    /// Validator for file types based on their header signatures.
    /// </summary>
    public class FileValidator
    {
        /// <summary>
        /// List of all supported valid file definitions, incl. their header signatures (magic numbers)
        /// and potentially their corresponding valid subtype signatures.
        /// </summary>
        internal static readonly List<FileDefinition> SupportedFileDefinitions = new List<FileDefinition>
        {
            new FileDefinition
            {
                FileType = FileExtensions.Jpeg,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF } // ÿØÿ
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Jpg,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF } // ÿØÿ
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Jpe,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF } // ÿØÿ
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Pdf,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D } // %PDF-
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Png,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } // ‰PNG␍␊␚␊
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Bmp,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x42, 0x4D } // BM
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Doc,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } // ÐÏ␑à¡±␚á
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Docx,
                // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .docx.
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Odt,
                // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .odt.
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Rtf,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 } // {\rtf1
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Xlsx,
                // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .xlsx.
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Xls,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } // ÐÏ␑à¡±␚á
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Pptx,
                // WARNING: This shares the same signature as .zip and could potentially allow for .zip disguised as .pptx.
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 } // PK␃␄
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.M4a,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x66, 0x74, 0x79, 0x70 } // ftyp
                },
                SignatureOffset = 4,
                HasSubtype = true,
                SubtypeOffset = 8,
                ValidSubtypeSignatures = new List<byte[]>
                {
                    new byte[] { 0x4D, 0x34, 0x41, 0x20 } // M4A_
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Mov,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x66, 0x74, 0x79, 0x70 } // ftyp
                },
                SignatureOffset = 4,
                HasSubtype = true,
                SubtypeOffset = 8,
                ValidSubtypeSignatures = new List<byte[]>
                {
                    new byte[] { 0x71, 0x74, 0x20, 0x20 } // qt__ (Quicktime)
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Avi,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF
                },
                HasSubtype = true,
                SubtypeOffset = 8,
                ValidSubtypeSignatures = new List<byte[]>
                {
                    new byte[] { 0x41, 0x56, 0x49, 0x20 } // AVI_
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Mp3,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0xFF, 0xFB }, // Without ID3 tag
                    new byte[] { 0xFF, 0xF2 }, // Without ID3 tag
                    new byte[] { 0xFF, 0xF3 }, // Without ID3 tag
                    new byte[] { 0x49, 0x44, 0x33 } // ID3
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Mp4,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x66, 0x74, 0x79, 0x70 } // ftyp
                },
                SignatureOffset = 4,
                HasSubtype = true,
                SubtypeOffset = 8,
                ValidSubtypeSignatures = new List<byte[]>
                {
                    new byte[] { 0x6D, 0x6D, 0x70, 0x34 }, // mmp4 (MP4)
                    new byte[] { 0x6D, 0x70, 0x34, 0x32 }, // mp42 (MP4 v2)
                    new byte[] { 0x69, 0x73, 0x6F, 0x6D }, // isom (ISO Base Media File)
                    new byte[] { 0x4D, 0x53, 0x4E, 0x56 } // MSNV (MPEG-4)
                }
            },
            new FileDefinition
            {
                FileType = FileExtensions.Wav,
                ValidSignatures = new List<byte[]>
                {
                    new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF
                },
                HasSubtype = true,
                SubtypeOffset = 8,
                ValidSubtypeSignatures = new List<byte[]>
                {
                    new byte[] { 0x57, 0x41, 0x56, 0x45 } // WAVE
                }
            }
        };

        /// <summary>
        /// Specific file extensions for files that are Open XML documents.
        /// These require extra care when validating, as Open XML files are ZIP-archives and can contain potentially harmful content.
        /// </summary>
        private static readonly List<string> OpenXmlFormats = new List<string>
        {
            FileExtensions.Docx,
            FileExtensions.Xlsx,
            FileExtensions.Pptx
        };

        /// <summary>
        /// Specific file extensions for files that are Open Document Format (ODF).
        /// These require extra care when validating, as ODF files are ZIP-archives and can contain potentially harmful content.
        /// </summary>
        private static readonly List<string> OpenDocumentFormats = new List<string>
        {
            FileExtensions.Odt
        };

        /// <summary>
        /// File validator configuration instance.
        /// </summary>
        private readonly FileValidatorConfiguration _configuration;

        /// <summary>
        /// Antimalware scanner instance.
        /// </summary>
        private readonly IAntimalwareScanner? _antimalwareScanner;

        /// <summary>
        /// Instantiate a new instance of the file validator.
        /// </summary>
        /// <param name="configuration">Validator configuration.</param>
        public FileValidator(FileValidatorConfiguration configuration)
        {
            ConfigurationValidator.ThrowIfInvalid(configuration);

            _configuration = configuration;
        }

        /// <summary>
        /// Instantiate a new instance of the file validator.
        /// </summary>
        /// <param name="configuration">Validator configuration.</param>
        /// <param name="antimalwareScanner">Antimalware scanner to use during file validation.</param>
        public FileValidator(FileValidatorConfiguration configuration, IAntimalwareScanner antimalwareScanner)
            : this(configuration)
        {
            if (antimalwareScanner == null)
            {
                throw new ArgumentNullException(nameof(antimalwareScanner), "Antimalware scanner cannot be null.");
            }

            _antimalwareScanner = antimalwareScanner;
        }

        /// <summary>
        /// Get all supported file types based on the current configuration.
        /// </summary>
        /// <returns>All supported file types as per the configuration.</returns>
        public List<string> GetSupportedFileTypes()
        {
            return _configuration.SupportedFileTypes;
        }

        /// <summary>
        /// Whether the given file is valid based on all parameters.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.jpg</c>).</param>
        /// <param name="stream">Stream content of the file.</param>
        /// <returns><c>true</c> if valid based on all parameters, <c>false</c> otherwise, unless <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</returns>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidSignatureException">Thrown if the file does not adhere to the expected file signature and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if the internal ZIP-archive structure does not adhere to the expected Open XML structure of the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidFile(string fileName, Stream stream)
        {
            // Validate file type.
            if (!IsValidFileType(fileName))
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new UnsupportedFileException();
                }

                return false;
            }

            // Validate file size.
            if (!HasValidSize(stream))
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new UnsupportedFileException();
                }

                return false;
            }

            // Validate file signature.
            if (!HasValidSignature(fileName, stream))
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidSignatureException();
                }

                return false;
            }

            // Validate Open XML conformance for specific file types.
            if (IsOpenXmlFormat(fileName) && !IsValidOpenXmlDocument(fileName, stream))
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidOpenXmlFormatException();
                }

                return false;
            }

            // Validate Open Document Format (ODF) for specific file types.
            if (IsOpenDocumentFormat(fileName) && !IsValidOpenDocumentFormat(fileName, stream))
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidOpenDocumentFormatException();
                }

                return false;
            }

            // Validate antimalware scan if configured.
            if (_antimalwareScanner != null)
            {
                var isClean = IsMalwareClean(fileName, stream);
                if (!isClean)
                {
                    if (_configuration.ThrowExceptionOnInvalidFile)
                    {
                        throw new MalwareDetectedException();
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Whether the given file is valid based on all parameters.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.jpg</c>).</param>
        /// <param name="content">Byte array content of the file.</param>
        /// <returns><c>true</c> if valid based on all parameters, <c>false</c> otherwise, unless <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</returns>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidSignatureException">Thrown if the file does not adhere to the expected file signature and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if the internal ZIP-archive structure does not adhere to the expected Open XML structure of the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidFile(string fileName, byte[] content)
        {
            using (var stream = new MemoryStream(content))
            {
                return IsValidFile(fileName, stream);
            }
        }

        /// <summary>
        /// Whether the given file is valid based on all parameters.
        /// </summary>
        /// <param name="filePath">Full path to the file including filename and extension (e.g. <c>C:\temp\my-file.jpg</c>).</param>
        /// <returns><c>true</c> if valid based on all parameters, <c>false</c> otherwise, unless <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="filePath"/> is null or whitespace.</exception>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidSignatureException">Thrown if the file does not adhere to the expected file signature and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if the internal ZIP-archive structure does not adhere to the expected Open XML structure of the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            using (var fileStream = File.OpenRead(filePath))
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);

                memoryStream.Position = 0;
                var content = memoryStream.ToArray();

                var fileName = Path.GetFileName(filePath);

                return IsValidFile(fileName, content);
            }
        }

        /// <summary>
        /// Whether the given filename has a valid and supported file type based on its extension.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not validate the file signature according to its file type. Only the file type
        /// (via its extension) is validated using this function.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.jpg</c>).</param>
        /// <returns><c>true</c> if supported file type, <c>false</c> otherwise, unless <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</returns>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var isSupported = _configuration.SupportedFileTypes.Contains(extension, StringComparer.InvariantCultureIgnoreCase) &&
                SupportedFileDefinitions.Any(fd => fd.FileType.Equals(extension, StringComparison.InvariantCultureIgnoreCase));

            if (!isSupported && _configuration.ThrowExceptionOnInvalidFile)
            {
                throw new UnsupportedFileException();
            }

            return isSupported;
        }

        /// <summary>
        /// Whether the given file has a valid signature based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering to the expected signature for its file type.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,Stream)"/>.</para>
        /// <para>File signatures (also known as "magic numbers") are specific sequences of bytes at the beginning of a file that indicate its format.
        /// Various file header signatures are sourced from <a href="https://en.wikipedia.org/wiki/List_of_file_signatures">Wikipedia</a>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.jpg</c>).</param>
        /// <param name="stream">Stream content of the file.</param>
        /// <returns><c>true</c> if the file signature matches one of the expected signatures for the file type, <c>false</c> otherwise, unless <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</returns>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidSignatureException">Thrown if the file does not adhere to the expected file signature and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the stream is not readable or seekable.</exception>
        public bool HasValidSignature(string fileName, Stream stream)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
            }

            var extension = Path.GetExtension(fileName).ToLower();

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Unable to deduct file type (extension) based on the file name.", nameof(fileName));
            }

            if (stream is null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream), "Stream cannot be null or empty when validating file signature.");
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is not readable.");
            }

            if (!stream.CanSeek)
            {
                throw new InvalidOperationException("Stream is not seekable.");
            }

            var fileDefinition = SupportedFileDefinitions.FirstOrDefault(fd =>
                fd.FileType.Equals(extension, StringComparison.InvariantCultureIgnoreCase));

            if (fileDefinition == null)
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new UnsupportedFileException();
                }

                return false;
            }

            // As PDF documents are somewhat special in terms of both signature validation,
            // we need to investigate these files further. The PdfValidator is made specifically
            // for this purpose.
            if (fileDefinition.FileType.Equals(FileExtensions.Pdf))
            {
                using (var pdfValidator = new PdfValidator(stream))
                {
                    var isValidPdf = pdfValidator.IsValidPdfSignature();
                    if (!isValidPdf && _configuration.ThrowExceptionOnInvalidFile)
                    {
                        throw new InvalidSignatureException();
                    }

                    return isValidPdf;
                }
            }

            // Calculate signature start and end index.
            var signatureEnd = fileDefinition.SignatureOffset + fileDefinition.ValidSignatures.Max(s => s.Length);

            // Check whether the content is valid according to the primary header signature length.
            if (stream.Length < signatureEnd)
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidSignatureException("File content is too short to contain a valid signature.");
                }

                return false;
            }

            // Check primary header signature.
            var signatureLength = fileDefinition.ValidSignatures.Max(s => s.Length);

            byte[] headerBytes = new byte[signatureLength];
            stream.Seek(fileDefinition.SignatureOffset, SeekOrigin.Begin);
#if NET8_0_OR_GREATER
            stream.ReadExactly(headerBytes, 0, signatureLength);
#else
            _ = stream.Read(headerBytes, 0, signatureLength);
#endif

            var result = fileDefinition.ValidSignatures.Any(signature => headerBytes.SequenceEqual(signature));

            // Might as well return early as the subtype check is irrelevant if the primary signature is invalid.
            if (!result)
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidSignatureException();
                }

                return false;
            }

            // Check subtype header signature.
            if (fileDefinition.HasSubtype)
            {
                var subtypeSignatureEnd = fileDefinition.SubtypeOffset + fileDefinition.ValidSubtypeSignatures.Max(s => s.Length);

                // Check whether the content is valid according to the primary header signature length.
                if (stream.Length < subtypeSignatureEnd)
                {
                    if (_configuration.ThrowExceptionOnInvalidFile)
                    {
                        throw new InvalidSignatureException("File content is too short to contain a valid subtype signature.");
                    }

                    return false;
                }

                // Check subtype header signature.
                var subtypeSignatureLength = fileDefinition.ValidSubtypeSignatures.Max(s => s.Length);

                var subtypeHeaderBytes = new byte[subtypeSignatureLength];
                stream.Seek(fileDefinition.SubtypeOffset, SeekOrigin.Begin);
#if NET8_0_OR_GREATER
                stream.ReadExactly(subtypeHeaderBytes, 0, subtypeSignatureLength);
#else
                _ = stream.Read(subtypeHeaderBytes, 0, subtypeSignatureLength);
#endif

                result = fileDefinition.ValidSubtypeSignatures.Any(signature => subtypeHeaderBytes.Take(signature.Length).SequenceEqual(signature));
            }

            if (!result && _configuration.ThrowExceptionOnInvalidFile)
            {
                throw new InvalidSignatureException();
            }

            return result;
        }

        /// <summary>
        /// Whether the given file has a valid signature based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering to the expected signature for its file type.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,byte[])"/>.</para>
        /// <para>File signatures (also known as "magic numbers") are specific sequences of bytes at the beginning of a file that indicate its format.
        /// Various file header signatures are sourced from <a href="https://en.wikipedia.org/wiki/List_of_file_signatures">Wikipedia</a>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.jpg</c>).</param>
        /// <param name="content">Byte array content of the file.</param>
        /// <returns><c>true</c> if the file signature matches one of the expected signatures for the file type, <c>false</c> otherwise, unless <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the file name is null, empty, or whitespace, or if the byte content is null.</exception>
        /// <exception cref="ArgumentException">Thrown if unable to deduct file type (extension) from the given file name.</exception>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidSignatureException">Thrown if the file does not adhere to the expected file signature and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool HasValidSignature(string fileName, byte[] content)
        {
            if (content is null || content.Length == 0)
            {
                throw new ArgumentNullException(nameof(content), "File content cannot be null or empty when validating file signature.");
            }

            using (var stream = new MemoryStream(content))
            {
                return HasValidSignature(fileName, stream);
            }
        }

        /// <summary>
        /// Whether the given file has a valid signature based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering to the expected signature for its file type.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/>.</para>
        /// <para>File signatures (also known as "magic numbers") are specific sequences of bytes at the beginning of a file that indicate its format.
        /// Various file header signatures are sourced from <a href="https://en.wikipedia.org/wiki/List_of_file_signatures">Wikipedia</a>.</para>
        /// </remarks>
        /// <param name="filePath">Full path to the file including filename and extension (e.g. <c>C:\temp\my-file.jpg</c>).</param>
        /// <returns><c>true</c> if the file signature matches one of the expected signatures for the file type, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="filePath"/> is null or whitespace.</exception>
        /// <exception cref="UnsupportedFileException">Thrown if the file type is not supported and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidSignatureException">Thrown if the file does not adhere to the expected file signature and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool HasValidSignature(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                var fileName = Path.GetFileName(filePath);

                return HasValidSignature(fileName, fileStream);
            }
        }

        /// <summary>
        /// Whether the given file has a size below the file size limit.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering to the expected signature for its file type.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,Stream)"/>.</para>
        /// </remarks>
        /// <param name="stream">Stream content of the file.</param>
        /// <returns><c>true</c> if the file size is below the file size limit, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidFileSizeException">>Thrown if the file size is greater than the configured file size limit and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled</exception>
        public bool HasValidSize(Stream stream)
        {
            var isBelowLimit = stream.Length <= _configuration.FileSizeLimit;

            if (_configuration.ThrowExceptionOnInvalidFile && !isBelowLimit)
            {
                throw new InvalidFileSizeException();
            }

            return isBelowLimit;
        }

        /// <summary>
        /// Whether the given file has a size below the file size limit.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering to the expected signature for its file type.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,byte[])"/>.</para>
        /// </remarks>
        /// <param name="content">Byte array content of the file.</param>
        /// <returns><c>true</c> if the file size is below the file size limit, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidFileSizeException">>Thrown if the file size is greater than the configured file size limit and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled</exception>
        public bool HasValidSize(byte[] content)
        {
            var isBelowLimit = content.Length <= _configuration.FileSizeLimit;

            if (_configuration.ThrowExceptionOnInvalidFile && !isBelowLimit)
            {
                throw new InvalidFileSizeException();
            }

            return isBelowLimit;
        }

        /// <summary>
        /// Whether the given file has a size below the file size limit.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering to the expected signature for its file type.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/>.</para>
        /// </remarks>
        /// <param name="filePath">Full path to the file including filename and extension (e.g. <c>C:\temp\my-file.jpg</c>)</param>
        /// <returns><c>true</c> if the file size is below the file size limit, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="filePath"/> is null or whitespace.</exception>
        /// <exception cref="InvalidFileSizeException">>Thrown if the file size is greater than the configured file size limit and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled</exception>
        public bool HasValidSize(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                return HasValidSize(fileStream);
            }
        }

        /// <summary>
        /// Whether the given Open XML document is valid based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering the Open XML format.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,Stream)"/>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.docx</c>).</param>
        /// <param name="stream">Stream content of the file.</param>
        /// <returns><c>true</c> if the file is a valid Open XML file, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if Open XML file is invalid based on the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the stream is not readable.</exception> 
        public bool IsValidOpenXmlDocument(string fileName, Stream stream)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Unable to deduct file type (extension) based on the file name.", nameof(fileName));
            }

            if (stream is null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream), "Stream cannot be null or empty when validating file signature.");
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is not readable.");
            }

            try
            {
                // If we are not expecting this file type to be an Open XML file, we can just return false.
                if (!IsOpenXmlFormat(fileName))
                {
                    if (_configuration.ThrowExceptionOnInvalidFile)
                    {
                        throw new InvalidOpenXmlFormatException("The provided file extension is not recognized as an Open XML document.");
                    }

                    return false;
                }

                bool isValid;
                switch (extension.ToLowerInvariant())
                {
                    case FileExtensions.Docx:
                        {
                            isValid = OpenXmlFormatValidator.IsValidWordDocument(stream);
                            break;
                        }
                    case FileExtensions.Xlsx:
                        {
                            isValid = OpenXmlFormatValidator.IsValidSpreadsheetDocument(stream);
                            break;
                        }
                    case FileExtensions.Pptx:
                        {
                            isValid = OpenXmlFormatValidator.IsValidPresentationDocument(stream);
                            break;
                        }
                    default:
                        throw new InvalidOpenXmlFormatException("The provided file extension is not recognized as an Open XML file.");
                }

                if (_configuration.ThrowExceptionOnInvalidFile && !isValid)
                {
                    throw new InvalidOpenXmlFormatException();
                }

                return isValid;
            }
            catch (InvalidDataException e)
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidOpenXmlFormatException("The provided file is not a valid Open XML file. See inner exception for details.", e);
                }

                return false;
            }
            catch (FileFormatException e)
            {
                // Thrown if the content is corrupt.
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidOpenXmlFormatException("File content appears to be corrupt. Se inner exception for details.", e);
                }

                return false;
            }
            catch (OpenXmlPackageException e)
            {
                // Thrown if the content is not valid Open XML.
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidOpenXmlFormatException("Content does not appear to be valid Open XML format. See inner exception for details.", e);
                }

                return false;
            }
            catch (InvalidOpenXmlFormatException)
            {
                // Exceptions throw from within the Open XML format validator.
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw;
                }

                return false;
            }
        }

        /// <summary>
        /// Whether the given Open XML document is valid based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering the Open XML format.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,byte[])"/>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.docx</c>).</param>
        /// <param name="content">Byte content of the file.</param>
        /// <returns><c>true</c> if the file is a valid Open XML file, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the file name is null, empty, or whitespace, or if the byte content is null.</exception>
        /// <exception cref="ArgumentException">Thrown if unable to deduct file type (extension) from the given file name.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if Open XML file is invalid based on the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidOpenXmlDocument(string fileName, byte[] content)
        {
            if (content is null || content.Length == 0)
            {
                throw new ArgumentNullException(nameof(content), "File content cannot be null or empty when validating Open XML structure.");
            }

            using (var memoryStream = new MemoryStream(content))
            {
                return IsValidOpenXmlDocument(fileName, memoryStream);
            }
        }

        /// <summary>
        /// Whether the given Open XML document is valid based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering the Open XML format.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/>.</para>
        /// </remarks>
        /// <param name="filePath">Full path to the file including filename and extension (e.g. <c>C:\temp\my-file.docx</c>).</param>
        /// <returns><c>true</c> if the file is a valid Open XML file, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="filePath"/> is null or whitespace.</exception>
        /// <exception cref="InvalidOpenXmlFormatException">Thrown if Open XML file is invalid based on the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidOpenXmlDocument(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                var fileName = Path.GetFileName(filePath);

                return IsValidOpenXmlDocument(fileName, fileStream);
            }
        }

        /// <summary>
        /// Whether the given Open Document Format (ODF) file is valid based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering the Open Document Format specification.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,Stream)"/>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="stream">Stream content of the file.</param>
        /// <returns><c>true</c> if the file is a valid Open Document Format (ODF) file, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOpenDocumentFormatException">Thrown if Open Document Format (ODF) file is invalid based on the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the stream is not readable or seekable.</exception>
        public bool IsValidOpenDocumentFormat(string fileName, Stream stream)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Unable to deduct file type (extension) based on the file name.", nameof(fileName));
            }

            if (stream is null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream), "Stream cannot be null or empty when validating file signature.");
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is not readable.");
            }

            try
            {
                // If we are not expecting this file type to be an Open XML file, we can just return false.
                if (!IsOpenDocumentFormat(fileName))
                {
                    if (_configuration.ThrowExceptionOnInvalidFile)
                    {
                        throw new InvalidOpenDocumentFormatException("The provided file extension is not recognized as an Open Document Format document.");
                    }

                    return false;
                }

                bool isValid;

                switch (extension.ToLowerInvariant())
                {
                    case FileExtensions.Odt:
                        {
                            isValid = OpenDocumentFormatValidator.IsValidOpenDocumentTextDocument(stream);
                            break;
                        }
                    default:
                        throw new InvalidOpenDocumentFormatException("The provided file extension is not recognized as an Open Document Format file.");
                }

                if (_configuration.ThrowExceptionOnInvalidFile && !isValid)
                {
                    throw new InvalidOpenDocumentFormatException();
                }

                return isValid;
            }
            catch (InvalidDataException e)
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new InvalidOpenDocumentFormatException("The provided file is not a valid Open Document Format file. See inner exception for details.", e);
                }

                return false;
            }
        }

        /// <summary>
        /// Whether the given Open Document Format (ODF) file is valid based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering the Open Document Format specification.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string,byte[])"/>.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="content">Byte content of the file.</param>
        /// <returns><c>true</c> if the file is a valid Open Document Format (ODF) file, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the file name is null, empty, or whitespace, or if the byte content is null.</exception>
        /// <exception cref="ArgumentException">Thrown if unable to deduct file type (extension) from the given file name.</exception>
        /// <exception cref="InvalidOpenDocumentFormatException">Thrown if Open Document Format (ODF) file is invalid based on the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidOpenDocumentFormat(string fileName, byte[] content)
        {
            if (content is null || content.Length == 0)
            {
                throw new ArgumentNullException(nameof(content), "File content cannot be null or empty when validating Open Document Format structure.");
            }

            using (var memoryStream = new MemoryStream(content))
            {
                return IsValidOpenDocumentFormat(fileName, content);
            }
        }

        /// <summary>
        /// Whether the given Open Document Format (ODF) file is valid based on its type.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is adhering the Open Document Format specification.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/>.</para>
        /// </remarks>
        /// <param name="filePath">Full path to the file including filename and extension (e.g. <c>C:\temp\my-file.odt</c>).</param>
        /// <returns><c>true</c> if the file is a valid Open Document Format (ODF) file, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="filePath"/> is null or whitespace.</exception>
        /// <exception cref="InvalidOpenDocumentFormatException">Thrown if Open Document Format (ODF) file is invalid based on the given file type and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        public bool IsValidOpenDocumentFormat(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                var fileName = Path.GetFileName(filePath);

                return IsValidOpenDocumentFormat(fileName, fileStream);
            }
        }

        /// <summary>
        /// Whether the given file is clean according to the configured antimalware scanner.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="content">Byte content of the file.</param>
        /// <returns><c>true</c> if the no malware was detected in the file from the configured antimalware scanner, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no antimalware scanner has been configured for the FileValidator.</exception>
        /// <exception cref="MalwareDetectedException">Thrown if malware was detected in the file and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="AntimalwareScannerException">Thrown if the configured antimalware scanner encountered an error while scanning the file for malware.</exception>
        public bool IsMalwareClean(string fileName, byte[] content)
        {
            if (_antimalwareScanner is null)
            {
                throw new InvalidOperationException("No antimalware scanner has been configured for the FileValidator.");
            }

            using (var memoryStream = new MemoryStream(content))
            {
                return IsMalwareClean(fileName, memoryStream);
            }
        }

        /// <summary>
        /// Whether the given file is clean according to the configured antimalware scanner.
        /// </summary>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.odt</c>).</param>
        /// <param name="stream">Stream content of the file.</param>
        /// <returns><c>true</c> if the no malware was detected in the file from the configured antimalware scanner, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no antimalware scanner has been configured for the FileValidator.</exception>
        /// <exception cref="MalwareDetectedException">Thrown if malware was detected in the file and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="AntimalwareScannerException">Thrown if the configured antimalware scanner encountered an error while scanning the file for malware.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the stream is not readable or seekable.</exception>
        public bool IsMalwareClean(string fileName, Stream stream)
        {
            if (_antimalwareScanner is null)
            {
                throw new InvalidOperationException("No antimalware scanner has been configured for the FileValidator.");
            }

            if (stream is null || stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(stream), "Stream cannot be null or empty when validating file signature.");
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is not readable.");
            }

            if (!stream.CanSeek)
            {
                throw new InvalidOperationException("Stream is not seekable.");
            }

            stream.Seek(0, SeekOrigin.Begin);

            bool isClean;
            try
            {
                isClean = _antimalwareScanner.IsClean(stream, fileName);
            }
            catch (Exception ex)
            {
                throw new AntimalwareScannerException(ex);
            }

            if (!isClean)
            {
                if (_configuration.ThrowExceptionOnInvalidFile)
                {
                    throw new MalwareDetectedException();
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Whether the given file is clean according to the configured antimalware scanner.
        /// </summary>
        /// <param name="filePath">Full path to the file including filename and extension (e.g. <c>C:\temp\my-file.odt</c>).</param>
        /// <returns><c>true</c> if the no malware was detected in the file from the configured antimalware scanner, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no antimalware scanner has been configured for the FileValidator.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="filePath"/> is null or whitespace.</exception>
        /// <exception cref="MalwareDetectedException">Thrown if malware was detected in the file and <see cref="FileValidatorConfiguration.ThrowExceptionOnInvalidFile"/> is enabled.</exception>
        /// <exception cref="AntimalwareScannerException">Thrown if the configured antimalware scanner encountered an error while scanning the file for malware.</exception>
        public bool IsMalwareClean(string filePath)
        {
            if (_antimalwareScanner is null)
            {
                throw new InvalidOperationException("No antimalware scanner has been configured for the FileValidator.");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                var fileName = Path.GetFileName(filePath);

                return IsValidOpenDocumentFormat(fileName, fileStream);
            }
        }

        /// <summary>
        /// Whether the given file type is expected to be an Open XML file.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is expected to be an Open XML file.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/> or any of the other possible overloads.</para>
        /// </remarks>
        /// <param name="fileName">File name including extension (e.g. <c>my-file.jdocx</c>).</param>
        /// <returns><c>true</c> if the given file type is expected to be an Open XML file according to <see cref="OpenXmlFormats"/>, <c>false</c> otherwise.</returns>
        public bool IsOpenXmlFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return OpenXmlFormats.Contains(extension.ToLowerInvariant());
        }

        /// <summary>
        /// Whether the given file type is expected to be an Open Document Format (ODF) file.
        /// </summary>
        /// <remarks>
        /// <para>WARNING: This does not check if the file type is supported according to the configuration of the FileValidator,
        /// but only validates if the provided file is expected to be an Open Document Format (ODF) file.</para>
        /// <para>To completely validate the file based on all parameters, please use <see cref="IsValidFile(string)"/> or any of the other possible overloads.</para>
        /// </remarks>
        /// <param name="fileName">File name including extensions (e.g. <c>my-file.odt</c>).</param>
        /// <returns><c>true</c> if the given file type is expected to be an Open Document Format (ODF) file according to <see cref="OpenDocumentFormats"/>, <c>false</c> otherwise.</returns>
        public bool IsOpenDocumentFormat(string fileName)
        {
            var extensions = Path.GetExtension(fileName);
            return OpenDocumentFormats.Contains(extensions.ToLowerInvariant());
        }
    }
}
