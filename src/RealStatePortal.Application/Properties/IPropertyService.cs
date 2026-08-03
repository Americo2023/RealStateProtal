namespace RealStatePortal.Application.Properties;

public interface IPropertyService
{
    Task<PropertyDto> CreateAsync(CreatePropertyRequest request, CancellationToken cancellationToken = default);
    Task<PropertyDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PropertyDto>> SearchAsync(PropertySearchCriteria criteria, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken cancellationToken = default);
    Task TransferAsync(Guid id, Guid brokerId, CancellationToken cancellationToken = default);
    Task SetAddressAsync(Guid id, SetPropertyAddressRequest request, CancellationToken cancellationToken = default);
    Task AddImageAsync(Guid id, AddPropertyImageRequest request, CancellationToken cancellationToken = default);
    Task RemoveImageAsync(Guid id, Guid imageId, CancellationToken cancellationToken = default);
    Task PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task WithdrawAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAsSoldAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}