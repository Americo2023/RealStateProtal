using RealStatePortal.Application.Common;

namespace RealStatePortal.Application.Brokers;

public interface IBrokerService
{
    Task<Result<IReadOnlyCollection<BrokerDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<BrokerDto>> UpdateAsync(Guid brokerId, UpdateBrokerRequest request, CancellationToken cancellationToken = default);
}