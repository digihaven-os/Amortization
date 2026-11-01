using System.Globalization;
using System.Text;
using Amortization.Domain.Schedules;

namespace Amortization.Infrastructure.Export;

public static class ScheduleCsvWriter
{
    public static void Write(IReadOnlyList<AmortizationScheduleEntry> schedule, string filePath)
    {
        var lines = new List<string>
        {
            "DUE DATE;DATE;OUTSTANDING PRINCIPAL;MONTHLY PAYMENT;OF WHICH PRINCIPAL;OF WHICH INTEREST;INSURANCE;MONTHLY PAYMENT INCLUDING INSURANCE;EARLY REPAYMENT"
        };

        for (var index = 0; index < schedule.Count; index++)
        {
            var entry = schedule[index];
            lines.Add(string.Join(";", new[]
            {
                (index + 1).ToString(CultureInfo.InvariantCulture),
                entry.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                entry.OutstandingPrincipal.ToString("0.00", CultureInfo.InvariantCulture),
                entry.MonthlyPayment.ToString("0.00", CultureInfo.InvariantCulture),
                entry.OfWhichPrincipal.ToString("0.00", CultureInfo.InvariantCulture),
                entry.OfWhichInterest.ToString("0.00", CultureInfo.InvariantCulture),
                entry.Insurance.ToString("0.00", CultureInfo.InvariantCulture),
                entry.MonthlyPaymentIncludingInsurance.ToString("0.00", CultureInfo.InvariantCulture),
                entry.EarlyRepayment.ToString("0.00", CultureInfo.InvariantCulture),
            }));
        }

        File.WriteAllLines(filePath, lines, Encoding.UTF8);
    }
}
