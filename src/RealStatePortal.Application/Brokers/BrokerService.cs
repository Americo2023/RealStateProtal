using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Common;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Brokers;

public sealed class BrokerService(
    IBrokerRepository brokerRepository,
    IUnitOfWork unitOfWork,
    IAuditLogRepository auditLogRepository,
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider) : IBrokerService
{
    public async Task<Result<IReadOnlyCollection<BrokerDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var brokers = await brokerRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<BrokerDto>>.Success(brokers.Select(broker => new BrokerDto(
            broker.Id,
            broker.UserId,
            broker.FullName,
            broker.Email,
            broker.Phone,
            broker.Bio,
            broker.IsActive)).ToArray());
    }

    public async Task<Result<BrokerDto>> UpdateAsync(Guid brokerId, UpdateBrokerRequest request, CancellationToken cancellationToken = default)
    {
        var broker = await brokerRepository.GetByIdAsync(brokerId, cancellationToken);
        if (broker is null)
        {
            return Result<BrokerDto>.Failure("Broker was not found.");
        }

        broker.Update(request.FullName, request.Email, request.Phone, request.Bio);
        broker.SetActive(request.IsActive);
        await auditLogRepository.AddAsync(
            new AuditLog(
                "BrokerProfile",
                broker.Id,
                "Updated",
                currentUserService.UserId,
                dateTimeProvider.UtcNow,
                $"Updated broker profile and active status to {broker.IsActive}."),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<BrokerDto>.Success(new BrokerDto(
            broker.Id,
            broker.UserId,
            broker.FullName,
            broker.Email,
            broker.Phone,
            broker.Bio,
            broker.IsActive));
    }
}