using CarSales.Services;
using Xunit;

namespace CarSales.Tests;

public class CarSalesIntegrationTests
{
    [Fact]
    public void ProcessCars_ReturnsCorrectWeekendSummaries()
    {
        // Arrange
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "TestData",
            "cars_full.xml");

        Assert.True(File.Exists(filePath));

        // Act
        var cars = CarXmlReader.Load(filePath);
        var summaries = WeekendSalesCalculator.Calculate(cars);

        var results = summaries.ToDictionary(s => s.Model);

        // Assert
        Assert.Equal(13, cars.Count);
        Assert.Equal(3, results.Count);

        Assert.Equal(
            1_000_000d,
            results["Škoda Oktávia"].TotalWithoutVat);

        Assert.Equal(
            1_200_000d,
            results["Škoda Oktávia"].TotalWithVat,
            2);

        Assert.Equal(
            700_000d,
            results["Škoda Fabia"].TotalWithoutVat);

        Assert.Equal(
            840_000d,
            results["Škoda Fabia"].TotalWithVat,
            2);

        Assert.Equal(
            630_000d,
            results["Škoda Felicia"].TotalWithoutVat);

        Assert.Equal(
            751_800d,
            results["Škoda Felicia"].TotalWithVat,
            2);

        Assert.Equal(
            2_330_000d,
            summaries.Sum(s => s.TotalWithoutVat),
            2);

        Assert.Equal(
            2_791_800d,
            summaries.Sum(s => s.TotalWithVat),
            2);
    }
}