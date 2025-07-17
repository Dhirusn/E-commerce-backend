using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Models;
using OrderService.Models;
using SharedLibraries.UserServices;

namespace OrderService.Services
{
    public class OrderService
    {
        private readonly OrderDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(OrderDbContext context, IUserContextService userContextService, ILogger<OrderService> logger)
        {
            _context = context;
            _userContextService = userContextService;
            _logger = logger;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var userId = _userContextService.UserId!;
                var userEmail = _userContextService.Email!;

                // Calculate totals
                var subTotal = createOrderDto.OrderItems.Sum(item => item.UnitPrice * item.Quantity);
                var totalAmount = subTotal + createOrderDto.TaxAmount + createOrderDto.ShippingAmount - createOrderDto.DiscountAmount;

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UserEmail = userEmail,
                    Status = OrderStatus.Pending,
                    SubTotal = subTotal,
                    TaxAmount = createOrderDto.TaxAmount,
                    ShippingAmount = createOrderDto.ShippingAmount,
                    DiscountAmount = createOrderDto.DiscountAmount,
                    TotalAmount = totalAmount,
                    Currency = createOrderDto.Currency,
                    Notes = createOrderDto.Notes,
                    ShippingAddress = new ShippingAddress
                    {
                        FirstName = createOrderDto.ShippingAddress.FirstName,
                        LastName = createOrderDto.ShippingAddress.LastName,
                        AddressLine1 = createOrderDto.ShippingAddress.AddressLine1,
                        AddressLine2 = createOrderDto.ShippingAddress.AddressLine2,
                        City = createOrderDto.ShippingAddress.City,
                        State = createOrderDto.ShippingAddress.State,
                        PostalCode = createOrderDto.ShippingAddress.PostalCode,
                        Country = createOrderDto.ShippingAddress.Country,
                        Phone = createOrderDto.ShippingAddress.Phone
                    },
                    BillingAddress = new BillingAddress
                    {
                        FirstName = createOrderDto.BillingAddress.FirstName,
                        LastName = createOrderDto.BillingAddress.LastName,
                        AddressLine1 = createOrderDto.BillingAddress.AddressLine1,
                        AddressLine2 = createOrderDto.BillingAddress.AddressLine2,
                        City = createOrderDto.BillingAddress.City,
                        State = createOrderDto.BillingAddress.State,
                        PostalCode = createOrderDto.BillingAddress.PostalCode,
                        Country = createOrderDto.BillingAddress.Country,
                        Phone = createOrderDto.BillingAddress.Phone
                    },
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add order items
                foreach (var itemDto in createOrderDto.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = itemDto.ProductId,
                        ProductName = itemDto.ProductName,
                        ProductSku = itemDto.ProductSku,
                        UnitPrice = itemDto.UnitPrice,
                        Quantity = itemDto.Quantity,
                        TotalPrice = itemDto.UnitPrice * itemDto.Quantity,
                        ProductImageUrl = itemDto.ProductImageUrl,
                        ProductAttributes = itemDto.ProductAttributes,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // Add initial status history
                var statusHistory = new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Status = OrderStatus.Pending,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = userId,
                    Notes = "Order created",
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderStatusHistory.Add(statusHistory);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetOrderByIdAsync(order.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null)
                throw new InvalidOperationException("Order not found");

            return MapToOrderResponseDto(order);
        }

        public async Task<List<OrderSummaryDto>> GetUserOrdersAsync(int page = 1, int pageSize = 10)
        {
            var userId = _userContextService.UserId;
            
            var orders = await _context.Orders
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return orders.Select(order => new OrderSummaryDto
            {
                Id = order.Id,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Currency = order.Currency,
                CreatedAt = order.CreatedAt,
                ItemCount = order.OrderItems.Count
            }).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto updateStatusDto)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
            
            if (order == null)
                return false;

            var userId = _userContextService.UserId;
            
            // Update order status
            order.Status = updateStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            // Update specific fields based on status
            switch (updateStatusDto.Status)
            {
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(updateStatusDto.TrackingNumber))
                        order.TrackingNumber = updateStatusDto.TrackingNumber;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
            }

            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Status = updateStatusDto.Status,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = userId,
                Notes = updateStatusDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.OrderStatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, string reason)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
            
            if (order == null || order.Status != OrderStatus.Pending)
                return false;

            var userId = _userContextService.UserId;
            
            // Only allow cancellation if order is pending
            if (order.Status != OrderStatus.Pending)
                return false;

            return await UpdateOrderStatusAsync(orderId, new UpdateOrderStatusDto
            {
                Status = OrderStatus.Cancelled,
                Notes = reason
            });
        }

        private OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserEmail = order.UserEmail,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                SubTotal = order.SubTotal,
                TaxAmount = order.TaxAmount,
                ShippingAmount = order.ShippingAmount,
                DiscountAmount = order.DiscountAmount,
                Currency = order.Currency,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                TrackingNumber = order.TrackingNumber,
                ShippingAddress = new ShippingAddressDto
                {
                    FirstName = order.ShippingAddress.FirstName,
                    LastName = order.ShippingAddress.LastName,
                    AddressLine1 = order.ShippingAddress.AddressLine1,
                    AddressLine2 = order.ShippingAddress.AddressLine2,
                    City = order.ShippingAddress.City,
                    State = order.ShippingAddress.State,
                    PostalCode = order.ShippingAddress.PostalCode,
                    Country = order.ShippingAddress.Country,
                    Phone = order.ShippingAddress.Phone
                },
                BillingAddress = new BillingAddressDto
                {
                    FirstName = order.BillingAddress.FirstName,
                    LastName = order.BillingAddress.LastName,
                    AddressLine1 = order.BillingAddress.AddressLine1,
                    AddressLine2 = order.BillingAddress.AddressLine2,
                    City = order.BillingAddress.City,
                    State = order.BillingAddress.State,
                    PostalCode = order.BillingAddress.PostalCode,
                    Country = order.BillingAddress.Country,
                    Phone = order.BillingAddress.Phone
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductSku = item.ProductSku,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    ProductImageUrl = item.ProductImageUrl,
                    ProductAttributes = item.ProductAttributes
                }).ToList(),
                StatusHistory = order.StatusHistory.Select(history => new OrderStatusHistoryDto
                {
                    Id = history.Id,
                    Status = history.Status,
                    ChangedAt = history.ChangedAt,
                    ChangedBy = history.ChangedBy,
                    Notes = history.Notes
                }).OrderBy(h => h.ChangedAt).ToList()
            };
        }
    }
}
