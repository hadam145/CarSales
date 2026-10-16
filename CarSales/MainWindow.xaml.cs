using CarSales.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Xml;

namespace CarSales
{
    /// <summary>
    /// Provides the user interface for importing and displaying car sales.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectXmlFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Vyberte XML soubor",
                Filter = "XML soubory (*.xml)|*.xml",
                Multiselect = false
            };

            if (dialog.ShowDialog(this) != true)
                return;

            try
            {
                var cars = CarXmlReader.Load(dialog.FileName);
                var summaries = WeekendSalesCalculator.Calculate(cars);

                AllCarsDataGrid.ItemsSource = cars;
                ResultsDataGrid.ItemsSource = summaries;

                SalesTabControl.SelectedIndex= 0;

                FileStatusTextBlock.Text =
                    $"Soubor: {Path.GetFileName(dialog.FileName)} | " +
                    $"Načteno: {cars.Count} vozidel";
            }
            catch (XmlException ex)
            {
                ClearResults();

                var location = ex.LineNumber > 0
                    ? $"Řádek: {ex.LineNumber}, pozice: {ex.LinePosition}.\n"
                    : string.Empty;

                MessageBox.Show(
                    this,
                    "XML soubor je syntakticky neplatný nebo obsahuje nepovolený obsah.\n" +
                    location +
                    "Zkontrolujte strukturu a obsah XML.",
                    "Neplatný XML soubor",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                FormatException or
                OverflowException)
            {
                ClearResults();

                MessageBox.Show(
                    this,
                    ex.Message,
                    "Chyba při načítání XML",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears the displayed sales results and resets the file status.
        /// </summary>
        private void ClearResults()
        {
            ResultsDataGrid.ItemsSource = null;
            AllCarsDataGrid.ItemsSource= null;

            FileStatusTextBlock.Text = "Soubor se nepodařilo načíst.";
        }
    }
}