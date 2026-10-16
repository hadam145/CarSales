using CarSales.Models;
using CarSales.Services;
using Xunit;

namespace CarSales.Tests;

public class WeekendSalesCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsCorrectTotalsForWeekendSales()
    {
        // Arrange
        var cars = new List<Car>
        {
            new()
            {
                Model = "Škoda Felicia",
                Sale = new DateTime(2000, 12, 3),
                Price = 210_000,
                VatRate = 20
            },
            new()
            {
                Model = "Škoda Felicia",
                Sale = new DateTime(2000, 12, 2),
                Price = 210_000,
                VatRate = 19
            },
            new()
            {
                Model = "Škoda Felicia",
                Sale = new DateTime(2000, 12, 4),
                Price = 210_000,
                VatRate = 19
            }
        };

        // Act
        var result = WeekendSalesCalculator.Calculate(cars);

        // Assert
        var summary = Assert.Single(result);

        Assert.Equal("Škoda Felicia", summary.Model);
        Assert.Equal(420_000d, summary.TotalWithoutVat);
        Assert.Equal(501_900d, summary.TotalWithVat, 2);
    }

    [Fact]
    public void Calculate_ExcludesWeekdaySales()
    {
        // Arrange
        var cars = new List<Car>
        {
            new()
            {
                Model = "Škoda Fabia",
                Sale = new DateTime(2010, 12, 6),
                Price = 350_000,
                VatRate = 20
            }
        };

        // Act
        var result = WeekendSalesCalculator.Calculate(cars);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Calculate_GroupsSalesByModel()
    {
        // Arrange
        var cars = new List<Car>
        {
            new()
            {
                Model = "Škoda Fabia",
                Sale = new DateTime(2010, 12, 4),
                Price = 100,
                VatRate = 20
            },
            new()
            {
                Model = "Škoda Fabia",
                Sale = new DateTime(2010, 12, 5),
                Price = 200,
                VatRate = 19
            },
            new()
            {
                Model = "Škoda Oktávia",
                Sale = new DateTime(2010, 12, 4),
                Price = 300,
                VatRate = 20
            }
        };

        // Act
        var result = WeekendSalesCalculator.Calculate(cars);
        var summaries = result.ToDictionary(s => s.Model);

        // Assert
        Assert.Equal(2, summaries.Count);

        Assert.Equal(300d, summaries["Škoda Fabia"].TotalWithoutVat);
        Assert.Equal(358d, summaries["Škoda Fabia"].TotalWithVat, 2);

        Assert.Equal(300d, summaries["Škoda Oktávia"].TotalWithoutVat);
        Assert.Equal(360d, summaries["Škoda Oktávia"].TotalWithVat, 2);
    }

    [Fact]
    public void Calculate_ReturnsEmpty_WhenNoCarsAreProvided()
    {
        // Arrange
        var cars = new List<Car>();

        // Act
        var result = WeekendSalesCalculator.Calculate(cars);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Calculate_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => WeekendSalesCalculator.Calculate(null!));
    }

    [Fact]
    public void Calculate_ThrowsOverflowException_WhenTotalsOverflow()
    {
        // Arrange
        var cars = new List<Car>
        {
            new()
            {
                Model = "Škoda Fabia",
                Sale = new DateTime(2010, 12, 4),
                Price = double.MaxValue,
                VatRate = 0
            },
            new()
            {
                Model = "Škoda Fabia",
                Sale = new DateTime(2010, 12, 5),
                Price = double.MaxValue,
                VatRate = 0
            }
        };

        // Act & Assert
        Assert.Throws<OverflowException>(
            () => WeekendSalesCalculator.Calculate(cars));
    }
}