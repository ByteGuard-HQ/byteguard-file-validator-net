using ByteGuard.FileValidator.Configuration;
using ByteGuard.FileValidator.Exceptions;

namespace ByteGuard.FileValidator.Tests.Unit;

public class ConfigurationValidatorTests
{
    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentNullException when configuration is null")]
    public void ThrowIfInvalid_ConfigurationIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new FileValidator(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentException when SupportedFileTypes is null")]
    public void ThrowIfInvalid_SupportedFileTypesIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = null!
        };

        // Act
        Action act = () => new FileValidator(config);

        // Act & Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentException when SupportedFileTypes is empty")]
    public void ThrowIfInvalid_SupportedFileTypesIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new()
        };

        // Act
        Action act = () => new FileValidator(config);

        // Act & Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentException when a file type is invalid")]
    public void ThrowIfInvalid_FileTypeIsInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { "pdf", ".jpg" } // "pdf" is missing "." prefix.
        };

        // Act
        Action act = () => new FileValidator(config);

        // Act & Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should throw UnsupportedFileException when a file type is unsupported")]
    public void ThrowIfInvalid_FileTypeIsUnsupported_ShouldThrowUnsupportedFileException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".unsupported", ".jpg" }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Act & Assert
        Assert.Throws<UnsupportedFileException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentException if file size limit is less than or equal to zero")]
    public void ThrowIfInvalid_FileSizeLimitIsLessThanOrEqualToZero_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }
}
