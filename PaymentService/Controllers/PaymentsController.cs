using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService.Services.PaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(PaymentService.Services.PaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("create-intent")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto createPaymentIntentDto)
        {
            try
            {
                var intentResponse = await _paymentService.CreatePaymentIntentAsync(createPaymentIntentDto);
                return Ok(intentResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("process")]
        [Authorize]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto processPaymentDto)
        {
            try
            {
                var paymentResponse = await _paymentService.ProcessPaymentAsync(processPaymentDto);
                return Ok(paymentResponse);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Invalid payment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                return Ok(payment);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refund")]
        [Authorize]
        public async Task<IActionResult> RefundPayment([FromBody] CreateRefundDto createRefundDto)
        {
            try
            {
                var result = await _paymentService.RefundPaymentAsync(createRefundDto);
                if (!result)
                    return BadRequest("Invalid refund request");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
