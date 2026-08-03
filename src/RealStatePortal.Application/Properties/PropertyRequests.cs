using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Properties;

public sealed record CreatePropertyRequest(
    string ReferenceNumber,
    string Title,
    string Description,
    PropertyType PropertyType,
    decimal Price,
    string Currency);

public sealed record UpdatePropertyRequest(
    string Title,
    string Description,
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
    EnergyClass? EnergyClass);

public sealed record SetPropertyAddressRequest(
    string Street,
    string StreetNumber,
    string PostalCode,
    string City,
    string Region,
    string Country,
    double? Latitude,
    double? Longitude);

public sealed record AddPropertyImageRequest(
    Stream Content,
    string FileName,
    string ContentType,
    string AltText,
    int SortOrder,
    bool IsPrimary);