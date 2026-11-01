using Amortization.Domain.Loans;

namespace Amortization.Domain.Schedules;

public static class AmortizationSchedule
{
    public static IReadOnlyList<AmortizationScheduleEntry> Generate(LoanOptions options)
    {
        if (options.LoanTerm is null)
        {
            throw new InvalidOperationException("Loan term is required to build an amortization schedule.");
        }

        var months = options.LoanTerm.Months;
        if (months <= 0)
        {
            throw new InvalidOperationException("Loan term must be greater than zero months.");
        }

        var principal = options.LoanAmount;
        var monthlyRate = (options.AnnualNominalRate / 100m) / 12m;
        var deferral = options.DeferralMonths;
        if (deferral < 0 || deferral >= months)
        {
            throw new InvalidOperationException("Deferral must be shorter than the loan term.");
        }

        // Each exceptional deferral (after the initial deferral) pushes the end of the loan back by one month.
        var exceptionalMonths = options.ExceptionalDeferrals
            .Select(date => MonthIndex(options.StartDate, date))
            .Where(index => index >= deferral && index < months)
            .ToHashSet();
        var totalMonths = months + exceptionalMonths.Count;

        var amortizingPayment = CalculateMonthlyPayment(principal, monthlyRate, months - deferral);
        var monthlyPayment = amortizingPayment;
        var currentBalance = principal;
        var entries = new List<AmortizationScheduleEntry>(totalMonths);

        for (var month = 0; month < totalMonths; month++)
        {
            var dueDate = options.StartDate.AddMonths(month);
            var interest = Math.Round(currentBalance * monthlyRate, 2, MidpointRounding.AwayFromZero);
            decimal principalPayment;
            var capitalizedInterest = 0m;

            if (month < deferral)
            {
                // Deferred period: interest only, no capital repaid.
                principalPayment = 0m;
                monthlyPayment = interest;
            }
            else if (exceptionalMonths.Contains(month))
            {
                // Exceptional deferral: only the insurance is paid, the unpaid interest is added to the balance.
                principalPayment = 0m;
                capitalizedInterest = interest;
                monthlyPayment = 0m;
                interest = 0m;
            }
            else
            {
                monthlyPayment = amortizingPayment;
                principalPayment = monthlyPayment - interest;

                if (month == totalMonths - 1 || principalPayment > currentBalance)
                {
                    principalPayment = currentBalance;
                    monthlyPayment = principalPayment + interest;
                    amortizingPayment = monthlyPayment;
                }
            }

            var insurance = GetMonthlyInsurance(options.MortgageInsurance, currentBalance);
            var paymentIncludingInsurance = Math.Round(monthlyPayment + insurance, 2, MidpointRounding.AwayFromZero);
            var balanceAfterPayment = currentBalance - principalPayment + capitalizedInterest;

            var earlyRepayment = options.EarlyRepayments
                .Where(r => r.Date.Year == dueDate.Year && r.Date.Month == dueDate.Month)
                .Sum(r => r.Amount);
            earlyRepayment += options.RecurringEarlyRepayments
                .Where(r => IsRecurringDue(r, options.StartDate, month))
                .Sum(r => r.Amount);
            earlyRepayment = Math.Min(earlyRepayment, balanceAfterPayment);
            balanceAfterPayment -= earlyRepayment;

            entries.Add(new AmortizationScheduleEntry
            {
                DueDate = dueDate,
                Date = dueDate,
                OutstandingPrincipal = currentBalance,
                MonthlyPayment = monthlyPayment,
                OfWhichPrincipal = principalPayment,
                OfWhichInterest = interest,
                Insurance = insurance,
                MonthlyPaymentIncludingInsurance = paymentIncludingInsurance,
                EarlyRepayment = earlyRepayment,
            });

            currentBalance = balanceAfterPayment;

            if (currentBalance <= 0m)
            {
                break;
            }

            if (earlyRepayment > 0m && options.EarlyRepaymentMode == EarlyRepaymentMode.ReducePayment)
            {
                amortizingPayment = CalculateMonthlyPayment(currentBalance, monthlyRate, totalMonths - Math.Max(month + 1, deferral) - exceptionalMonths.Count(index => index > month));
            }
        }

        return entries;
    }

    private static int MonthIndex(DateOnly start, DateOnly date)
        => (date.Year - start.Year) * 12 + date.Month - start.Month;

    private static bool IsRecurringDue(RecurringEarlyRepayment repayment, DateOnly start, int month)
    {
        var elapsed = month - MonthIndex(start, repayment.StartDate);
        if (repayment.EndDate is { } endDate && month > MonthIndex(start, endDate))
        {
            return false;
        }

        return elapsed >= 0 && elapsed % repayment.IntervalMonths == 0;
    }

    private static decimal CalculateMonthlyPayment(decimal principal, decimal monthlyRate, int months)
    {
        if (monthlyRate == 0m)
        {
            return Math.Round(principal / months, 2, MidpointRounding.AwayFromZero);
        }

        var factor = (decimal)Math.Pow((double)(1 + monthlyRate), months);
        var payment = principal * monthlyRate * factor / (factor - 1);

        return Math.Round(payment, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal GetMonthlyInsurance(MortgageInsurance? insurance, decimal outstandingBalance)
    {
        if (insurance is null)
        {
            return 0m;
        }

        return insurance.Type switch
        {
            MortgageInsuranceType.FixedAmount => Math.Round(insurance.Value, 2, MidpointRounding.AwayFromZero),
            MortgageInsuranceType.Percentage => Math.Round(outstandingBalance * insurance.Value / 100m, 2, MidpointRounding.AwayFromZero),
            _ => 0m,
        };
    }
}
