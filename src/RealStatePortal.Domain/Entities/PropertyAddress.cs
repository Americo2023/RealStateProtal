using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.ValueObjects;

namespace RealStatePortal.Domain.Entities;

public sealed class PropertyAddress : Entity
{
    public PropertyAddress(
        Guid id,
        string street,
        string streetNumber,
        string postalCode,
        string city,
        string region,
        string country,
        Coordinates? coordinates = null)
        : base(id)
    {
        Street = Required(street, nameof(street));
        StreetNumber = Required(streetNumber, nameof(streetNumber));
        PostalCode = Required(postalCode, nameof(postalCode));
        City = Required(city, nameof(city));
        Region = Required(region, nameof(region));
        Country = Required(country, nameof(country));
        Coordinates = coordinates;
    }

    private PropertyAddress()
        : base(Guid.NewGuid())
    {
        Street = string.Empty;
        StreetNumber = string.Empty;
        PostalCode = string.Empty;
        City = string.Empty;
        Region = string.Empty;
        Country = string.Empty;
    }

    public string Street { get; private set; }
    public string StreetNumber { get; private set; }
    public string PostalCode { get; private set; }
    public string City { get; private set; }
    public string Region { get; private set; }
    public string Country { get; private set; }
    public Coordinates? Coordinates { get; private set; }

    private static string Required(string value, string name)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value is required.", name)
            : value.Trim();
    }
}
