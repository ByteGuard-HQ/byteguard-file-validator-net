using ByteGuard.FileValidator.Configuration;
using DocumentFormat.OpenXml;

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
    
    [Fact(DisplayName = "ConfigureOdfValidationRules sets the correct values on the configuration object")]
    public void ConfigureOdfValidationRules_SetsCorrectValues()
    {
        // Arrange
        var builder = new FileValidatorConfigurationBuilder()
            .AllowFileTypes(".odt");
        
        // Act
        builder.ConfigureOdfValidationRules(config =>
        {
            config.RequireMimetype = false;
        });

        var config = builder.Build();
        
        // Assert
        Assert.False(config.FileTypeRules.OdfRules.RequireMimetype);
    }
    
    [Fact(DisplayName = "ConfigureOpenXmlValidationRules sets the correct values on the configuration object")]
    public void ConfigureOpenXmlValidationRules_SetsCorrectValues()
    {
        // Arrange
        var expectedConformanceVersion = FileFormatVersions.Office2016;
        var builder = new FileValidatorConfigurationBuilder()
            .AllowFileTypes(".docx");
        
        // Act
        builder.ConfigureOpenXmlValidationRules(config =>
        {
            config.PerformConformanceValidation = true;
            config.ConformanceVersion = expectedConformanceVersion;
        });

        var config = builder.Build();
        
        // Assert
        Assert.True(config.FileTypeRules.OdfRules.RequireMimetype);
        Assert.Equal(expectedConformanceVersion, config.FileTypeRules.OpenXmlRules.ConformanceVersion);
    }
}
