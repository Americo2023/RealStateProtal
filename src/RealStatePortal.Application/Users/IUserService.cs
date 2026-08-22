using RealStatePortal.Application.Common;

namespace RealStatePortal.Application.Users;

public interface IUserService
{
    Task<Result<IReadOnlyCollection<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
}