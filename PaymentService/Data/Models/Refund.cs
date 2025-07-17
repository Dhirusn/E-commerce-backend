using PaymentService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Data.Models
{
    public class Refund : BaseEntity<Guid>
    {
        [Required]
        public Guid PaymentId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public RefundStatus Status { get; set; }
        
        public string? RefundId { get; set; } // External refund ID
        
        public string? GatewayResponse { get; set; } // JSON string for additional data
        
        public DateTime? ProcessedAt { get; set; }
        
        public string? FailureReason { get; set; }
        
        // Navigation properties
        public Payment Payment { get; set; } = null!;
    }
    
    public enum RefundStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}
