using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Auditing;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.Brokers;

public sealed class BrokerAdministrationService(
    IUserRepository userRepository,
    IBrokerProfileRepository brokerRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditService auditService) : IBrokerAdministrationService
{
    public async Task<BrokerDto> CreateAsync(CreateBrokerRequest request, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' was not found.");
        if (await brokerRepository.GetByIdAsync(request.UserId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("The user already has a broker profile.");
        }

        user.ChangeRole(UserRole.Broker, dateTimeProvider.UtcNow);
        var broker = new BrokerProfile(
            Guid.NewGuid(),
            request.UserId,
            request.FullName,
            request.Email,
            request.Phone,
            request.Bio);
        brokerRepository.Add(broker);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditService.RecordAsync("BrokerProfile", broker.Id, "Created", cancellationToken: cancellationToken);
        return Map(broker);
    }

    public async Task<BrokerDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        return Map(await GetRequiredAsync(userId, cancellationToken));
    }

    public async Task<IReadOnlyCollection<BrokerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        var brokers = await brokerRepository.GetAllAsync(cancellationToken);
        return brokers.Select(Map).ToArray();
    }

    public async Task DeactivateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        EnsureAdministrator();
        var broker = await GetRequiredAsync(userId, cancellationToken);
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{userId}' was not found.");
        broker.Deactivate();
        user.Deactivate(dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditService.RecordAsync("BrokerProfile", broker.Id, "Deactivated", cancellationToken: cancellationToken);
    }

    private async Task<BrokerProfile> GetRequiredAsync(Guid userId, CancellationToken cancellationToken) =>
        await brokerRepository.GetByIdAsync(userId, cancellationToken)
        ?? throw new KeyNotFoundException($"Broker profile for user '{userId}' was not found.");

    private void EnsureAdministrator()
    {
        if (!currentUser.IsInRole(UserRole.Administrator))
        {
            throw new UnauthorizedAccessException("Only administrators can manage brokers.");
        }
    }

    private static BrokerDto Map(BrokerProfile broker) => new(
        broker.Id,
        broker.UserId,
        broker.FullName,
        broker.Email,
        broker.Phone,
        broker.Bio,
        broker.IsActive);
}