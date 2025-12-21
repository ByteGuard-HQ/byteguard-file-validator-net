# ByteGuard File Validator ![NuGet Version](https://img.shields.io/nuget/v/ByteGuard.FileValidator)

**ByteGuard.FileValidator** is a lightweight security-focused library for validating user-supplied files in .NET applications.  
It helps you enforce consistent file upload rules by checking:

- Allowed file extensions
- File size limits
- File signatures (magic numbers) to detect spoofed types
- ZIP cotnaienr safety and specification conformance for Office Open XML / Open Document Formats (`.docx`, `.xlsx`, `.pptx`, `.odt`)
- Malware scan result using a varity of scanners (_requires the addition of a specific ByteGuard.FileValidator scanner package_)

> ⚠️ **Important:** This package is one layer in a defense-in-depth strategy.  
> It does **not** replace endpoint protection, sandboxing, input validation, or other security controls.

## Features

- ✅ Validate files by **extension**
- ✅ Validate files by **size**
- ✅ Validate files by **signature (_magic-numbers_)**
- ✅ Validate files by **specification conformance** for archive-based formats (_Open XML and Open Document Formats_)
- ✅ Validate **ZIP container safety** for ZIP-based formats (_Open XML and Open Document Formats_) to protect against decompression bombs and suspicious paths
- ✅ **Ensure no malware** through a variety of antimalware scanners
- ✅ Validate using file path, `Stream`, or `byte[]`
- ✅ Configure which file types to support
- ✅ Configure whether to **throw exceptions** or simply return a boolean
- ✅ **Fluent configuration API** for easy setup

## Getting Started

### Installation

This package is published and installed via [NuGet](https://www.nuget.org/packages/ByteGuard.FileValidator).

Reference the package in your project:

```bash
dotnet add package ByteGuard.FileValidator
```

### Antimalware scanners

In order to use the antimalware scanning capabilities, ensure you have a ByteGuard.FileValidator antimalware package referenced as well. You can find the relevant scanner package on [NuGet](https://www.nuget.org/packages?q=ByteGuard.FileValidator.Scanner.&includeComputedFrameworks=true&prerel=true&sortby=relevance) under the namespace `ByteGuard.FileValidator.Scanner`.

## Usage

### Basic validation

```csharp
var configuration = new FileValidatorConfiguration
{
  SupportedFileTypes = [FileExtensions.Pdf, FileExtensions.Jpg, FileExtensions.Png],
  FileSizeLimit = ByteSize.MegaBytes(25),
  ThrowExceptionOnInvalidFile = false,
  ZipValidationConfiguration = new ZipValidationConfiguration
  {
    Enabled = true,
    MaxEntries = 10_000,
    TotalUncompressedSizeLimit = ByteSize.MegaBytes(512),
    EntryUncompressedSizeLimit = ByteSize.MegaBytes(128),
    CompresseionRateLimit = 200.0,
    RejectSuspiciousPaths = true
  }
};

// Without antimalware scanner
var fileValidator = new FileValidator(configuration);
var isValid = fileValidator.IsValidFile("example.pdf", fileStream);

// With antimalware
var antimalwareScanner = AntimalwareScannerImplementation();
var fileValidator = new FileValidator(configuration, antimalwareScanner);
var isValid = fileValidator.IsValidFile("example.pdf", fileStream);
```

### Using the fluent builder

```csharp
var configuration = new FileValidatorConfigurationBuilder()
  .AllowFileTypes(FileExtensions.Pdf, FileExtensions.Jpg, FileExtensions.Png)
  .SetFileSizeLimit(ByteSize.MegaBytes(25))
  .SetThrowExceptionOnInvalidFile(false)
  .ConfigureZipValidation(zipOptions =>
  {
    zipOptions.Enabled = true;
    zipOptions.MaxEntries = 10_000;
    zipOptions.TotalUncompressedSizeLimit = ByteSize.MegaBytes(512);
    zipOptions.EntryUncompressedSizeLimit = ByteSize.MegaBytes(128);
    zipOptions.CompressionRateLimit = 200.0;
    zipOptions.RejectSuspiciousPaths = true;
  })
  .Build();

var fileValidator = new FileValidator(configuration);
var isValid = fileValidator.IsValidFile("example.pdf", fileStream);
```

### Validating specific aspects

The `FileValidator` class provides methods to validate specific aspects of a file.

> ⚠️ It’s recommended to use `IsValidFile` for comprehensive validation.
>
> `IsValidFile` performs, in order:
>
> 1. Extension validation
> 2. File size validation
> 3. Signature (magic-number) validation
> 4. Optional Open XML / Open Document Format specification conformance validation (for supported types), including ZIP container safety
> 5. Optional antimalware scanning with a compatible scanning package

```csharp
bool isExtensionValid = fileValidator.IsValidFileType(fileName);
bool isFileSizeValid = fileValidator.HasValidSize(fileStream);
bool isSignatureValid = fileValidator.HasValidSignature(fileName, fileStream);
bool isOpenXmlValid = fileValidator.IsValidOpenXmlDocument(fileName, fileStream);
bool isOpenDocumentFormatValid = fileValidator.IsValidOpenDocumentFormat(fileName, fileStream);
bool isMalwareClean = fileValidator.IsMalwareClean(fileName, fileStream);
```

### Example

```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    using var stream = file.OpenReadStream();

    var antimalwareScanner = AntimalwareScannerImplementation();

    var configuration = new FileValidatorConfiguration
    {
        SupportedFileTypes = [FileExtensions.Pdf, FileExtensions.Docx],
        FileSizeLimit = ByteSize.MegaBytes(10),
        ThrowExceptionOnInvalidFile = false
    };

    var validator = new FileValidator(configuration, antimalwareScanner);

    if (!validator.IsValidFile(file.FileName, stream))
    {
        return BadRequest("Invalid or unsupported file.");
    }

    // Proceed with processing/saving...

    return Ok();
}
```

## Supported File Extensions

The following file types are supported by the `FileValidator`:

| Category      | Supported extensions                                               |
| ------------- | ------------------------------------------------------------------ |
| **Documents** | `.doc`, `.docx`, `.xls`, `.xlsx`, `.pptx`, `.odt` , `.pdf`, `.rtf` |
| **Images**    | `.jpg`, `.jpeg`, `.png,`, `.bmp`                                   |
| **Video**     | `.mov`, `.avi`, `.mp4`                                             |
| **Audio**     | `.m4a`, `.mp3`, `.wav`                                             |

### Validation coverage per type

`IsValidFile` always validates:

- File extension (_against `SupportedFileTypes`_)
- File size (_against `FileSizeLimit`_)
- File signature (_magic number_)
- Malware scan result (_if an antimalware scanner has been configured_)

For some formats, additional checks are performed:

- **Microsoft Office / Open Document Format** (`.docx`, `.xlsx`, `.pptx`, `.odt`):

  - Extension
  - File size
  - Signature
  - ZIP container safety
  - Specification conformance
  - Malware scan result

- **Other binary formats**:
  - Extension
  - File size
  - Signature
  - Malware scan result

## Configuration Options

The `FileValidatorConfiguration` supports:

| Setting                       | Required | Default     | Description                                                                                                                                 |
| ----------------------------- | -------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| `SupportedFileTypes`          | Yes      | N/A         | A list of allowed file extensions (e.g., `.pdf`, `.jpg`).<br>Use the predefined constants in `FileExtensions` for supported types.          |
| `FileSizeLimit`               | Yes      | N/A         | Maximum permitted size of files.<br>Use the static `ByteSize` class provided with this package, to simplify your limit.                     |
| `ThrowExceptionOnInvalidFile` | No       | `true`      | Whether to throw an exception on invalid files or return `false`.                                                                           |
| `ZipValidationConfiguration`  | Yes      | _See below_ | Specific configuration class to configure how ZIP validation is performed on ZIP-based file formats (_Open XML and Open Document Formats_). |

The nested `ZipValidationConfiguration` supports:

| Setting                      | Required | Default       | Description                                                                                                                      |
| ---------------------------- | -------- | ------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| `Enabled`                    | Yes      | `true`        | Whether ZIP validation is enabled.                                                                                               |
| `MaxEntries`                 | Yes      | `10000`       | The maximum allowed number of entries within the ZIP container.                                                                  |
| `TotalUncompressedSizeLimit` | Yes      | 512 MB        | The total uncompressed size limit of the entire ZIP container.                                                                   |
| `EntryUncompressedSizeLimit` | Yes      | 128 MB        | The maximum uncompressed size limit of individuel entries within the ZIP container.                                              |
| `CompressionRateLimit`       | Yes      | `200` (200:1) | The maximum allowed compression rate (compressed size / uncompressed size).                                                      |
| `RejectSuspiciousPaths`      | Yes      | `true`        | Whether files should be rejected if their full name contains suspicious paths (e.g. root paths, drive letters, path traversal.). |

### Exceptions

When `ThrowExceptionOnInvalidFile` is set to `true`, validation functions will throw one of the appropriate exceptions defined below. However, when `ThrowExceptionOnInvalidFile` is set to `false`, all validation functions will either return `true` or `false`.

| Exception type                       | Scenario                                                                                             |
| ------------------------------------ | ---------------------------------------------------------------------------------------------------- |
| `EmptyFileException`                 | Thrown when the file content is `null` or empty, indicating a file without any content.              |
| `UnsupportedFileException`           | Thrown when the file extension is not in the list of supported types.                                |
| `InvalidFileSizeException`           | Thrown when the file size exceeds the configured file size limit.                                    |
| `InvalidSignatureException`          | Thrown when the file's signature does not match the expected signature for its type.                 |
| `InvalidOpenXmlFormatException`      | Thrown when the internal structure of an Open XML file is invalid (`.docx`, `.xlsx`, `.pptx`, etc.). |
| `InvalidOpenDocumentFormatException` | Thrown when the specification conformance of an Open Document Format file is invalid (`.odt`, etc.). |
| `InvalidZipArchiveException`         | Thrown when the ZIP-baesd file format does not respect the ZIP validation rules.                     |
| `MalwareDetectedException`           | Thrown when the configured antimalware scanner detected malware in the file from a scan result.      |

## When to use this package

- ✅ Whenever you need **consistent file validation rules** across projects
- ✅ When handling user uploads in APIs or web applications
- ✅ When you want **defense-in-depth** against spoofed or malicious files

## License

_ByteGuard FileValidator is Copyright © ByteGuard Contributors - Provided under the MIT license._
