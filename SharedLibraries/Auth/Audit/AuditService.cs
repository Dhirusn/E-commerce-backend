using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Shared.Library.Audit
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;

        public AuditService(ILogger<AuditService> logger) => _logger = logger;

        public Task RecordEventAsync(string category, string action, string? performedBy = null, string? data = null)
        {
            _logger.LogInformation("AUDIT | Category: {Category} | Action: {Action} | By: {By} | Data: {Data}",
                category, action, performedBy ?? "-", data ?? "-");
            return Task.CompletedTask;
        }
    }
}
