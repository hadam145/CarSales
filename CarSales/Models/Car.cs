namespace CarSales.Models
{
    /// <summary>
    /// Represents a vehicle sale loaded from an XML file.
    /// </summary>
    public class Car
    {
        public string Model { get; init; } = string.Empty;

        public DateTime Sale { get; init; }

        public double Price { get; init; }

        public double VatRate { get; init; }

    }
}
