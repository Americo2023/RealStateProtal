using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealStatePortal.Application.Auditing;

namespace RealStatePortal.Api.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize(Policy = "Administrator")]
public sealed class AuditController(IAuditService auditService) : ControllerBase
{
    [HttpGet("{entityName}/{entityId:guid}")]
    public Task<IReadOnlyCollection<AuditLogDto>> GetByEntity(
        string entityName,
        Guid entityId,
        CancellationToken cancellationToken) =>
        auditService.GetByEntityAsync(entityName, entityId, cancellationToken);
}