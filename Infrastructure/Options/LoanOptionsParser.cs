using System.Globalization;
using Amortization.Domain.Loans;

namespace Amortization.Infrastructure.Options;

public static class LoanOptionsParser
{
    public static LoanOptions Parse(string filePath)
    {
        var options = new LoanOptions();

        foreach (var rawLine in File.ReadLines(filePath))
        {
            var line = RemoveComment(rawLine).Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split('=', 2);

            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            switch (key)
            {
                case "LoanAmount":
                    options.LoanAmount = ParseDecimal(value.TrimEnd('€'));
                    break;

                case "LoanTerm":
                    options.LoanTerm = ParseLoanTerm(value);
                    break;

                case "AnnualNominalRate":
                    options.AnnualNominalRate = ParsePercentage(value);
                    break;

                case "MortgageInsurance":
                    options.MortgageInsurance = ParseMortgageInsurance(value);
                    break;

                case "StartDate":
                    options.StartDate = ParseStartDate(value);
                    break;

                case "OutputFile":
                    options.OutputFilePath = value;
                    break;

                case "Deferral":
                    options.DeferralMonths = ParseLoanTerm(value).Months;
                    break;

                case "ExceptionalDeferral":
                    options.ExceptionalDeferrals.Add(ParseStartDate(value));
                    break;

                case "EarlyRepayment":
                    options.EarlyRepayments.Add(ParseEarlyRepayment(value));
                    break;

                case "YearlyEarlyRepayment":
                    options.RecurringEarlyRepayments.Add(ParseYearlyEarlyRepayment(value));
                    break;

                case "EarlyRepaymentMode":
                    options.EarlyRepaymentMode = value.ToLowerInvariant() switch
                    {
                        "term" => EarlyRepaymentMode.ReduceTerm,
                        "payment" => EarlyRepaymentMode.ReducePayment,
                        _ => throw new FormatException($"Invalid early repayment mode: {value}"),
                    };
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unknown option '{key}'.");
            }
        }

        return options;
    }

    private static string RemoveComment(string line)
    {
        var index = line.IndexOf("//", StringComparison.Ordinal);

        return index >= 0
            ? line[..index]
            : line;
    }

    private static decimal ParseDecimal(string value)
    {
        value = value.Replace(',', '.');

        return decimal.Parse(
            value,
            NumberStyles.Number,
            CultureInfo.InvariantCulture);
    }

    private static decimal ParsePercentage(string value)
    {
        value = value.Trim();

        if (!value.EndsWith("%"))
            throw new FormatException($"Invalid percentage: {value}");

        return ParseDecimal(value[..^1]);
    }

    private static LoanTerm ParseLoanTerm(string value)
    {
        value = value.Trim().ToLowerInvariant();

        if (value.EndsWith("m"))
        {
            var months = int.Parse(value[..^1]);
            return LoanTerm.FromMonths(months);
        }

        if (value.EndsWith("y"))
        {
            var years = ParseDecimal(value[..^1]);
            return LoanTerm.FromYears(years);
        }

        throw new FormatException($"Invalid loan term: {value}");
    }

    private static MortgageInsurance ParseMortgageInsurance(string value)
    {
        value = value.Trim();

        if (value.EndsWith("€"))
        {
            var amount = ParseDecimal(value[..^1]);
            return MortgageInsurance.Fixed(amount);
        }

        if (value.EndsWith("%"))
        {
            var percentage = ParsePercentage(value);
            return MortgageInsurance.Percentage(percentage);
        }

        throw new FormatException(
            $"Invalid mortgage insurance value: {value}");
    }

    // Format: EarlyRepayment=MM/yyyy:amount€  (the option may be repeated)
    private static EarlyRepayment ParseEarlyRepayment(string value)
    {
        var parts = value.Split(':', 2);

        if (parts.Length != 2)
            throw new FormatException($"Invalid early repayment: {value}");

        var date = ParseStartDate(parts[0].Trim());
        var amount = ParseDecimal(parts[1].Trim().TrimEnd('€'));

        return new EarlyRepayment(date, amount);
    }

    // Format: YearlyEarlyRepayment=MM/yyyy:amount€[:MM/yyyy]  (optional last month, inclusive)
    private static RecurringEarlyRepayment ParseYearlyEarlyRepayment(string value)
    {
        var parts = value.Split(':');

        if (parts.Length is < 2 or > 3)
            throw new FormatException($"Invalid yearly early repayment: {value}");

        var startDate = ParseStartDate(parts[0].Trim());
        var amount = ParseDecimal(parts[1].Trim().TrimEnd('€'));
        DateOnly? endDate = parts.Length == 3 ? ParseStartDate(parts[2].Trim()) : null;

        return new RecurringEarlyRepayment(startDate, amount, 12, endDate);
    }

    private static DateOnly ParseStartDate(string value)
    {
        return DateOnly.ParseExact(
            "01/" + value,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture);
    }
}
