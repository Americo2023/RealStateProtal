using RealStatePortal.Application.Common;
using RealStatePortal.Application.Properties.Dtos;

namespace RealStatePortal.Application.Properties.Services;

public interface IPropertyService
{
    Task<Result<PropertyDto>> GetByIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PropertyDto>>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PropertyDto>>> SearchAsync(string? query, CancellationToken cancellationToken = default);
    Task<Result<PropertyDto>> CreateAsync(CreatePropertyRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid propertyId, UpdatePropertyRequest request, CancellationToken cancellationToken = default);
    Task<Result> PublishAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Result> WithdrawAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Result> MarkAsSoldAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid propertyId, CancellationToken cancellationToken = default);
}