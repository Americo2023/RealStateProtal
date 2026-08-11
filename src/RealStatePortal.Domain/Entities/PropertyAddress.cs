using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.ValueObjects;

namespace RealStatePortal.Domain.Entities;

public sealed class PropertyAddress : Entity
{
    public PropertyAddress(
        string street,
        string streetNumber,
        string postalCode,
        string city,
        string region,
        string country,
        Coordinates coordinates,
        Guid? id = null)
        : base(id)
    {
        Update(street, streetNumber, postalCode, city, region, country, coordinates);
    }

    public string Street { get; private set; } = null!;
    public string StreetNumber { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Region { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public Coordinates Coordinates { get; private set; }

    internal void Update(
        string street,
        string streetNumber,
        string postalCode,
        string city,
        string region,
        string country,
        Coordinates coordinates)
    {
        Street = Guard.Required(street, nameof(street));
        StreetNumber = Guard.Required(streetNumber, nameof(streetNumber));
        PostalCode = Guard.Required(postalCode, nameof(postalCode));
        City = Guard.Required(city, nameof(city));
        Region = Guard.Required(region, nameof(region));
        Country = Guard.Required(country, nameof(country));
        Coordinates = coordinates;
    }
}