using CarSales.Services;

namespace CarSales.Tests;

public class CarXmlReaderTests
{
    [Fact]
    public void Load_ReturnsCars_WhenXmlIsValid()
    {
        // Arrange
        var filePath = GetTestFilePath("cars_valid.xml");

        // Act
        var cars = CarXmlReader.Load(filePath);

        // Assert
        var car = Assert.Single(cars);
        Assert.Equal("Škoda Oktávia", car.Model);
    }

    [Theory]
    [InlineData("cars_empty.xml")]
    [InlineData("cars_empty_model.xml")]
    [InlineData("cars_invalid_date.xml")]
    [InlineData("cars_invalid_price.xml")]
    [InlineData("cars_invalid_root.xml")]
    [InlineData("cars_invalid_vat.xml")]
    [InlineData("cars_missing_price.xml")]
    [InlineData("cars_missing_sale.xml")]
    [InlineData("cars_negative_price.xml")]
    public void Load_ThrowsFormatException_WhenDataIsInvalid(string fileName)
    {
        // Arrange
        var filePath = GetTestFilePath(fileName);

        // Act & Assert
        Assert.Throws<FormatException>(
            () => CarXmlReader.Load(filePath));
    }

    [Fact]
    public void Load_ThrowsXmlException_WhenXmlIsMalformed()
    {
        // Arrange
        var filePath = GetTestFilePath("cars_malformed.xml");

        // Act & Assert
        Assert.Throws<System.Xml.XmlException>(
            () => CarXmlReader.Load(filePath));
    }

    private static string GetTestFilePath(string fileName)
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "TestData",
            fileName);
    }
}