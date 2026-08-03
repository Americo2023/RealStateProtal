using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Properties;

public sealed record PropertyDto(
    Guid Id,
    string ReferenceNumber,
    string Title,
    string Description,
    PropertyStatus Status,
    PropertyType PropertyType,
    decimal Price,
    string Currency,
    int Bedrooms,
    int Bathrooms,
    int Rooms,
    decimal? LivingArea,
    decimal? TotalArea,
    int? Floor,
    int? NumberOfFloors,
    int? ConstructionYear,
    EnergyClass? EnergyClass,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid BrokerId,
    PropertyAddressDto? Address,
    IReadOnlyCollection<PropertyImageDto> Images);

public sealed record PropertyImageDto(
    Guid Id,
    string Url,
    string AltText,
    int SortOrder,
    bool IsPrimary);

public sealed record PropertyAddressDto(
    Guid Id,
    string Street,
    string StreetNumber,
    string PostalCode,
    string City,
    string Region,
    string Country,
    double? Latitude,
    double? Longitude);