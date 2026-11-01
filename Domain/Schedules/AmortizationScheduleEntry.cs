namespace Amortization.Domain.Schedules;

public sealed class AmortizationScheduleEntry
{
    public DateOnly DueDate { get; init; }
    public DateOnly Date { get; init; }
    public decimal OutstandingPrincipal { get; init; }
    public decimal MonthlyPayment { get; init; }
    public decimal OfWhichPrincipal { get; init; }
    public decimal OfWhichInterest { get; init; }
    public decimal Insurance { get; init; }
    public decimal MonthlyPaymentIncludingInsurance { get; init; }
    public decimal EarlyRepayment { get; init; }
}
