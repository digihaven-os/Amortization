namespace Amortization.Domain.Loans;

/// <param name="EndDate">Last month in which the repayment may occur (inclusive); null means until the loan ends.</param>
public sealed record RecurringEarlyRepayment(DateOnly StartDate, decimal Amount, int IntervalMonths, DateOnly? EndDate = null);
