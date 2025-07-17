using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Data.Models;
using PaymentService.Models;
using SharedLibraries.UserServices;
using Stripe;

namespace PaymentService.Services
{
    public class PaymentService
    {
        private readonly PaymentDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IConfiguration _configuration;

        public PaymentService(PaymentDbContext context, IUserContextService userContextService, ILogger<PaymentService> logger, IConfiguration configuration)
        {
            _context = context;
            _userContextService = userContextService;
            _logger = logger;
            _configuration = configuration;

            // Initialize Stripe
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(CreatePaymentIntentDto createPaymentIntentDto)
        {
            try
            {
                // Build the payment intent create options
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(createPaymentIntentDto.Amount * 100), // Amount in cents
                    Currency = createPaymentIntentDto.Currency,
                    PaymentMethodTypes = new List<string>{ "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "OrderId", createPaymentIntentDto.OrderId.ToString() },
                        { "UserId", _userContextService.UserId },
                        { "PaymentMetadata", createPaymentIntentDto.PaymentMetadata ?? string.Empty }
                    }
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                // Store the payment in DB
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = createPaymentIntentDto.OrderId,
                    UserId = _userContextService.UserId,
                    Amount = createPaymentIntentDto.Amount,
                    Currency = createPaymentIntentDto.Currency,
                    PaymentMethod = createPaymentIntentDto.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    PaymentIntentId = intent.Id,
                    PaymentGateway = "Stripe",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Return response
                return new PaymentIntentResponseDto
                {
                    PaymentIntentId = intent.Id,
                    ClientSecret = intent.ClientSecret,
                    Amount = createPaymentIntentDto.Amount,
                    Currency = createPaymentIntentDto.Currency,
                    Status = intent.Status,
                    PaymentId = payment.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                throw;
            }
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto processPaymentDto)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == processPaymentDto.PaymentId && !p.IsDeleted);

                if (payment == null)
                    throw new InvalidOperationException("Payment not found");

                // Confirm payment if needed
                if (processPaymentDto.ConfirmPayment)
                {
                    var service = new PaymentIntentService();
                    var confirmOptions = new PaymentIntentConfirmOptions
                    {
                        PaymentMethod = processPaymentDto.PaymentMethodId
                    };

                    var intent = await service.ConfirmAsync(processPaymentDto.PaymentIntentId, confirmOptions);

                    // Update payment status
                    payment.Status = intent.Status switch
                    {
                        "succeeded" => PaymentStatus.Completed,
                        "processing" => PaymentStatus.Processing,
                        "requires_payment_method" => PaymentStatus.Failed,
                        _ => payment.Status
                    };

                    payment.TransactionId = intent.LatestChargeId;
                    payment.ProcessedAt = DateTime.UtcNow;

                    // Log a payment transaction
                    var paymentTransaction = new PaymentTransaction
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = payment.Id,
                        TransactionId = payment.TransactionId,
                        Amount = payment.Amount,
                        Currency = payment.Currency,
                        Type = TransactionType.Payment,
                        Status = TransactionStatus.Completed,
                        ProcessedAt = DateTime.UtcNow
                    };

                    _context.PaymentTransactions.Add(paymentTransaction);
                    await _context.SaveChangesAsync();
                }

                return await GetPaymentByIdAsync(payment.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                throw;
            }
        }

        public async Task<PaymentResponseDto> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Transactions)
                .Include(p => p.Refunds)
                .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);

            if (payment == null)
                throw new InvalidOperationException("Payment not found");

            return MapToPaymentResponseDto(payment);
        }

        public async Task<bool> RefundPaymentAsync(CreateRefundDto createRefundDto)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == createRefundDto.PaymentId && !p.IsDeleted);

                if (payment == null || payment.Amount < createRefundDto.Amount)
                    throw new InvalidOperationException("Invalid refund request");

                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = payment.PaymentIntentId,
                    Amount = (long)(createRefundDto.Amount * 100),
                    Reason = createRefundDto.Reason
                };

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions);

                // Record refund transaction
                var refundEntity = new Data.Models.Refund
                {
                    Id = Guid.NewGuid(),
                    PaymentId = payment.Id,
                    Amount = createRefundDto.Amount,
                    Status = refund.Status == "succeeded" ? RefundStatus.Completed : RefundStatus.Failed,
                    RefundId = refund.Id,
                    GatewayResponse = refund.ToJson(),
                    ProcessedAt = DateTime.UtcNow
                };

                _context.Refunds.Add(refundEntity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                throw;
            }
        }

        private PaymentResponseDto MapToPaymentResponseDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentIntentId = payment.PaymentIntentId,
                TransactionId = payment.TransactionId,
                PaymentGateway = payment.PaymentGateway,
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt,
                FailureReason = payment.FailureReason,
                Transactions = payment.Transactions.Select(t => new PaymentTransactionDto
                {
                    Id = t.Id,
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    Currency = t.Currency,
                    Type = t.Type,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    ProcessedAt = t.ProcessedAt,
                    FailureReason = t.FailureReason
                }).ToList(),
                Refunds = payment.Refunds.Select(r => new RefundDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    Status = r.Status,
                    RefundId = r.RefundId,
                    CreatedAt = r.CreatedAt,
                    ProcessedAt = r.ProcessedAt,
                    FailureReason = r.FailureReason
                }).ToList()
            };
        }
    }
}
