namespace Amortization.Domain.Loans;

public enum EarlyRepaymentMode
{
    /// <summary>Monthly payment stays the same, the loan ends earlier.</summary>
    ReduceTerm,

    /// <summary>Loan term stays the same, the monthly payment decreases.</summary>
    ReducePayment,
}
