using PaymentService.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.Models
{
    public class CreatePaymentIntentDto
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        public string? PaymentMetadata { get; set; }
    }
    
    public class ProcessPaymentDto
    {
        [Required]
        public Guid PaymentId { get; set; }
        
        [Required]
        public string PaymentIntentId { get; set; }
        
        public string? PaymentMethodId { get; set; }
        
        public bool ConfirmPayment { get; set; } = true;
    }
    
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentGateway { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
        public List<PaymentTransactionDto> Transactions { get; set; } = new();
        public List<RefundDto> Refunds { get; set; } = new();
    }
    
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
    }
    
    public class RefundDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public RefundStatus Status { get; set; }
        public string? RefundId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
    }
    
    public class CreateRefundDto
    {
        [Required]
        public Guid PaymentId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public string? Reason { get; set; }
    }
    
    public class PaymentIntentResponseDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
    }
    
    public class PaymentSummaryDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
    
    public class WebhookEventDto
    {
        public string EventType { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? FailureReason { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
    }
}
