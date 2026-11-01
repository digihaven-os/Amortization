<a id="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <h3 align="center">Amortization</h3>

  <p align="center">
    A .NET console application that builds a loan amortization schedule from a simple text file.
    <br />
    <a href="#usage"><strong>Explore the options »</strong></a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#project-structure">Project Structure</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li>
      <a href="#usage">Usage</a>
      <ul>
        <li><a href="#options-reference">Options reference</a></li>
        <li><a href="#example">Example</a></li>
        <li><a href="#calculation-rules">Calculation rules</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

Amortization reads the loan description from `options.txt`, computes the month-by-month repayment schedule
and prints it as a table in the console. The same schedule is exported to a CSV file.

Supported features:

* Fixed-rate loan with a monthly mortgage insurance (fixed amount or percentage of the outstanding balance)
* Initial deferral: only interest is paid during the first months
* Exceptional deferral: in a given month only the insurance is paid, and the loan is extended by one month
* One-off early repayments and yearly recurring early repayments (with an optional end month)
* Early repayment adjustment: keep the monthly payment (shorter loan) or keep the end date (lower payment)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

* [![.NET][dotnet-badge]][dotnet-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Project Structure

```
Domain/
  Loans/       LoanOptions, LoanTerm, LoanTermUnit, MortgageInsurance, MortgageInsuranceType,
               EarlyRepayment, RecurringEarlyRepayment, EarlyRepaymentMode
  Schedules/   AmortizationSchedule, AmortizationScheduleEntry
Infrastructure/
  Options/     LoanOptionsParser
  Export/      ScheduleCsvWriter
Presentation/  ScheduleConsolePrinter
Program.cs     Entry point
options.txt    Loan description
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Installation

1. Clone or download the repository
2. Move to the project folder
   ```sh
   cd Amortization
   ```
3. Edit `options.txt` to describe your loan
4. Run the application
   ```sh
   dotnet run
   ```

`options.txt` is copied next to the executable at build time, so the built `.exe` can also be launched directly.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE -->
## Usage

The application reads `options.txt` (one `Key=Value` per line, `//` starts a comment), prints the schedule
and writes it to a CSV file.

### Options reference

| Option | Format | Description |
|---|---|---|
| `LoanAmount` | `50000€` | Borrowed amount |
| `LoanTerm` | `114m` or `9.5y` | Total duration, in months or years (the initial deferral is included) |
| `AnnualNominalRate` | `3.83%` | Annual nominal interest rate |
| `MortgageInsurance` | `22€` or `0.3%` | Monthly insurance: fixed amount, or percentage of the outstanding balance |
| `StartDate` | `MM/yyyy` | Month of the first payment |
| `Deferral` | `18m` or `1.5y` | Initial period during which only interest is paid |
| `ExceptionalDeferral` | `MM/yyyy` | Month in which only the insurance is paid. Can be repeated |
| `EarlyRepayment` | `MM/yyyy:amount€` | One-off early repayment. Can be repeated |
| `YearlyEarlyRepayment` | `MM/yyyy:amount€[:MM/yyyy]` | Repayment every 12 months from the first month, until the optional last month (inclusive) or the end of the loan |
| `OutputFile` | `schedule.csv` | Name or path of the CSV file the schedule is written to (default: `amortization.csv`) |
| `EarlyRepaymentMode` | `term` or `payment` | `term`: keep the monthly payment, the loan ends earlier. `payment`: keep the end date, the monthly payment decreases |

### Example

```
LoanAmount=50000€
LoanTerm=114m
AnnualNominalRate=3.83%
MortgageInsurance=22€
StartDate=11/2024
EarlyRepaymentMode=payment
Deferral=18m
ExceptionalDeferral=07/2026
EarlyRepayment=02/2027:8000€
YearlyEarlyRepayment=05/2027:1500€:05/2033
OutputFile=amortization.csv
```

### Calculation rules

* Interest of a month = outstanding balance × annual rate / 12, rounded to 2 decimals.
* During the initial deferral, the payment is the interest only and the balance does not change.
* During an exceptional deferral, only the insurance is paid. The unpaid interest is added to the outstanding
  balance, and the loan ends one month later. An exceptional deferral that falls in the initial deferral is ignored.
* An early repayment is applied on the due date of its month, after the regular payment, and is capped at the
  remaining balance. Several repayments in the same month are added together.
* The schedule stops as soon as the balance reaches zero.

The schedule is written to the CSV file given by `OutputFile` (`amortization.csv` if the option is missing),
using `;` as separator and `.` as decimal separator.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are welcome.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a pull request

Please keep the one-type-per-file rule and place new code in the layer it belongs to.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[dotnet-badge]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[dotnet-url]: https://dotnet.microsoft.com/
