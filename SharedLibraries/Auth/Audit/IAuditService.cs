using System.Threading.Tasks;

namespace Shared.Library.Audit
{
    public interface IAuditService
    {
        Task RecordEventAsync(string category, string action, string? performedBy = null, string? data = null);
    }
}
