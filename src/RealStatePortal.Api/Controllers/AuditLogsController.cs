using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Auditing;

namespace RealStatePortal.Api.Controllers;

[Route("api/audit-logs")]
[Authorize(Policy = "Administrator")]
public sealed class AuditLogsController(IAuditLogService auditLogService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        FromResult(await auditLogService.GetAllAsync(cancellationToken));
}