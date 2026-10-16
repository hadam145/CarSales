using CarSales.Models;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace CarSales.Services
{
    /// <summary>
    /// Loads and validates vehicle sales records from XML files.
    /// </summary>
    public static class CarXmlReader
    {
        /// <summary>
        /// Loads vehicle sales records from the specified XML file.
        /// </summary>
        /// <param name="filePath">The path to the XML file.</param>
        /// <returns>A list of validated vehicle sales records.</returns>
        /// <exception cref="FormatException">
        /// The XML structure or vehicle data is invalid.
        /// </exception>
        /// <exception cref="XmlException">
        /// The XML document is malformed or contains prohibited content.
        /// </exception>
        public static List<Car> Load(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 5_000_000
            };

            using var reader = XmlReader.Create(filePath, settings);
            var document = XDocument.Load(reader);

            if (document.Root?.Name != "cars")
                throw new FormatException("Neplatná struktura XML.");

            if (document.Root.Elements().Any(e => e.Name != "car"))
                throw new FormatException(
                    "XML obsahuje neznámý element vozidla.");

            var cars = document.Root.Elements("car")
                .Select((element, index) => ParseCar(element, index + 1))
                .ToList();

            if (cars.Count == 0)
                throw new FormatException("XML neobsahuje žádná vozidla.");

            return cars;
        }

        /// <summary>
        /// Parses and validates a single vehicle sale record.
        /// </summary>
        private static Car ParseCar(XElement element, int recordNumber)
        {
            var unexpected = element.Elements().FirstOrDefault(e =>
                e.Name != "model" &&
                e.Name != "sale" &&
                e.Name != "price" &&
                e.Name != "dph");

            if (unexpected is not null)
                throw new FormatException(
                    $"Záznam {recordNumber}: neznámý element <{unexpected.Name}>.");

            var modelElement = GetRequiredElement(
                element, "model", recordNumber);

            var saleElement = GetRequiredElement(
                element, "sale", recordNumber);

            var priceElement = GetRequiredElement(
                element, "price", recordNumber);

            var vatElement = GetRequiredElement(
                element, "dph", recordNumber);

            var model = modelElement.Value.Trim();

            if (string.IsNullOrWhiteSpace(model))
                throw new FormatException(
                    $"Záznam {recordNumber}: chybí název modelu.");

            return new Car
            {
                Model = model,
                Sale = ParseSaleDate(saleElement, recordNumber),
                Price = ParsePrice(priceElement, recordNumber),
                VatRate = ParseVatRate(vatElement, recordNumber)
            };
        }

        /// <summary>
        /// Gets exactly one required child element.
        /// </summary>
        private static XElement GetRequiredElement(
            XElement parent,
            string name,
            int recordNumber)
        {
            var elements = parent.Elements(name).ToList();

            if (elements.Count == 0)
                throw new FormatException(
                    $"Záznam {recordNumber}: chybí element <{name}>.");

            if (elements.Count > 1)
                throw new FormatException(
                    $"Záznam {recordNumber}: duplicitní element <{name}>.");

            var element = elements[0];

            if (element.HasElements)
                throw new FormatException(
                    $"Záznam {recordNumber}: element <{name}> " +
                    "nesmí obsahovat vnořené elementy.");

            return element;
        }

        /// <summary>
        /// Parses a sale date using the required XML date-time format.
        /// </summary>
        private static DateTime ParseSaleDate(
            XElement element,
            int recordNumber)
        {
            const string dateFormat = "yyyy-MM-dd'T'HH:mm:ss";

            if (!DateTime.TryParseExact(
                    element.Value,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var sale))
            {
                throw new FormatException(
                    $"Záznam {recordNumber}: neplatné datum prodeje " +
                    $"'{element.Value}'. Očekávaný formát: yyyy-MM-ddTHH:mm:ss.");
            }

            return sale;
        }

        /// <summary>
        /// Parses and validates a non-negative vehicle price.
        /// </summary>
        private static double ParsePrice(
            XElement element,
            int recordNumber)
        {
            var price = ParseDouble(element, "ceny", recordNumber);

            if (!double.IsFinite(price) || price < 0)
                throw new FormatException(
                    $"Záznam {recordNumber}: cena musí být nezáporné konečné číslo.");

            return price;
        }

        /// <summary>
        /// Parses and validates a VAT rate expressed as a percentage.
        /// </summary>
        private static double ParseVatRate(
            XElement element,
            int recordNumber)
        {
            var vatRate = ParseDouble(element, "DPH", recordNumber);

            if (!double.IsFinite(vatRate) || vatRate is < 0 or > 100)
                throw new FormatException(
                    $"Záznam {recordNumber}: sazba DPH musí být mezi 0 a 100 %.");

            return vatRate;
        }

        /// <summary>
        /// Parses a numeric XML value using invariant culture.
        /// </summary>
        private static double ParseDouble(
            XElement element,
            string fieldName,
            int recordNumber)
        {
            if (!double.TryParse(
                    element.Value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var value))
            {
                throw new FormatException(
                    $"Záznam {recordNumber}: neplatný formát {fieldName} " +
                    $"'{element.Value}'.");
            }

            return value;
        }
    }
}