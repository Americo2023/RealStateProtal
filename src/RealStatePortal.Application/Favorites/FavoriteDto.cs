using RealStatePortal.Application.Properties.Dtos;

namespace RealStatePortal.Application.Favorites;

public sealed record FavoriteDto(Guid Id, DateTimeOffset CreatedAt, PropertyDto Property);