using PaymentService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Data.Models
{
    public class PaymentTransaction : BaseEntity<Guid>
    {
        [Required]
        public Guid PaymentId { get; set; }
        
        [Required]
        public string TransactionId { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public TransactionType Type { get; set; }
        
        [Required]
        public TransactionStatus Status { get; set; }
        
        public string? GatewayResponse { get; set; } // JSON string
        
        public DateTime? ProcessedAt { get; set; }
        
        public string? FailureReason { get; set; }
        
        // Navigation properties
        public Payment Payment { get; set; } = null!;
    }
    
    public enum TransactionType
    {
        Payment = 0,
        Refund = 1,
        Chargeback = 2
    }
    
    public enum TransactionStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}
