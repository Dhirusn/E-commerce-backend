using PaymentService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Data.Models
{
    public class Payment : BaseEntity<Guid>
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        public string? PaymentIntentId { get; set; } // Stripe payment intent ID
        
        public string? TransactionId { get; set; } // External transaction ID
        
        public string? PaymentGateway { get; set; } = "Stripe";
        
        public DateTime? ProcessedAt { get; set; }
        
        public string? FailureReason { get; set; }
        
        public string? PaymentMetadata { get; set; } // JSON string for additional data
        
        public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
        
        public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    }
    
    public enum PaymentMethod
    {
        CreditCard = 0,
        DebitCard = 1,
        PayPal = 2,
        BankTransfer = 3,
        DigitalWallet = 4
    }
    
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5,
        PartiallyRefunded = 6
    }
}
