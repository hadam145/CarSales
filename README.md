# CarSales

Jednoduchá desktopová aplikace pro Windows, která načítá záznamy o prodeji automobilů ze souboru XML, zobrazuje jejich přehled a vypočítává souhrnné tržby za vozidla prodaná v **sobotu a neděli**.
Výsledky seskupuje podle modelu a uvádí ceny bez DPH i s DPH.

## Funkcionalita
- Výběr XML souboru – uživatel vybere soubor z disku prostřednictvím systémového dialogu. Po úspěšném načtení se zobrazí název souboru a počet načtených vozidel.
- Víkendové souhrny – výchozí záložka zobrazuje vozidla prodaná v sobotu nebo v neděli, seskupená podle modelu. Pro každý model uvádí součet cen bez DPH a s DPH ve dvouřádkovém formátu podle zadání. DPH se počítá pro každý prodej samostatně, protože jednotlivá vozidla mohou mít různé sazby.
- Všechny prodeje – druhá záložka zobrazuje kompletní obsah načteného XML v tabulce se sloupci Název modelu, Datum prodeje, Cena bez DPH a DPH (%).
- Validace XML – při načítání se kontroluje struktura dokumentu, povinné elementy, formát data, ceny a sazby DPH. Neplatné soubory jsou odmítnuty a uživateli se zobrazí chybová zpráva v češtině.
- Automatizované testy – projekt obsahuje unit testy a integrační testy vytvořené pomocí frameworku xUnit, které ověřují načítání XML, validaci vstupů a správnost výpočtů.

## Použité technologie
- C# / .NET 10
- WPF
- LINQ to XML
- LINQ
- xUnit

## Ukázka aplikace

### Víkendové souhrny

<img width="797" height="617" alt="Víkendové souhrny prodejů" src="https://github.com/user-attachments/assets/0c03d498-697c-4dc5-8caa-9c3bb79c600a" />

### Všechny prodeje

<img width="797" height="617" alt="Přehled všech prodejů" src="https://github.com/user-attachments/assets/ab1b140a-1bd4-4c1d-a1e5-d81720fa96b7" />

## Požadavky

- Windows 10/11
- .NET 10 SDK
- Visual Studio 2026 (18.0 nebo novější)

## Spuštění aplikace

1. Naklonujte repozitář do lokálního adresáře.
2. Otevřete řešení `CarSales` ve Visual Studiu.
3. Nastavte projekt `CarSales` jako spouštěcí projekt a spusťte aplikaci pomocí `F5`.
4. Klikněte na **Vybrat XML soubor** a vyberte vzorový soubor ze složky `CarSales/SampleData`.
5. V záložkách **Víkendové souhrny** a **Všechny prodeje** můžete prohlížet zpracované výsledky a načtené záznamy.

Aplikaci lze spustit také z kořenového adresáře repozitáře pomocí .NET CLI:

```powershell
dotnet run --project CarSales/CarSales.csproj
```

## Testování

Projekt obsahuje samostatný testovací projekt `CarSales.Tests`, který využívá framework xUnit.

Testy pokrývají tři hlavní oblasti:

- **CarXmlReaderTests** – načítání XML, kontrolu povinných elementů, formátů hodnot a neplatných vstupů.
- **WeekendSalesCalculatorTests** – filtrování víkendových prodejů, seskupování modelů a výpočty DPH.
- **CarSalesIntegrationTests** – ověření celého procesu načtení XML a výpočtu souhrnných výsledků.

Testovací XML soubory jsou umístěny ve složce `CarSales.Tests/TestData`.

Testy lze spustit pomocí Visual Studio Test Exploreru nebo příkazem:

```powershell
dotnet test CarSales.Tests/CarSales.Tests.csproj
```
Projekt využívá také **GitHub Actions** pro automatické sestavení aplikace a spuštění testů při každém pushi a pull requestu. Workflow běží na Windows runneru kvůli závislosti aplikace na WPF.

## Struktura projektu

```text
CarSales/
├── .github/
│   └── workflows/
│       └── dotnet.yml
├── CarSales/
│   ├── Models/
│   │   ├── Car.cs
│   │   └── CarSummary.cs
│   ├── SampleData/
│   │   └── cars.xml
│   ├── Services/
│   │   ├── CarXmlReader.cs
│   │   └── WeekendSalesCalculator.cs
│   ├── MainWindow.xaml
│   └── CarSales.csproj
├── CarSales.Tests/
│   ├── TestData/
│   ├── CarSalesIntegrationTests.cs
│   ├── CarXmlReaderTests.cs
│   ├── WeekendSalesCalculatorTests.cs
│   └── CarSales.Tests.csproj
├── .gitignore
├── CarSales.slnx
└── README.md
```

## Architektura a implementace

Aplikace odděluje uživatelské rozhraní od načítání dat a výpočetní logiky.

- `CarXmlReader` zajišťuje načítání a validaci XML. Kontroluje strukturu dokumentu, povinné hodnoty a jejich správný formát.
- `WeekendSalesCalculator` zpracovává víkendové prodeje a vypočítává souhrny nezávisle na uživatelském rozhraní.
- `MainWindow` zajišťuje interakci s uživatelem a prezentaci výsledků.

Pro zpracování dat je využito LINQ. DPH se vypočítává samostatně pro každý prodej, protože jednotlivá vozidla stejného modelu mohou mít různé sazby DPH.
