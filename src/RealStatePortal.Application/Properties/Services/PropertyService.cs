using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Common;
using RealStatePortal.Application.Properties.Dtos;
using RealStatePortal.Domain.Common;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.ValueObjects;

namespace RealStatePortal.Application.Properties.Services;

public sealed class PropertyService(
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IPropertyService
{
    public async Task<Result<PropertyDto>> GetByIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        return property is null
            ? Result<PropertyDto>.Failure("Property was not found.")
            : Result<PropertyDto>.Success(Map(property));
    }

    public async Task<Result<IReadOnlyCollection<PropertyDto>>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var properties = await propertyRepository.GetPublishedAsync(cancellationToken);
        return Result<IReadOnlyCollection<PropertyDto>>.Success(properties.Select(Map).ToArray());
    }

    public async Task<Result<IReadOnlyCollection<PropertyDto>>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var properties = await propertyRepository.SearchAsync(query, cancellationToken);
        return Result<IReadOnlyCollection<PropertyDto>>.Success(properties.Select(Map).ToArray());
    }

    public async Task<Result<PropertyDto>> CreateAsync(CreatePropertyRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserIsBroker())
        {
            return Result<PropertyDto>.Failure("Only brokers can create properties.");
        }

        if (!currentUser.UserId.HasValue)
        {
            return Result<PropertyDto>.Failure("An authenticated broker is required.");
        }

        if (await propertyRepository.ExistsByReferenceNumberAsync(request.ReferenceNumber, cancellationToken))
        {
            return Result<PropertyDto>.Failure("The property reference number already exists.");
        }

        var property = new Property(
            request.ReferenceNumber,
            request.Title,
            request.Description,
            request.PropertyType,
            new Money(request.Price, request.Currency),
            request.Bedrooms,
            request.Bathrooms,
            request.Rooms,
            request.LivingArea,
            request.TotalArea,
            request.Floor,
            request.NumberOfFloors,
            request.ConstructionYear,
            request.EnergyClass,
            currentUser.UserId.Value,
            dateTimeProvider.UtcNow);

        await propertyRepository.AddAsync(property, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<PropertyDto>.Success(Map(property));
    }

    public async Task<Result> UpdateAsync(Guid propertyId, UpdatePropertyRequest request, CancellationToken cancellationToken = default)
    {
        var propertyResult = await GetOwnedPropertyAsync(propertyId, cancellationToken);
        if (!propertyResult.IsSuccess)
        {
            return Result.Failure(propertyResult.Error!);
        }

        var property = propertyResult.Value!;
        property.UpdateDetails(
            request.Title,
            request.Description,
            request.PropertyType,
            new Money(request.Price, request.Currency),
            request.Bedrooms,
            request.Bathrooms,
            request.Rooms,
            request.LivingArea,
            request.TotalArea,
            request.Floor,
            request.NumberOfFloors,
            request.ConstructionYear,
            request.EnergyClass,
            dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public Task<Result> PublishAsync(Guid propertyId, CancellationToken cancellationToken = default) => ChangeStatusAsync(propertyId, (property, now) => property.Publish(now), cancellationToken);
    public Task<Result> WithdrawAsync(Guid propertyId, CancellationToken cancellationToken = default) => ChangeStatusAsync(propertyId, (property, now) => property.Withdraw(now), cancellationToken);
    public Task<Result> MarkAsSoldAsync(Guid propertyId, CancellationToken cancellationToken = default) => ChangeStatusAsync(propertyId, (property, now) => property.MarkAsSold(now), cancellationToken);
    public Task<Result> DeleteAsync(Guid propertyId, CancellationToken cancellationToken = default) => ChangeStatusAsync(propertyId, (property, now) => property.Delete(now), cancellationToken);

    private async Task<Result> ChangeStatusAsync(Guid propertyId, Action<Property, DateTimeOffset> transition, CancellationToken cancellationToken)
    {
        var propertyResult = await GetOwnedPropertyAsync(propertyId, cancellationToken);
        if (!propertyResult.IsSuccess)
        {
            return Result.Failure(propertyResult.Error!);
        }

        try
        {
            transition(propertyResult.Value!, dateTimeProvider.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception exception) when (exception is ArgumentException or DomainException)
        {
            return Result.Failure(exception.Message);
        }
    }

    private async Task<Result<Property>> GetOwnedPropertyAsync(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!currentUser.UserId.HasValue)
        {
            return Result<Property>.Failure("Authentication is required.");
        }

        var property = await propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
        {
            return Result<Property>.Failure("Property was not found.");
        }

        var isAdministrator = currentUser.Roles.Contains("Administrator", StringComparer.OrdinalIgnoreCase);
        if (!isAdministrator && property.BrokerId != currentUser.UserId.Value)
        {
            return Result<Property>.Failure("The current user cannot manage this property.");
        }

        return Result<Property>.Success(property);
    }

    private bool _currentUserIsBroker() => currentUser.Roles.Contains("Broker", StringComparer.OrdinalIgnoreCase)
        || currentUser.Roles.Contains("Administrator", StringComparer.OrdinalIgnoreCase);

    private static PropertyDto Map(Property property) => new(
        property.Id,
        property.ReferenceNumber,
        property.Title,
        property.Description,
        property.Status,
        property.PropertyType,
        property.Price.Amount,
        property.Price.Currency,
        property.Bedrooms,
        property.Bathrooms,
        property.Rooms,
        property.LivingArea,
        property.TotalArea,
        property.Floor,
        property.NumberOfFloors,
        property.ConstructionYear,
        property.EnergyClass,
        property.PublishedAt,
        property.CreatedAt,
        property.UpdatedAt,
        property.BrokerId,
        property.Address is null ? null : new PropertyAddressDto(
            property.Address.Id,
            property.Address.Street,
            property.Address.StreetNumber,
            property.Address.PostalCode,
            property.Address.City,
            property.Address.Region,
            property.Address.Country,
            property.Address.Coordinates.Latitude,
            property.Address.Coordinates.Longitude),
        property.Images.Select(image => new PropertyImageDto(image.Id, image.Url, image.AltText, image.SortOrder, image.IsPrimary)).ToArray());
}