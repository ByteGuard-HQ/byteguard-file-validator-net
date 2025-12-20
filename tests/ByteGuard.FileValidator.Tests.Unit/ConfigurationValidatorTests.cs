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

    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentNullException if the ZIP preflight options is null")]
    public void ThrowIfInvalid_NullZipPreflightConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = null!
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should not throw any exception if the ZIP preflight options is disabled though values are invalid")]
    public void ThrowIfInvalid_ZipPreflightNotEnabledWithIncorrectValues_ShouldPassValidation()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = new()
            {
                Enabled = false,
                MaxEntries = -25,
                TotalUncompressedSizeLimit = -25,
                EntryUncompressedSizeLimit = -10,
                CompressionRateLimit = double.PositiveInfinity
            }
        };

        // Act
        var exception = Record.Exception(() => new FileValidator(config));

        // Assert
        Assert.Null(exception);
    }

    [Theory(DisplayName = "ThrowIfInvalid should throw ArgumentException if MaxEntries is invalid")]
    [InlineData(0)]
    [InlineData(-25)]
    public void ThrowIfInvalid_MaxEntriesIsInvalid_ShouldThrowArgumentException(int value)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = new()
            {
                Enabled = true,
                MaxEntries = value,
                TotalUncompressedSizeLimit = -1,
                EntryUncompressedSizeLimit = -1,
                CompressionRateLimit = -1
            }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory(DisplayName = "ThrowIfInvalid should throw ArgumentException if TotalUncompressedSizeLimit is invalid")]
    [InlineData(0)]
    [InlineData(-25)]
    public void ThrowIfInvalid_TotalUncompressedSizeLimitIsInvalid_ShouldThrowArgumentException(long value)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = new()
            {
                Enabled = true,
                MaxEntries = -1,
                TotalUncompressedSizeLimit = value,
                EntryUncompressedSizeLimit = -1,
                CompressionRateLimit = -1
            }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory(DisplayName = "ThrowIfInvalid should throw ArgumentException if EntryUncompressedSizeLimit is invalid")]
    [InlineData(0)]
    [InlineData(-25)]
    public void ThrowIfInvalid_EntryUncompressedSizeLimitIsInvalid_ShouldThrowArgumentException(long value)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = new()
            {
                Enabled = true,
                MaxEntries = -1,
                TotalUncompressedSizeLimit = -1,
                EntryUncompressedSizeLimit = value,
                CompressionRateLimit = -1
            }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact(DisplayName = "ThrowIfInvalid should throw ArgumentException if EntryUncompressedSizeLimit is greater than TotalUncompressedSizeLimit")]
    public void ThrowIfInvalid_EntryCompressedSizeLimitIsGreaterThanTotalUncompressedSizeLimit_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = new()
            {
                Enabled = true,
                MaxEntries = -1,
                TotalUncompressedSizeLimit = 25,
                EntryUncompressedSizeLimit = 30,
                CompressionRateLimit = -1
            }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory(DisplayName = "ThrowIfInvalid should throw ArgumentException if CompressionRateLimit is invalid")]
    [InlineData(0)]
    [InlineData(-25)]
    [InlineData(double.PositiveInfinity)]
    public void ThrowIfInvalid_CompressionRateLimitIsInvalid_ShouldThrowArgumentException(double value)
    {
        // Arrange
        var config = new FileValidatorConfiguration
        {
            SupportedFileTypes = new() { ".jpg" },
            FileSizeLimit = 25,
            ZipValidationConfiguration = new()
            {
                Enabled = true,
                MaxEntries = -1,
                TotalUncompressedSizeLimit = -1,
                EntryUncompressedSizeLimit = -1,
                CompressionRateLimit = value
            }
        };

        // Act
        Action act = () => new FileValidator(config);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }
}
