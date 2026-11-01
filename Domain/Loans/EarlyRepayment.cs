namespace Amortization.Domain.Loans;

public sealed record EarlyRepayment(DateOnly Date, decimal Amount);
