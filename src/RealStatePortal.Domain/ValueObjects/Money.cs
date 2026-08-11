using RealStatePortal.Domain.Common;

namespace RealStatePortal.Domain.ValueObjects;

public sealed record Money
{
    private Money()
    {
        Amount = 0;
        Currency = string.Empty;
    }

    public Money(decimal amount, string currency)
    {
        Amount = Guard.NonNegative(amount, nameof(amount));
        Currency = Guard.Required(currency, nameof(currency)).ToUpperInvariant();

        if (Currency.Length != 3)
        {
            throw new ArgumentException("Currency must use a three-letter ISO code.", nameof(currency));
        }
    }

    public decimal Amount { get; }

    public string Currency { get; }
}