using System.ComponentModel.DataAnnotations;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Properties.Dtos;

public sealed record CreatePropertyRequest(
    [param: Required, StringLength(50)]
    string ReferenceNumber,
    [param: Required, StringLength(200)]
    string Title,
    [param: Required, StringLength(4000)]
    string Description,
    PropertyType PropertyType,
    [param: Range(0.01, double.MaxValue)]
    decimal Price,
    [param: Required, StringLength(3, MinimumLength = 3)]
    string Currency,
    [param: Range(0, 100)]
    int Bedrooms,
    [param: Range(0, 100)]
    int Bathrooms,
    [param: Range(0, 100)]
    int Rooms,
    [param: Range(0.01, double.MaxValue)]
    decimal LivingArea,
    [param: Range(0.01, double.MaxValue)]
    decimal TotalArea,
    int? Floor,
    int? NumberOfFloors,
    int? ConstructionYear,
    EnergyClass EnergyClass);

public sealed record UpdatePropertyRequest(
    [param: Required, StringLength(200)]
    string Title,
    [param: Required, StringLength(4000)]
    string Description,
    PropertyType PropertyType,
    [param: Range(0.01, double.MaxValue)]
    decimal Price,
    [param: Required, StringLength(3, MinimumLength = 3)]
    string Currency,
    [param: Range(0, 100)]
    int Bedrooms,
    [param: Range(0, 100)]
    int Bathrooms,
    [param: Range(0, 100)]
    int Rooms,
    [param: Range(0.01, double.MaxValue)]
    decimal LivingArea,
    [param: Range(0.01, double.MaxValue)]
    decimal TotalArea,
    int? Floor,
    int? NumberOfFloors,
    int? ConstructionYear,
    EnergyClass EnergyClass);