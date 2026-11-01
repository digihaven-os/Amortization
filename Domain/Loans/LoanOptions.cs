namespace Amortization.Domain.Loans;

public sealed class LoanOptions
{
    public decimal LoanAmount { get; set; }

    public LoanTerm? LoanTerm { get; set; }

    public decimal AnnualNominalRate { get; set; }

    public MortgageInsurance? MortgageInsurance { get; set; }

    public DateOnly StartDate { get; set; }

    /// <summary>Number of initial months during which only interest is paid (included in the loan term).</summary>
    public int DeferralMonths { get; set; }

    /// <summary>Months in which only interest is paid; each one extends the loan by one month.</summary>
    public List<DateOnly> ExceptionalDeferrals { get; } = new();

    public List<EarlyRepayment> EarlyRepayments { get; } = new();

    public List<RecurringEarlyRepayment> RecurringEarlyRepayments { get; } = new();

    /// <summary>Path of the CSV file the schedule is written to.</summary>
    public string OutputFilePath { get; set; } = "amortization.csv";

    public EarlyRepaymentMode EarlyRepaymentMode { get; set; } = EarlyRepaymentMode.ReduceTerm;
}
