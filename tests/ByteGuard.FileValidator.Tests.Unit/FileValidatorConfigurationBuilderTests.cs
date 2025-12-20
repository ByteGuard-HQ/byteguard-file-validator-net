using ByteGuard.FileValidator.Configuration;

namespace ByteGuard.FileValidator.Tests.Unit;

public class FileValidatorConfigurationBuilderTests
{
    [Fact(DisplayName = "AllowFileTypes adds allowed file types to the configuration")]
    public void AllowFileTypes_AddsAllowedFileTypesToConfiguration()
    {
        // Arrange
        var builder = new FileValidatorConfigurationBuilder();

        // Act
        builder.AllowFileTypes(".png", ".jpg");
        var config = builder.Build();

        // Assert
        Assert.Contains(".png", config.SupportedFileTypes);
        Assert.Contains(".jpg", config.SupportedFileTypes);
    }

    [Fact(DisplayName = "ThrowOnInvalidFiles sets the ThrowExceptionOnInvalidFile property")]
    public void ThrowOnInvalidFiles_SetsThrowExceptionOnInvalidFileProperty()
    {
        // Arrange
        var builder = new FileValidatorConfigurationBuilder();

        // Act
        builder
            .AllowFileTypes(".pdf") // Ensure there's at least one allowed file type
            .ThrowOnInvalidFiles(false);

        var config = builder.Build();

        // Assert
        Assert.False(config.ThrowExceptionOnInvalidFile);
    }

    [Fact(DisplayName = "DisableZipValidation should set Enabled to false")]
    public void DisableZipValidation_ShouldSetEnabledToFalse()
    {
        // Arrange
        var builder = new FileValidatorConfigurationBuilder();

        // Act
        builder.AllowFileTypes(".pdf")
            .DisableZipValidation();

        var config = builder.Build();

        // Assert
        Assert.False(config.ZipValidationConfiguration.Enabled);
    }

    [Fact(DisplayName = "ConfigureZipValidation should populate all properties values as provided")]
    public void ConfigureZipValidation_ShouldPopulateAllPropertiesValuesAsProvided()
    {
        // Arrange
        var builder = new FileValidatorConfigurationBuilder();

        var expected = new ZipValidationConfiguration()
        {
            Enabled = false,
            MaxEntries = 10,
            TotalUncompressedSizeLimit = 10,
            EntryUncompressedSizeLimit = 9,
            CompressionRateLimit = 20,
            RejectSuspiciousPaths = false
        };

        // Act
        builder.AllowFileTypes(".pdf")
            .ConfigureZipValidation(options =>
            {
                options.Enabled = expected.Enabled;
                options.MaxEntries = expected.MaxEntries;
                options.TotalUncompressedSizeLimit = expected.TotalUncompressedSizeLimit;
                options.EntryUncompressedSizeLimit = expected.EntryUncompressedSizeLimit;
                options.CompressionRateLimit = expected.CompressionRateLimit;
                options.RejectSuspiciousPaths = expected.RejectSuspiciousPaths;
            });

        var config = builder.Build();

        // Assert
        Assert.Equal(config.ZipValidationConfiguration.Enabled, expected.Enabled);
        Assert.Equal(config.ZipValidationConfiguration.MaxEntries, expected.MaxEntries);
        Assert.Equal(config.ZipValidationConfiguration.TotalUncompressedSizeLimit, expected.TotalUncompressedSizeLimit);
        Assert.Equal(config.ZipValidationConfiguration.EntryUncompressedSizeLimit, expected.EntryUncompressedSizeLimit);
        Assert.Equal(config.ZipValidationConfiguration.CompressionRateLimit, expected.CompressionRateLimit);
        Assert.Equal(config.ZipValidationConfiguration.RejectSuspiciousPaths, expected.RejectSuspiciousPaths);
    }

    [Fact(DisplayName = "Build throws exception when configuration is invalid")]
    public void Build_ThrowsException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = new FileValidatorConfigurationBuilder()
            .AllowFileTypes(".unsupported");

        // Act
        Action act = () => builder.Build();

        // Act & Assert
        Assert.ThrowsAny<Exception>(act);
    }
}
