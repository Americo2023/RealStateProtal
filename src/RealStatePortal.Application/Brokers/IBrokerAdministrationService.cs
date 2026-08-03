namespace RealStatePortal.Application.Brokers;

public interface IBrokerAdministrationService
{
    Task<BrokerDto> CreateAsync(CreateBrokerRequest request, CancellationToken cancellationToken = default);
    Task<BrokerDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<BrokerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid userId, CancellationToken cancellationToken = default);
}