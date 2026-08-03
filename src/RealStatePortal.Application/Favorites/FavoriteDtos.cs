namespace RealStatePortal.Application.Favorites;

public sealed record FavoriteDto(Guid UserId, Guid PropertyId, DateTimeOffset CreatedAt);