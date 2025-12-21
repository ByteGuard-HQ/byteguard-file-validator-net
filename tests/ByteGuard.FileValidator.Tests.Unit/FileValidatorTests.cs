using System.Reflection;
using ByteGuard.FileValidator.Configuration;
using ByteGuard.FileValidator.Exceptions;
using ByteGuard.FileValidator.Scanners;
using ByteGuard.FileValidator.Tests.Unit.TestHelpers;
using NSubstitute;

namespace ByteGuard.FileValidator.Tests.Unit;

public class FileValidatorTests
{
    [Fact(DisplayName = "Constructor should throw ArgumentException when configuration is not provided")]
    public void Constructor_ShouldThrowArgumentNullException_WhenConfigurationIsNotProvided()
    {
        // Act
        Action act = () => new FileValidator(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Theory(DisplayName = "IsValidFileType should return true for supported file types")]
    [InlineData(new[] { ".pdf", ".jpg", ".mov" }, "some-file.pdf")]
    [InlineData(new[] { ".m4a", ".jpg", ".mp3" }, "test-file.jpg")]
    [InlineData(new[] { ".jpeg", ".jpg", ".png" }, "my-file.png")]
    public void IsValidFileType_SupportedFileTypes_ShouldReturnTrue(string[] supportedFileTypes, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new List<string>(supportedFileTypes),
            FileSizeLimit = ByteSize.MegaBytes(25)
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.IsValidFileType(fileName);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "IsValidFileType should throw UnsupportedFileException for unsupported file types if ThrowExceptionOnInvalidFile is true")]
    public void IsValidFileType_ThrowExceptionOnInvalidFileIsTrue_ShouldThrowUnsupportedFileExceptionForUnsupportedFileType()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.IsValidFileType("some-file.jpg");

        // Assert
        Assert.Throws<UnsupportedFileException>(act);
    }

    [Fact(DisplayName = "IsValidFileType should return false for unsupported file types if ThrowExceptionOnInvalidFile is false")]
    public void IsValidFileType_ThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalseForUnsupportedFileType()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.IsValidFileType("some-file.jpg");

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "HasValidSize(byte[]) should return true for file sizes below the file size limit")]
    public void HasValidSize_ValidSize_ShouldReturnTrue()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = 25,
            ThrowExceptionOnInvalidFile = false
        };
        var fileContent = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // PDF
        var fileValidator = new FileValidator(config);

        // Act
        var actual = fileValidator.HasValidSize(fileContent);

        // Assert
        Assert.True(actual);
    }

    [Fact(DisplayName = "HasValidSize(byte[]) should return false if the file size exceeds the limit and ThrowExceptionOnInvalidFile is false")]
    public void HasValidSize_InvalidSizeAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = 2,
            ThrowExceptionOnInvalidFile = false
        };
        var fileContent = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // PDF
        var fileValidator = new FileValidator(config);

        // Act
        var actual = fileValidator.HasValidSize(fileContent);

        // Assert
        Assert.False(actual);
    }

    [Fact(DisplayName = "HasValidSize(byte[]) should throw InvalidFileSizeException if the file size exceeds the limit and ThrowExceptionOnInvalidFile is true")]
    public void HasValidSize_InvalidSizeAndThrowExceptionOnInvalidFileIsTrue_ShouldThrowInvalidFileSizeException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = 2,
            ThrowExceptionOnInvalidFile = true
        };
        var fileContent = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // PDF
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.HasValidSize(fileContent);

        // Assert
        Assert.Throws<InvalidFileSizeException>(act);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should return true for files with valid signatures")]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, "test.pdf")] // PDF
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF }, "test.jpg")] // JPEG
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "test.png")] // PNG
    public void HasValidSignature_ValidSignatures_ShouldReturnTrue(byte[] fileBytes, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "HasValidSignature(byte[]) should throw UnsupportedFileException if the file type is unknown")]
    public void HasValidSignature_UnknownFileType_ShouldThrowUnsupportedFileException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf", ".jpg", ".png"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);
        var fileBytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        // Act
        Action act = () => fileValidator.HasValidSignature("unknown.xyz", fileBytes);

        // Assert
        Assert.Throws<UnsupportedFileException>(act);
    }

    [Fact(DisplayName = "HasValidSignature(byte[]) should return false if the file type is unknown and ThrowExceptionOnInvalidFile is false")]
    public void HasValidSignature_UnknownFileTypeAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf", ".jpg", ".png"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);
        var fileBytes = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        // Act
        var result = fileValidator.HasValidSignature("unknown.xyz", fileBytes);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should throw ArgumentNullException when fileBytes is null or empty")]
    [InlineData((byte[])null!)]
    [InlineData(new byte[] { })]
    public void HasValidSignature_FileBytesIsNull_ShouldThrowArgumentNullException(byte[] bytes)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.HasValidSignature("test.pdf", bytes);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should return false for files with invalid signatures if ThrowExceptionOnInvalidFile is false")]
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 }, ".pdf", "test.pdf")] // Invalid PDF
    [InlineData(new byte[] { 0x00, 0x01 }, ".jpg", "test.jpg")] // Invalid JPEG
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 }, ".png", "test.png")] // Invalid PNG
    public void HasValidSignature_InvalidSignatureAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse(byte[] fileBytes, string fileType, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [fileType],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should return false for files with invalid signatures if ThrowExceptionOnInvalidFile is false")]
    [InlineData(new byte[] { 0x00, 0x01 }, ".pdf", "test.pdf")] // Invalid PDF
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 }, ".jpg", "test.jpg")] // Invalid JPEG
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05 }, ".png", "test.png")] // Invalid PNG
    public void HasValidSignature_InvalidSignature_ShouldThrowInvalidSignatureException(byte[] fileBytes, string fileType, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [fileType],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.Throws<InvalidSignatureException>(act);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should return true for files with valid signatures")]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, ".pdf", "test.pdf")] // Valid PDF
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF }, ".jpg", "test.jpg")] // Valid JPEG
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, ".png", "test.png")] // Valid PNG
    public void HasValidSignature_ValidSignature_ShouldReturnTrue(byte[] fileBytes, string fileType, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [fileType],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should return false if invalid subtype signature is provided and ThrowExceptionOnInvalidFile is false")]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x00, 0x00, 0x00, 0x00 }, ".m4a", "test.m4a")] // Invalid M4A
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x00, 0x00, 0x00, 0x00 }, ".mov", "test.mov")] // Invalid MOV
    [InlineData(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, ".avi", "test.avi")] // Invalid AVI
    public void HasValidSignature_InvalidSubtypeSignature_ShouldThrowInvalidSignatureExcpetion(byte[] fileBytes, string fileType, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [fileType],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.Throws<InvalidSignatureException>(act);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should throw InvalidSignatureException if invalid subtype signature is provided")]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x00, 0x00, 0x00, 0x00 }, ".m4a", "test.m4a")] // Invalid M4A
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x00, 0x00, 0x00, 0x00 }, ".mov", "test.mov")] // Invalid MOV
    [InlineData(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, ".avi", "test.avi")] // Invalid AVI
    public void HasValidSignature_InvalidSubtypeSignatureAndThrowsExceptionOnInvalidFileIsFalse_ShouldReturnFalse(byte[] fileBytes, string fileType, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [fileType],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should return true if valid subtype signature is provided")]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20 }, ".m4a", "test.m4a")] // Invalid M4A
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20 }, ".mov", "test.mov")] // Invalid MOV
    [InlineData(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x41, 0x56, 0x49, 0x20 }, ".avi", "test.avi")] // Invalid AVI
    public void HasValidSignature_ValidSubtypeSignature_ShouldReturnTrue(byte[] fileBytes, string fileType, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [fileType],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should be case insensitive for file extensions")]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, "test.PDF")] // PDF
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF }, "test.JpG")] // JPEG
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "test.pNg")] // PNG
    public void HasValidSignature_FileExtensionCaseInsensitivity_ShouldReturnTrue(byte[] fileBytes, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf", ".jpg", ".png"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "HasValidSignature(byte[]) should throw ArgumentNullException when fileName is null or empty")]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("    ")]
    public void HasValidSignature_FileNameIsNullOrEmpty_ShouldThrowArgumentNullException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);
        var fileBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };

        // Act
        Action act = () => fileValidator.HasValidSignature(fileName, fileBytes);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact(DisplayName = "HasValidSignature(byte[]) should throw ArgumentNullException when fileName does not have an extension")]
    public void HasValidSignature_FileNameDoesNotHaveExtension_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);
        var fileBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };

        // Act
        Action act = () => fileValidator.HasValidSignature("filewithoutextension", fileBytes);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory(DisplayName = "IsOpenXmlFormat should return true for valid Open XML files")]
    [InlineData("test.docx")] // DOCX
    [InlineData("test.xlsx")] // XLSX
    [InlineData("test.pptx")] // PPTX
    public void IsOpenXmlFormat_ValidOpenXmlFiles_ShouldReturnTrue(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.IsOpenXmlFormat(fileName);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "IsOpenDocumentFormat should return true for valid ODF files")]
    [InlineData("test.odt")] // ODT
    public void IsOpenDocumentFormat_ValidOpenDocumentFiles_ShouldReturnTrue(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.IsOpenDocumentFormat(fileName);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "IsOpenXmlFormat(byte[]) should return false if the file type is not expected to be an Open XML file and ThrowExceptionOnInvalidFile is false")]
    [InlineData("test.pdf")]
    [InlineData("test.jpg")]
    [InlineData("test.png")]
    public void IsOpenXmlFormat_FileTypeNotExpectedToBeAnOpenXmlFileAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.IsOpenXmlFormat(fileName);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw InvalidOpenXmlFormatException if the file type is not expected to be an Open XML file")]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, "test.pdf")] // Valid PDF
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF }, "test.jpg")] // Valid JPEG
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "test.png")] // Valid PNG
    public void IsValidOpenXmlDocument_FileTypeNotExpectedToBeAnOpenXmlFile_ShouldThrowInvalidOpenXmlFormatException(byte[] fileBytes, string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument(fileName, fileBytes);

        // Assert
        Assert.Throws<InvalidOpenXmlFormatException>(act);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should return true for valid Open XML files")]
    [InlineData("DOCX_test.docx")]
    [InlineData("PPTX_test.pptx")]
    [InlineData("XLSX_test.xlsx")]
    public void IsValidOpenXmlDocument_ValidOpenXmlFiles_ShouldReturnTrue(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        var result = fileValidator.IsValidOpenXmlDocument(fileName, content);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should return false for invalid Open XML files if ThrowExceptionOnInvalidFile is false")]
    [InlineData("ZIP_test_fake_DOCX.docx")]
    [InlineData("ZIP_test_fake_PPTX.pptx")]
    [InlineData("ZIP_test_fake_XLSX.xlsx")]
    public void IsValidOpenXmlDocument_InvalidOpenXmlFilesAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        var result = fileValidator.IsValidOpenXmlDocument(fileName, content);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw InvalidOpenXmlFormatException for invalid Open XML files")]
    [InlineData("ZIP_test_fake_DOCX.docx")]
    [InlineData("ZIP_test_fake_PPTX.pptx")]
    [InlineData("ZIP_test_fake_XLSX.xlsx")]
    public void IsValidOpenXmlDocument_InvalidOpenXmlFiles_ShouldThrowInvalidOpenXmlFormatException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument(fileName, content);

        // Assert
        Assert.Throws<InvalidOpenXmlFormatException>(act);
    }

    [Theory(DisplayName = "IsValidOpenDocumentFormat(byte[]) should return true for valid ODF files")]
    [InlineData("ODT_test.odt")]
    public void IsValidOpenDocumentFormat_ValidOpenOpenDocumentFormatFiles_ShouldReturnTrue(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        var result = fileValidator.IsValidOpenDocumentFormat(fileName, content);

        // Assert
        Assert.True(result);
    }

    [Theory(DisplayName = "IsValidOpenDocumentFormat(byte[]) should return false for invalid ODF files if ThrowExceptionOnInvalidFile is false")]
    [InlineData("ZIP_test_fake_ODT.odt")]
    public void IsValidOpenDocumentFormat_InvalidOpenDocumentFormatFilesAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        var result = fileValidator.IsValidOpenDocumentFormat(fileName, content);

        // Assert
        Assert.False(result);
    }

    [Theory(DisplayName = "IsValidOpenDocumentFormat(byte[]) should throw InvalidOpenDocumentFormatException for invalid ODF files")]
    [InlineData("ZIP_test_fake_ODT.odt")]
    public void IsValidOpenDocumentFormat_InvalidOpenDocumentFormatFiles_ShouldThrowInvalidOpenDocumentFormatException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidOpenDocumentFormat(fileName, content);

        // Assert
        Assert.Throws<InvalidOpenDocumentFormatException>(act);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw InvalidOpenXmlFormatException for Open XML files containing macros")]
    [InlineData("DOCM_test_fake_DOCX.docx")]
    [InlineData("XLSM_test_fake_XLSX.xlsx")]
    [InlineData("PPTM_test_fake_PPTX.pptx")]
    public void IsValidOpenXmlDocument_FileContainsMacros_ShouldThrowInvalidOpenXmlFormatException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument(fileName, content);

        // Assert
        Assert.Throws<InvalidOpenXmlFormatException>(act);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw InvalidOpenXmlFormatException for Open XML template files")]
    [InlineData("DOTX_test_fake_DOCX.docx")]
    [InlineData("XLTX_test_fake_XLSX.xlsx")]
    [InlineData("POTX_test_fake_PPTX.pptx")]
    public void IsValidOpenXmlDocument_FileIsTemplate_ShouldThrowInvalidOpenXmlFormatException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument(fileName, content);

        // Assert
        Assert.Throws<InvalidOpenXmlFormatException>(act);
    }

    [Fact(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw InvalidOpenXmlFormatException if the provided file is not an Open XML file")]
    public void IsValidOpenXmlDocument_InvalidOpenXmlFile_ShouldThrowInvalidOpenXmlFormatException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".docx"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("ZIP_invalid_archive.docx"));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument("test.docx", content);

        // Assert
        Assert.Throws<InvalidOpenXmlFormatException>(act);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw ArgumentNullException when file name is null or empty")]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("    ")]
    public void IsValidOpenXmlDocument_FileNameIsNullOrEmpty_ShouldThrowArgumentNullException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".docx"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);
        var fileBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // ZIP file signature

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument(fileName, fileBytes);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Theory(DisplayName = "IsValidOpenXmlDocument(byte[]) should throw ArgumentNullException when fileBytes is null or empty")]
    [InlineData((byte[])null!)]
    [InlineData(new byte[] { })]
    public void IsValidOpenXmlDocument_FileBytesIsNullOrEmpty_ShouldThrowArgumentNullException(byte[] fileBytes)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".docx"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.IsValidOpenXmlDocument("test.docx", fileBytes);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Theory(DisplayName = "IsValidFile(string byte[]) should validate both file type and signature")]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, "valid.pdf", true)] // Valid PDF
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF }, "valid.jpg", true)] // Valid JPEG
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "valid.png", true)] // Valid PNG
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "invalid.xyz", false)] // Unknown file type
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 }, "invalid.pdf", false)] // Invalid PDF signature
    public void IsValidFile_ValidateBothFileTypeAndSignature(byte[] fileBytes, string fileName, bool expectedResult)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf", ".jpg", ".png"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Act
        var result = fileValidator.IsValidFile(fileName, fileBytes);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory(DisplayName = "IsValidFile(string, byte[]) should validate Open XML files")]
    [InlineData("DOCX_test.docx", true)] // Valid DOCX
    [InlineData("PPTX_test.pptx", true)] // Valid PPTX
    [InlineData("XLSX_test.xlsx", true)] // Valid XLSX
    [InlineData("ZIP_test_fake_DOCX.docx", false)] // Invalid DOCX
    [InlineData("ZIP_test_fake_PPTX.pptx", false)] // Invalid PPTX
    [InlineData("ZIP_test_fake_XLSX.xlsx", false)] // Invalid XLSX
    public void IsValidFile_ValidateOpenXmlFiles(string fileName, bool expectedResult)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        var result = fileValidator.IsValidFile(fileName, content);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory(DisplayName = "IsValidFile(string, byte[]) should validate Open XML files")]
    [InlineData("ODT_test.odt", true)] // Valid ODT
    [InlineData("ZIP_test_fake_ODT.odt", false)] // Invalid ODT
    public void IsValidFile_ValidateOpenDocumentFormatFiles(string fileName, bool expectedResult)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        var result = fileValidator.IsValidFile(fileName, content);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory(DisplayName = "IsValidFile(string, byte[]) should throw appropriate exceptions based on configuration")]
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03 }, "invalid.xyz", typeof(UnsupportedFileException))] // Unknown file type
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 }, "invalid.pdf", typeof(InvalidSignatureException))] // Invalid PDF signature
    public void IsValidFile_ThrowExceptionsBasedOnConfiguration(byte[] fileBytes, string fileName, Type expectedExceptionType)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf", ".jpg", ".png"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.IsValidFile(fileName, fileBytes);

        // Assert
        var exception = Assert.Throws(expectedExceptionType, act);
        Assert.NotNull(exception);
    }

    [Theory(DisplayName = "IsValidFile(string, byte[]) should throw InvalidOpenXmlFormatException for invalid Open XML files")]
    [InlineData("ZIP_test_fake_DOCX.docx")] // Invalid DOCX
    [InlineData("ZIP_test_fake_PPTX.pptx")] // Invalid PPTX
    [InlineData("ZIP_test_fake_XLSX.xlsx")] // Invalid XLSX
    public void IsValidFile_InvalidOpenXmlFiles_ShouldThrowInvalidOpenXmlFormatException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidFile(fileName, content);

        // Assert
        Assert.Throws<InvalidOpenXmlFormatException>(act);
    }

    [Theory(DisplayName = "IsValidFile(string, Stream) should throw InvalidZipArchiveException for entry names with suspicious paths")]
    [InlineData("../evil.txt")]
    [InlineData("..\\evil.txt")]
    [InlineData("/evil.txt")]
    [InlineData("\\evil.txt")]
    [InlineData("C:/evil")]
    [InlineData("C:\\evil")]
    public void IsValidFile_SuspiciousFilePaths_ShouldThrowInvalidZipArchiveException(string entryName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [FileExtensions.Docx],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        using var zipStream = ZipTestFactory.CreateZipWithEntry(entryName);

        // Act
        Action act = () => fileValidator.IsValidFile("evil.docx", zipStream);

        // Assert
        Assert.Throws<InvalidZipArchiveException>(act);
    }

    [Fact(DisplayName = "IsValidFile(string, Stream) should throw InvalidZipArchiveException when MaxEntries is exceeded")]
    public void IsValidFile_MaxEntriesExceeded_ShouldThrowInvalidZipArchiveException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [FileExtensions.Docx],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true,
            ZipValidationConfiguration = new()
            {
                MaxEntries = 3
            }
        };
        var fileValidator = new FileValidator(config);

        var entries = new Dictionary<string, string>()
        {
            { "file1.txt", "Content 1" },
            { "file2.txt", "Content 2" },
            { "file3.txt", "Content 3" },
            { "file4.txt", "Content 4" } // Exceeds MaxEntries
        };
        using var zipStream = ZipTestFactory.CreateZipWithEntries(entries);

        // Act
        Action act = () => fileValidator.IsValidFile("exceed.docx", zipStream);

        // Assert
        Assert.Throws<InvalidZipArchiveException>(act);
    }

    [Fact(DisplayName = "IsValidFile(string, Stream) should throw InvalidZipArchiveException when EntryUncompressedSizeLimit is exceeded")]
    public void IsValidFile_EntryUncompressedSizeLimitExceeded_ShouldThrowInvalidZipArchiveException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [FileExtensions.Docx],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true,
            ZipValidationConfiguration = new()
            {
                EntryUncompressedSizeLimit = 10 // 10 bytes
            }
        };
        var fileValidator = new FileValidator(config);

        using var zipStream = ZipTestFactory.CreateZipWithEntry("largefile.txt", new string('A', 20)); // 20 bytes, exceeds limit

        // Act
        Action act = () => fileValidator.IsValidFile("exceed.docx", zipStream);

        // Assert
        Assert.Throws<InvalidZipArchiveException>(act);
    }

    [Fact(DisplayName = "IsValidFile(string, Stream) should throw InvalidZipArchiveException when CompressionRateLimit is exceeded")]
    public void IsValidFile_CompressionRateLimitExceeded_ShouldThrowInvalidZipArchiveException()
    {
        // Arrange
        var zipBytes = ZipTestFactory.CreateZipWithHiglyCompressibleEntry(uncompressedBytes: 5 * 1024 * 1024); // 5 MB
        var actualRatio = ZipTestFactory.GetCompressionRate(zipBytes);

        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [FileExtensions.Docx],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true,
            ZipValidationConfiguration = new()
            {
                CompressionRateLimit = actualRatio - 0.1 // Set limit just below actual ratio to trigger exception
            }
        };
        var fileValidator = new FileValidator(config);

        using var zipStream = new MemoryStream(zipBytes);

        // Act
        Action act = () => fileValidator.IsValidFile("exceed.docx", zipStream);

        // Assert
        Assert.Throws<InvalidZipArchiveException>(act);
    }

    [Fact(DisplayName = "IsValidFile(string, Stream) should throw InvalidZipArchiveException when TotalUncompressedSizeLimit is exceeded")]
    public void IsValidFile_TotalUncompressedSizeLimitExceeded_ShouldThrowInvalidZipArchiveException()
    {
        // Arrange
        var zipBytes = ZipTestFactory.CreateZipWithFixedSizeEntries(entryCount: 3, bytesPerEntry: 10); // Total 30 bytes

        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [FileExtensions.Docx],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true,
            ZipValidationConfiguration = new()
            {
                RejectSuspiciousPaths = false,

                MaxEntries = -1,
                EntryUncompressedSizeLimit = -1,
                CompressionRateLimit = -1,

                TotalUncompressedSizeLimit = 29 // Set limit below total size to trigger exception
            }
        };
        var fileValidator = new FileValidator(config);

        using var zipStream = new MemoryStream(zipBytes);

        // Act
        Action act = () => fileValidator.IsValidFile("exceed.docx", zipStream);

        // Assert
        Assert.Throws<InvalidZipArchiveException>(act);
    }

    [Theory(DisplayName = "IsValidFile(string, byte[]) should throw InvalidOpenDocumentFormatException for invalid ODF files")]
    [InlineData("ZIP_test_fake_ODT.odt")] // Invalid ODT
    public void IsValidFile_InvalidOpenDocumentFormatFiles_ShouldThrowInvalidOpenDocumentFormatException(string fileName)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [Path.GetExtension(fileName)],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Load file from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new InvalidOperationException("Embedded resource not found.");

        using var reader = new StreamReader(stream);
        using var memoryStream = new MemoryStream();
        reader.BaseStream.CopyTo(memoryStream);

        var content = memoryStream.ToArray();

        // Act
        Action act = () => fileValidator.IsValidFile(fileName, content);

        // Assert
        Assert.Throws<InvalidOpenDocumentFormatException>(act);
    }

    [Theory(DisplayName = "IsValidFile(string, byte[]) should throw InvalidFileSizeException for files exceeding the file size limit")]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, 3)]
    [InlineData(new byte[] { 0x25, 0x50, 0x44 }, 2)]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x2D }, 5)]
    public void IsValidFile_InvalidFileSize_ShouldThrowInvalidFileSizeException(byte[] fileBytes, long fileSizeLimit)
    {
        // Arrange
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = fileSizeLimit,
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config);

        // Act
        Action act = () => fileValidator.IsValidFile("test.pdf", fileBytes);

        // Assert
        Assert.Throws<InvalidFileSizeException>(act);
    }

    [Fact(DisplayName = "IsValidFile(string, byte[]) should throw MalwareDetectedException when an antimalware scanner is registered and malware is detected and ThrowExceptionOnInvalidFile is true")]
    public void IsValidFile_AntimalwareScannerDetectsMalwareAndThrowExceptionOnInvalidFileIsTrue_ShouldThrowMalwareDetectedException()
    {
        // Arrange
        var mockAntimalwareScanner = Substitute.For<IAntimalwareScanner>();
        mockAntimalwareScanner.IsClean(Arg.Any<Stream>(), Arg.Any<string>()).Returns(false); // Simulate malware detection

        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = true
        };
        var fileValidator = new FileValidator(config, mockAntimalwareScanner);
        var fileBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // Valid PDF signature

        // Act
        Action act = () => fileValidator.IsValidFile("test.pdf", fileBytes);

        // Assert
        Assert.Throws<MalwareDetectedException>(act);
    }

    [Fact(DisplayName = "IsValidFile(string, byte[]) should return false when an antimalware scanner is registered and malware is detected and ThrowExceptionOnInvalidFile is false")]
    public void IsValidFile_AntimalwareScannerDetectsMalwareAndThrowExceptionOnInvalidFileIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var mockAntimalwareScanner = Substitute.For<IAntimalwareScanner>();
        mockAntimalwareScanner.IsClean(Arg.Any<Stream>(), Arg.Any<string>()).Returns(false); // Simulate malware detection

        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = [".pdf"],
            FileSizeLimit = ByteSize.MegaBytes(25),
            ThrowExceptionOnInvalidFile = false
        };
        var fileValidator = new FileValidator(config, mockAntimalwareScanner);
        var fileBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // Valid PDF signature

        // Act
        var actual = fileValidator.IsValidFile("test.pdf", fileBytes);

        // Assert
        Assert.False(actual);
    }
}
