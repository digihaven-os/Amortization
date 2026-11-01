using System.Globalization;
using Amortization.Domain.Schedules;

namespace Amortization.Presentation;

public static class ScheduleConsolePrinter
{
    public static void Print(IReadOnlyList<AmortizationScheduleEntry> schedule)
    {
        const string header = "DUE DATE";
        const string header2 = "DATE";
        const string header3 = "OUTSTANDING PRINCIPAL";
        const string header4 = "MONTHLY PAYMENT";
        const string header5 = "OF WHICH PRINCIPAL";
        const string header6 = "OF WHICH INTEREST";
        const string header7 = "INSURANCE";
        const string header8 = "MONTHLY PAYMENT INCLUDING INSURANCE";
        const string header9 = "EARLY REPAYMENT";
        const string format = "| {0,-10} | {1,-10} | {2,22} | {3,18} | {4,20} | {5,19} | {6,12} | {7,35} | {8,15} |";

        var displayCulture = CultureInfo.GetCultureInfo("fr-FR");
        var headerLine = string.Format(format, header, header2, header3, header4, header5, header6, header7, header8, header9);
        Console.WriteLine(headerLine);
        Console.WriteLine(new string('-', headerLine.Length));

        for (var index = 0; index < schedule.Count; index++)
        {
            var entry = schedule[index];
            Console.WriteLine(format,
                (index + 1).ToString(CultureInfo.InvariantCulture),
                entry.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                entry.OutstandingPrincipal.ToString("0.00", displayCulture),
                entry.MonthlyPayment.ToString("0.00", displayCulture),
                entry.OfWhichPrincipal.ToString("0.00", displayCulture),
                entry.OfWhichInterest.ToString("0.00", displayCulture),
                entry.Insurance.ToString("0.00", displayCulture),
                entry.MonthlyPaymentIncludingInsurance.ToString("0.00", displayCulture),
                entry.EarlyRepayment.ToString("0.00", displayCulture));
        }
    }
}
