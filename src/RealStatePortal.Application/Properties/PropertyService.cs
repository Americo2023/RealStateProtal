using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Storage;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;
using RealStatePortal.Domain.ValueObjects;

namespace RealStatePortal.Application.Properties;

public sealed class PropertyService(
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    IImageStorage imageStorage) : IPropertyService
{
    public async Task<PropertyDto> CreateAsync(CreatePropertyRequest request, CancellationToken cancellationToken = default)
    {
        var brokerId = RequireUserId();
        EnsureBrokerOrAdministrator();

        var property = Property.Create(
            request.ReferenceNumber,
            request.Title,
            request.Description,
            request.PropertyType,
            new Money(request.Price, request.Currency),
            brokerId,
            dateTimeProvider.UtcNow);

        propertyRepository.Add(property);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(property);
    }

    public async Task<PropertyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        return Map(property);
    }

    public async Task<IReadOnlyCollection<PropertyDto>> SearchAsync(PropertySearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var properties = await propertyRepository.SearchAsync(criteria, cancellationToken);
        return properties.Select(Map).ToArray();
    }

    public async Task UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        EnsureCanManage(property);

        property.UpdateDetails(
            request.Title,
            request.Description,
            request.PropertyType,
            new Money(request.Price, request.Currency),
            dateTimeProvider.UtcNow);
        property.UpdateCharacteristics(
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
    }

    public async Task TransferAsync(Guid id, Guid brokerId, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        if (!currentUser.IsInRole(UserRole.Administrator))
        {
            throw new UnauthorizedAccessException("Only administrators can transfer properties.");
        }

        property.TransferTo(brokerId, dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SetAddressAsync(Guid id, SetPropertyAddressRequest request, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        EnsureCanManage(property);

        Coordinates? coordinates = request.Latitude.HasValue && request.Longitude.HasValue
            ? new Coordinates(request.Latitude.Value, request.Longitude.Value)
            : null;
        var address = new PropertyAddress(
            Guid.NewGuid(),
            request.Street,
            request.StreetNumber,
            request.PostalCode,
            request.City,
            request.Region,
            request.Country,
            coordinates);

        property.SetAddress(address, dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddImageAsync(Guid id, AddPropertyImageRequest request, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        EnsureCanManage(property);
        var url = await imageStorage.SaveAsync(request.Content, request.FileName, request.ContentType, cancellationToken);
        property.AddImage(
            new PropertyImage(Guid.NewGuid(), url, request.AltText, request.SortOrder, request.IsPrimary),
            dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveImageAsync(Guid id, Guid imageId, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        EnsureCanManage(property);
        var image = property.Images.SingleOrDefault(item => item.Id == imageId)
            ?? throw new KeyNotFoundException($"Image '{imageId}' was not found.");
        property.RemoveImage(imageId, dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageStorage.DeleteAsync(image.Url, cancellationToken);
    }

    public Task PublishAsync(Guid id, CancellationToken cancellationToken = default) => ChangeStatusAsync(id, property => property.Publish(dateTimeProvider.UtcNow), cancellationToken);

    public Task WithdrawAsync(Guid id, CancellationToken cancellationToken = default) => ChangeStatusAsync(id, property => property.Withdraw(dateTimeProvider.UtcNow), cancellationToken);

    public Task MarkAsSoldAsync(Guid id, CancellationToken cancellationToken = default) => ChangeStatusAsync(id, property => property.MarkAsSold(dateTimeProvider.UtcNow), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        EnsureCanManage(property);
        property.Delete(dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ChangeStatusAsync(Guid id, Action<Property> change, CancellationToken cancellationToken)
    {
        var property = await GetRequiredAsync(id, cancellationToken);
        EnsureCanManage(property);
        change(property);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Property> GetRequiredAsync(Guid id, CancellationToken cancellationToken)
    {
        var property = await propertyRepository.GetByIdAsync(id, cancellationToken);
        return property ?? throw new KeyNotFoundException($"Property '{id}' was not found.");
    }

    private Guid RequireUserId() => currentUser.UserId
        ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    private void EnsureBrokerOrAdministrator()
    {
        if (!currentUser.IsInRole(UserRole.Broker) && !currentUser.IsInRole(UserRole.Administrator))
        {
            throw new UnauthorizedAccessException("Only brokers and administrators can manage properties.");
        }
    }

    private void EnsureCanManage(Property property)
    {
        if (currentUser.IsInRole(UserRole.Administrator))
        {
            return;
        }

        if (!currentUser.IsInRole(UserRole.Broker) || property.BrokerId != RequireUserId())
        {
            throw new UnauthorizedAccessException("The current user cannot manage this property.");
        }
    }

    private static PropertyDto Map(Property property)
    {
        var address = property.Address is null
            ? null
            : new PropertyAddressDto(
                property.Address.Id,
                property.Address.Street,
                property.Address.StreetNumber,
                property.Address.PostalCode,
                property.Address.City,
                property.Address.Region,
                property.Address.Country,
                property.Address.Coordinates?.Latitude,
                property.Address.Coordinates?.Longitude);

        return new PropertyDto(
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
            address,
            property.Images.Select(image => new PropertyImageDto(image.Id, image.Url, image.AltText, image.SortOrder, image.IsPrimary)).ToArray());
    }
}