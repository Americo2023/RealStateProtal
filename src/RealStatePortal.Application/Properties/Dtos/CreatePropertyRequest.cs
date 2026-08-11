using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Properties.Dtos;

public sealed record CreatePropertyRequest(
    string ReferenceNumber,
    string Title,
    string Description,
    PropertyType PropertyType,
    decimal Price,
    string Currency,
    int Bedrooms,
    int Bathrooms,
    int Rooms,
    decimal LivingArea,
    decimal TotalArea,
    int? Floor,
    int? NumberOfFloors,
    int? ConstructionYear,
    EnergyClass EnergyClass);

public sealed record UpdatePropertyRequest(
    string Title,
    string Description,
    PropertyType PropertyType,
    decimal Price,
    string Currency,
    int Bedrooms,
    int Bathrooms,
    int Rooms,
    decimal LivingArea,
    decimal TotalArea,
    int? Floor,
    int? NumberOfFloors,
    int? ConstructionYear,
    EnergyClass EnergyClass);