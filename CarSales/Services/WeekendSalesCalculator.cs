using CarSales.Models;

namespace CarSales.Services
{
    /// <summary>
    /// Calculates weekend sales summaries grouped by car model.
    /// </summary>
    public static class WeekendSalesCalculator
    {
        /// <summary>
        /// Calculates total weekend sales amounts with and without VAT.
        /// </summary>
        /// <param name="cars">The vehicle sales to process.</param>
        /// <returns>
        /// Sales summaries for models with at least one weekend sale.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The input collection is null.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The calculated totals exceed the supported numeric range.
        /// </exception>
        public static List<CarSummary> Calculate(IEnumerable<Car> cars)
        {
            ArgumentNullException.ThrowIfNull(cars);

            var summaries = cars
                .Where(car => car.Sale.DayOfWeek is
                    DayOfWeek.Saturday or DayOfWeek.Sunday)
                .GroupBy(car => car.Model)
                .Select(group => new CarSummary
                {
                    Model = group.Key,
                    TotalWithoutVat = group.Sum(car => car.Price),
                    TotalWithVat = group.Sum(car =>
                        car.Price * (1 + car.VatRate / 100))
                })
                .ToList();

            if (summaries.Any(summary =>
                !double.IsFinite(summary.TotalWithoutVat) ||
                !double.IsFinite(summary.TotalWithVat)))
            {
                throw new OverflowException(
                    "Výsledná cena překračuje povolený rozsah.");
            }

            return summaries;
        }
    }
}