namespace CarSales.Models
{
    /// <summary>
    /// Represents aggregated sales amounts for a car model.
    /// </summary>
    public class CarSummary
    {
        public string Model { get; init; } = string.Empty;
        public double TotalWithoutVat { get; init; }
        public double TotalWithVat { get; init; }
    }
}