namespace Amortization.Domain.Loans;

public sealed class MortgageInsurance
{
    public MortgageInsuranceType Type { get; }

    public decimal Value { get; }

    private MortgageInsurance(MortgageInsuranceType type, decimal value)
    {
        Type = type;
        Value = value;
    }

    public static MortgageInsurance Fixed(decimal amount)
        => new(MortgageInsuranceType.FixedAmount, amount);

    public static MortgageInsurance Percentage(decimal percentage)
        => new(MortgageInsuranceType.Percentage, percentage);

    public override string ToString()
    {
        return Type switch
        {
            MortgageInsuranceType.FixedAmount => $"{Value:0.##}€",
            MortgageInsuranceType.Percentage => $"{Value:0.##}%",
            _ => Value.ToString()
        };
    }
}