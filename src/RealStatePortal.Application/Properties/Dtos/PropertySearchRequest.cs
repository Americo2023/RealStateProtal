using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Properties.Dtos;

public sealed record PropertySearchRequest(
    string? Query,
    PropertyType? PropertyType,
    string? City,
    decimal? PriceMin,
    decimal? PriceMax,
    int? BedroomsMin,
    int? BathroomsMin,
    decimal? AreaMin,
    decimal? AreaMax,
    PropertyStatus? Status,
    string Sort = "Newest");
