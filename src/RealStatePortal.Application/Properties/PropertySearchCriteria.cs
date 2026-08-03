using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Properties;

public sealed record PropertySearchCriteria(
    string? SearchText = null,
    PropertyType? PropertyType = null,
    string? City = null,
    decimal? PriceMin = null,
    decimal? PriceMax = null,
    int? BedroomsMin = null,
    int? BathroomsMin = null,
    decimal? AreaMin = null,
    decimal? AreaMax = null,
    PropertyStatus? Status = null,
    PropertySortOrder SortOrder = PropertySortOrder.Newest,
    int PageNumber = 1,
    int PageSize = 20);