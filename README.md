
# ByteGuard File Validator

**ByteGuard.FileValidator** is a lightweight security-focused library for validating user-supplied files in .NET applications.  
It helps you enforce consistent file upload rules by checking:
-   Allowed file extensions
-   File size limits
-   File signatures (magic numbers) to detect spoofed types
-   Internal ZIP structure for Office Open XML / OpenDocument formats (`.docx`,  `.xlsx`,  `.pptx`,  `.odt`)

> ⚠️ **Important:** This library should be part of a **defense-in-depth** strategy.  
It does not replace antivirus scanning, sandboxing, or other security controls.

## Features

- ✅ Validate files by **extension**
- ✅ Validate files by **size**
- ✅ Validate files by **signature (_magic-numbers_)**
- ✅ Validate files by **internal ZIP structure** for archive-based formats (_Open XML and OpenDocument formats_)
- ✅ Validate using file path, `Stream`, or `byte[]`
- ✅ Configure which file types to support
- ✅ Configure whether to **throw exceptions** or simply return a boolean
- ✅ **Fluent configuration API** for easy setup

## Getting Started

### Installation
This package is published and installed via NuGet.

Reference the package in your project:
```bash
dotnet add package ByteGuard.FileValidator
```

## Usage

### Basic validation

```csharp
var configuration = new FileValidatorConfiguration
{
	SupportedFileTypes = [FileExtensions.Pdf, FileExtensions.Jpg, FileExtensions.Png],
	FileSizeLimit = ByteSize.MegaBytes(25),
	ThrowExceptionOnInvalidFile = false
};

var fileValidator = new FileValidator(configuration);
var isValid = fileValidator.IsValidFile("example.pdf", fileStream);
```

### Using the fluent builder

```csharp
var  configuration = new FileValidatorConfigurationBuilder()
	.AllowFileTypes(FileExtensions.Pdf, FileExtensions.Jpg, FileExtensions.Png)
	.SetFileSizeLimit(ByteSize.MegaBytes(25))
	.SetThrowExceptionOnInvalidFile(false)
	.Build();

var fileValidator = new FileValidator(configuration);
var isValid = fileValidator.IsValidFile("example.pdf", fileStream);
```

### Validating specific aspects

The `FileValidator` class provides methods to validate specific aspects of a file.

> ⚠️ It’s recommended to use `IsValidFile` for comprehensive validation.
> 
> `IsValidFile`  performs, in order:
> 1.  Extension validation
> 2.  File size validation
> 3.  Signature (magic-number) validation
> 4.  Optional Open XML / OpenDocument structure validation (for supported types)

```csharp
bool isExtensionValid = fileValidator.IsValidFileType(fileName);
bool isFileSizeValid = fileValidator.HasValidSize(fileStream);
bool isSignatureValid = fileValidator.HasValidSignature(fileName, fileStream);
bool isOpenXmlValid = fileValidator.IsValidOpenXmlDocument(fileName, fileStream);
```

### Example
```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    using var stream = file.OpenReadStream();

    var configuration = new FileValidatorConfiguration
    {
        SupportedFileTypes = [FileExtensions.Pdf, FileExtensions.Docx],
        FileSizeLimit = ByteSize.MegaBytes(10),
        ThrowExceptionOnInvalidFile = false
    };

    var validator = new FileValidator(configuration);

    if (!validator.IsValidFile(file.FileName, stream))
    {
        return BadRequest("Invalid or unsupported file.");
    }

    // Proceed with processing/saving...
    
    return Ok();
}
```

## Supported File Extensions
The following file extensions are supported by the `FileValidator`:
- `.jpeg`, `.jpg`
- `.pdf`
- `.png`
- `.bmp`
- `.doc`
- `.docx`
- `.odt`
- `.rtf`
- `.xls`
- `.xlsx`
- `.pptx`
- `.m4a`
- `.mov`
- `.avi`
- `.mp3`
- `.mp4`
- `.wav`

### Validation coverage per type

`IsValidFile` always validates:

- File extension (against `SupportedFileTypes`)
- File size (against `FileSizeLimit`)
- File signature (magic number)

For some formats, additional checks are performed:

- **Office Open XML / OpenDocument** (`.docx`, `.xlsx`, `.pptx`, `.odt`):  
  - Extension
  - File size
  - Signature
  - Internal ZIP structure (basic format sanity)

- **Other binary formats** (e.g. images, audio, video such as `.jpg`, `.png`, `.mp3`, `.mp4`):  
  - Extension
  - File size
  - Signature

## Configuration Options

The `FileValidatorConfiguration` supports:

| Setting | Required | Default | Description |
|--|--|--|--|
| `SupportedFileTypes` | Yes | N/A | A list of allowed file extensions (e.g., `.pdf`, `.jpg`).<br>Use the predefined constants in `FileExtensions` for supported types. |
| `FileSizeLimit` | Yes | N/A | Maximum permitted size of files.<br>Use the static  `ByteSize` class provided with this package, to simplify your limit. |
| `ThrowExceptionOnInvalidFile` | No | `true` | Whether to throw an exception on invalid files or return `false`. |

### Exceptions

When `ThrowExceptionOnInvalidFile` is set to `true`, validation functions will throw one of the appropriate exceptions defined below. However, when `ThrowExceptionOnInvalidFile` is set to `false`, all validation functions will either return `true` or `false`.

| Exception type | Scenario |
|--|--|
| `UnsupportedFileException` | Thrown when the file extension is not in the list of supported types. |
| `InvalidFileSizeException` | Thrown when the file size exceeds the configured file size limit. |
| `InvalidSignatureException` | Thrown when the file's signature does not match the expected signature for its type. |
| `InvalidOpenXmlFormatException` | Thrown when the internal structure of an Open XML file is invalid (`.docx`, `.xlsx`, `.pptx`, etc.). |

## When to use this package

- ✅ Whenever you need **consistent file validation rules** across projects
- ✅ When handling user uploads in APIs or web applications
- ✅ When you want **defense-in-depth** against spoofed or malicious files

## License
_ByteGuard FileValidator is copyright © ByteGuard Contributors - Provided under the MIT license._