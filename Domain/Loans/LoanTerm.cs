namespace Amortization.Domain.Loans;

public sealed class LoanTerm
{
    public int Months { get; }

    private LoanTerm(int months)
    {
        Months = months;
    }

    public static LoanTerm FromMonths(int months)
        => new(months);

    public static LoanTerm FromYears(decimal years)
        => new((int)Math.Round(years * 12m));
}