using System.Globalization;

namespace ByteGuard.FileValidator.Tests.Unit;

public class ByteSizeTests
{
    [Theory(DisplayName = "KiloBytes(int) should return the correct calculated value based on input")]
    [InlineData(2, 2048)]
    [InlineData(5, 5120)]
    [InlineData(15, 15360)]
    [InlineData(25, 25600)]
    public void KiloBytes_Int_ShouldCorrectlyCalculateByteSize(int input, long expected)
    {
        // Act
        var actual = ByteSize.KiloBytes(input);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "KiloBytes(double) should return the correct calculated value based on input")]
    [InlineData(2.5, 2560)]
    [InlineData(5.3, 5428)]
    [InlineData(15.15, 15514)]
    [InlineData(25.75, 26368)]
    public void KiloBytes_Double_ShouldCorrectlyCalculateByteSize(double input, long expected)
    {
        // Act
        var actual = ByteSize.KiloBytes(input);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "MegaBytes(int) should return the correct calculated value based on input")]
    [InlineData(2, 2_097_152)]
    [InlineData(5, 5_242_880)]
    [InlineData(15, 15_728_640)]
    [InlineData(25, 26_214_400)]
    public void MegaBytes_Int_ShouldCorrectlyCalculateByteSize(int input, long expected)
    {
        // Act
        var actual = ByteSize.MegaBytes(input);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "MegaBytes(double) should return the correct calculated value based on input")]
    [InlineData(2.5, 2_621_440)]
    [InlineData(5.3, 5_557_453)]
    [InlineData(15.15, 15_885_927)]
    [InlineData(25.75, 27_000_832)]
    public void MegaBytes_Double_ShouldCorrectlyCalculateByteSize(double input, long expected)
    {
        // Act
        var actual = ByteSize.MegaBytes(input);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "GigaBytes(int) should return the correct calculated value based on input")]
    [InlineData(2, 2_147_483_648)]
    [InlineData(5, 5_368_709_120)]
    [InlineData(15, 16_106_127_360)]
    [InlineData(25, 26_843_545_600)]
    public void GigaBytes_Int_ShouldCorrectlyCalculateByteSize(int input, long expected)
    {
        // Act
        var actual = ByteSize.GigaBytes(input);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "GigaBytes(double) should return the correct calculated value based on input")]
    [InlineData(2.5, 2_684_354_560)]
    [InlineData(5.3, 5_690_831_668)]
    [InlineData(15.15, 16_267_188_634)]
    [InlineData(25.75, 27_648_851_968)]
    public void GigaBytes_Double_ShouldCorrectlyCalculateByteSize(double input, long expected)
    {
        // Act
        var actual = ByteSize.GigaBytes(input);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "Parse should return the correct calculated value based on input")]
    [InlineData("25MB", 26_214_400)]
    [InlineData("15 KB", 15_360)]
    [InlineData("15      B", 15)]
    [InlineData("    2 GB    ", 2_147_483_648)]
    [InlineData("5,3 GB", 5_690_831_668)]
    public void Parse_ShouldCorrectlyCalculateByteSize(string input, long expected)
    {
        // Act
        // We need to enforce da-DK culture as our build servers are not da-DK.
        var formatProvider = CultureInfo.GetCultureInfo("da-DK");
        var actual = ByteSize.Parse(input, formatProvider);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "Parse(IFormatProvider) should return the correct calculated value based on input")]
    [InlineData("25.5 MB", "en-US", 26_738_688)]
    [InlineData("2.500,5 KB", "da-DK", 2_560_512)]
    public void Parse_CustomFormatProvider_ShouldCorrectlyCalculateByteSize(string input, string cultureName, long expected)
    {
        // Arrange
        IFormatProvider formatProvider = CultureInfo.GetCultureInfo(cultureName);

        // Act
        var actual = ByteSize.Parse(input, formatProvider);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory(DisplayName = "Parse should throw ArgumentNullException when the provided value is null or whitespace")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("\t")]
    public void Parse_ValueIsNullOrWhiteSpace_ShouldThrowArgumentNullException(string input)
    {
        // Act
        Action act = () => ByteSize.Parse(input);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Theory(DisplayName = "Parse should throw FormatException when the provided value does not contain a size indicator/magnitude")]
    [InlineData("25")]
    [InlineData("13,45     ")]
    [InlineData("   17,2   ")]
    [InlineData("      23.760")]
    public void Parse_NoSizeIndicator_ShouldThrowFormatException(string input)
    {
        // Act
        Action act = () => ByteSize.Parse(input);

        // Assert
        Assert.Throws<FormatException>(act);
    }

    [Theory(DisplayName = "Parse should throw FormatException when the provided value does not contain a size")]
    [InlineData("asd")]
    [InlineData(",     ")]
    [InlineData("   B   ")]
    [InlineData("      MB")]
    public void Parse_NoSize_ShouldThrowFormatException(string input)
    {
        // Act
        Action act = () => ByteSize.Parse(input);

        // Assert
        Assert.Throws<FormatException>(act);
    }

    [Theory(DisplayName = "Parse should throw FormatException when the provided size indicator/magnitude is not supported")]
    [InlineData("25 TB")]
    [InlineData("  1 PB   ")]
    [InlineData("   5 RND   ")]
    [InlineData("      54.jpg")]
    public void Parse_InvalidSizeIndicator_ShouldThrowFormatException(string input)
    {
        // Act
        Action act = () => ByteSize.Parse(input);

        // Assert
        Assert.Throws<FormatException>(act);
    }
}