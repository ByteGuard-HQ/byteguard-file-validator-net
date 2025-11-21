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
