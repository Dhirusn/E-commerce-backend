# Mixed Communication Pattern - Usage Examples

## Overview
This document shows practical examples of how to use the mixed communication pattern in your E-commerce system and when to use each approach.

## 1. Direct HTTP Communication (Synchronous)

### When to Use:
- **Real-time operations** that need immediate response
- **Critical validations** before proceeding
- **Data that must be current** (inventory, pricing)
- **User-facing operations** that need instant feedback

### Example Scenarios:

#### A. Adding Product to Cart (CartService → ProductService)
```csharp
public async Task<CartResponseDto> AddProductToCartAsync(Guid cartId, AddToCartDto addToCartDto)
{
    // 1. DIRECT HTTP: Get current product info (price, availability)
    var product = await _productServiceClient.GetProductAsync(addToCartDto.ProductId);
    if (product == null)
    {
        throw new InvalidOperationException("Product not found");
    }

    // 2. DIRECT HTTP: Check real-time inventory
    var hasStock = await _productServiceClient.CheckInventoryAsync(
        addToCartDto.ProductId, 
        addToCartDto.Quantity
    );
    
    if (!hasStock)
    {
        throw new InvalidOperationException("Insufficient stock");
    }

    // 3. Add to cart using current price
    var cartItem = new CartItem
    {
        ProductId = product.Id,
        ProductName = product.Title,
        UnitPrice = product.Price, // Use current price
        Quantity = addToCartDto.Quantity,
        IsAvailable = true
    };

    // Save to database
    await _cartService.AddItemToCartAsync(cartId, cartItem);

    // 4. PUBLISH EVENT: Notify other services
    await _eventBus.PublishAsync(new CartItemAddedEvent
    {
        CartId = cartId,
        ProductId = product.Id,
        Quantity = addToCartDto.Quantity,
        UserId = _userContextService.GetUserId()
    });

    return await GetCartByIdAsync(cartId);
}
```

#### B. Order Creation (OrderService → CartService → ProductService)
```csharp
public async Task<OrderResponseDto> CreateOrderFromCartAsync(Guid cartId)
{
    // 1. DIRECT HTTP: Get cart details
    var cart = await _cartServiceClient.GetCartAsync(cartId);
    if (cart == null)
    {
        throw new InvalidOperationException("Cart not found");
    }

    // 2. DIRECT HTTP: Reserve inventory for each item
    var reservationTasks = cart.CartItems.Select(async item =>
    {
        var reserved = await _productServiceClient.ReserveInventoryAsync(
            item.ProductId, 
            item.Quantity, 
            Guid.NewGuid() // Future order ID
        );
        return new { item.ProductId, item.Quantity, Reserved = reserved };
    });

    var reservationResults = await Task.WhenAll(reservationTasks);
    
    // Check if all items were reserved
    if (reservationResults.Any(r => !r.Reserved))
    {
        throw new InvalidOperationException("Could not reserve all items");
    }

    // 3. Create order
    var order = await _orderService.CreateOrderAsync(cart);

    // 4. PUBLISH EVENT: Notify cart conversion
    await _eventBus.PublishAsync(new OrderCreatedEvent
    {
        OrderId = order.Id,
        CartId = cartId,
        UserId = cart.UserId,
        TotalAmount = order.TotalAmount,
        Items = order.OrderItems.Select(oi => new OrderItemEvent
        {
            ProductId = oi.ProductId,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice
        }).ToList()
    });

    return order;
}
```

## 2. Event-Driven Communication (Asynchronous)

### When to Use:
- **Non-critical updates** that can be processed later
- **Notifications** to multiple services
- **Data synchronization** between services
- **Business events** that trigger workflows

### Example Scenarios:

#### A. Product Price/Inventory Updates
```csharp
// ProductService - Publishing Events
public async Task UpdateProductAsync(Guid productId, UpdateProductDto updateDto)
{
    var product = await _productRepository.GetByIdAsync(productId);
    
    // Store old values for comparison
    var oldPrice = product.Price;
    var oldStock = product.Stock;
    
    // Update product
    product.Price = updateDto.Price;
    product.Stock = updateDto.Stock;
    await _productRepository.UpdateAsync(product);

    // PUBLISH EVENT: Notify other services about changes
    await _eventBus.PublishAsync(new ProductUpdatedEvent
    {
        ProductId = productId,
        ProductName = product.Title,
        ProductSku = product.SKU,
        Price = updateDto.Price,
        Stock = updateDto.Stock,
        IsAvailable = updateDto.Stock > 0,
        ImageUrl = product.ImageUrl
    });
}

// CartService - Event Handler
public class CartEventHandler : IEventHandler<ProductUpdatedEvent>
{
    public async Task HandleAsync(ProductUpdatedEvent eventData)
    {
        // Update all cart items with this product
        var cartItems = await _cartRepository.GetCartItemsByProductIdAsync(eventData.ProductId);
        
        foreach (var cartItem in cartItems)
        {
            // Update price if changed
            if (cartItem.UnitPrice != eventData.Price)
            {
                cartItem.UnitPrice = eventData.Price;
                cartItem.TotalPrice = eventData.Price * cartItem.Quantity;
            }
            
            // Update availability
            cartItem.IsAvailable = eventData.IsAvailable;
            cartItem.AvailableStock = eventData.Stock;
            
            await _cartRepository.UpdateCartItemAsync(cartItem);
        }
    }
}
```

#### B. Order Status Changes
```csharp
// OrderService - Publishing Status Changes
public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
{
    var order = await _orderRepository.GetByIdAsync(orderId);
    var oldStatus = order.Status;
    
    order.Status = newStatus;
    await _orderRepository.UpdateAsync(order);

    // PUBLISH EVENT: Notify status change
    await _eventBus.PublishAsync(new OrderStatusChangedEvent
    {
        OrderId = orderId,
        UserId = order.UserId,
        OldStatus = oldStatus.ToString(),
        NewStatus = newStatus.ToString()
    });

    // PUBLISH SPECIFIC EVENTS based on status
    switch (newStatus)
    {
        case OrderStatus.Shipped:
            await _eventBus.PublishAsync(new OrderShippedEvent
            {
                OrderId = orderId,
                UserId = order.UserId,
                TrackingNumber = order.TrackingNumber
            });
            break;
            
        case OrderStatus.Delivered:
            await _eventBus.PublishAsync(new OrderDeliveredEvent
            {
                OrderId = orderId,
                UserId = order.UserId
            });
            break;
            
        case OrderStatus.Cancelled:
            // Release inventory
            await _eventBus.PublishAsync(new OrderCancelledEvent
            {
                OrderId = orderId,
                UserId = order.UserId,
                Items = order.OrderItems.Select(oi => new OrderItemEvent
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()
            });
            break;
    }
}

// ProductService - Event Handler for Order Cancellation
public class ProductEventHandler : IEventHandler<OrderCancelledEvent>
{
    public async Task HandleAsync(OrderCancelledEvent eventData)
    {
        // Release reserved inventory
        foreach (var item in eventData.Items)
        {
            await _productService.ReleaseInventoryAsync(
                item.ProductId, 
                item.Quantity, 
                eventData.OrderId
            );
        }
    }
}
```

## 3. API Gateway Pattern

### When to Use:
- **Client-facing operations** (mobile apps, web apps)
- **Request routing** to appropriate services
- **Authentication and authorization**
- **Response aggregation** from multiple services

### Example Implementation:

#### A. API Gateway Routes Configuration
```csharp
// ApiGateway - Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure routes
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        // Add authentication
        await next();
    });
});

// Custom aggregated endpoints
app.MapGet("/api/dashboard/{userId}", async (string userId, IServiceProvider services) =>
{
    var cartClient = services.GetRequiredService<ICartServiceClient>();
    var orderClient = services.GetRequiredService<IOrderServiceClient>();
    
    // Aggregate data from multiple services
    var cartTask = cartClient.GetUserCartAsync(userId);
    var ordersTask = orderClient.GetUserOrdersAsync(userId);
    
    await Task.WhenAll(cartTask, ordersTask);
    
    return new DashboardDto
    {
        Cart = await cartTask,
        RecentOrders = await ordersTask
    };
});
```

#### B. Client Usage
```typescript
// Frontend - React/Angular example
const api = {
  // Through API Gateway
  async getCart(userId: string) {
    return await fetch(`${API_GATEWAY_URL}/api/carts/user/${userId}`);
  },
  
  async addToCart(cartId: string, productId: string, quantity: number) {
    return await fetch(`${API_GATEWAY_URL}/api/carts/${cartId}/items`, {
      method: 'POST',
      body: JSON.stringify({ productId, quantity })
    });
  },
  
  async getDashboard(userId: string) {
    // This calls the aggregated endpoint
    return await fetch(`${API_GATEWAY_URL}/api/dashboard/${userId}`);
  }
};
```

## 4. Complete Flow Example: "Add to Cart and Checkout"

### Step 1: Add Product to Cart
```csharp
// Client Request → API Gateway → CartService
[HttpPost("{cartId}/items")]
public async Task<IActionResult> AddToCart(Guid cartId, AddToCartDto dto)
{
    // 1. DIRECT HTTP: Validate product and get current price
    var product = await _productServiceClient.GetProductAsync(dto.ProductId);
    if (product == null) return BadRequest("Product not found");
    
    // 2. DIRECT HTTP: Check inventory
    var hasStock = await _productServiceClient.CheckInventoryAsync(dto.ProductId, dto.Quantity);
    if (!hasStock) return BadRequest("Insufficient stock");
    
    // 3. Add to cart
    var result = await _cartService.AddItemToCartAsync(cartId, dto);
    
    // 4. PUBLISH EVENT: Notify that item was added
    await _eventBus.PublishAsync(new CartItemAddedEvent
    {
        CartId = cartId,
        ProductId = dto.ProductId,
        Quantity = dto.Quantity,
        UserId = _userContextService.GetUserId()
    });
    
    return Ok(result);
}
```

### Step 2: Checkout Process
```csharp
// OrderService - Checkout with Saga Pattern
public async Task<OrderResponseDto> CheckoutAsync(Guid cartId)
{
    var saga = new CheckoutSaga(_eventBus, _productServiceClient, _paymentServiceClient);
    
    try
    {
        // 1. DIRECT HTTP: Get cart
        var cart = await _cartServiceClient.GetCartAsync(cartId);
        
        // 2. DIRECT HTTP: Reserve inventory
        await saga.ReserveInventoryAsync(cart.CartItems);
        
        // 3. DIRECT HTTP: Process payment
        var payment = await saga.ProcessPaymentAsync(cart);
        
        // 4. Create order
        var order = await CreateOrderAsync(cart);
        
        // 5. PUBLISH EVENT: Order created
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            CartId = cartId,
            UserId = cart.UserId
        });
        
        return order;
    }
    catch (Exception ex)
    {
        // Compensate (rollback)
        await saga.CompensateAsync();
        throw;
    }
}
```

## 5. Configuration and Setup

### A. Service Configuration
```csharp
// CartService - Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add HTTP clients for direct communication
builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>();

// Add RabbitMQ for event-driven communication
builder.Services.Configure<RabbitMQOptions>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// Register event handlers
builder.Services.AddScoped<IEventHandler<ProductUpdatedEvent>, ProductEventHandler>();
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderEventHandler>();

var app = builder.Build();

// Subscribe to events
var eventBus = app.Services.GetRequiredService<IEventBus>();
var productEventHandler = app.Services.GetRequiredService<IEventHandler<ProductUpdatedEvent>>();

eventBus.Subscribe<ProductUpdatedEvent>(productEventHandler.HandleAsync);
```

### B. appsettings.json Configuration
```json
{
  "ServiceEndpoints": {
    "ProductService": "https://localhost:7001",
    "CartService": "https://localhost:7002",
    "OrderService": "https://localhost:7003",
    "PaymentService": "https://localhost:7004"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "ecommerce_events"
  }
}
```

## 6. Decision Matrix: When to Use Each Pattern

| Scenario | Direct HTTP | Event-Driven | API Gateway |
|----------|-------------|--------------|-------------|
| **Adding to Cart** | ✅ Check inventory | ✅ Notify addition | ✅ Client endpoint |
| **Price Updates** | ❌ Too slow | ✅ Async updates | ❌ Not needed |
| **Order Creation** | ✅ Reserve inventory | ✅ Notify creation | ✅ Client endpoint |
| **Inventory Low** | ❌ Not urgent | ✅ Notifications | ❌ Not needed |
| **User Dashboard** | ✅ Real-time data | ❌ Too slow | ✅ Aggregate data |
| **Order Tracking** | ✅ Current status | ✅ Status changes | ✅ Client endpoint |

## 7. Best Practices

### A. Error Handling
```csharp
// Resilient HTTP calls with retry
public async Task<T> CallWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
        }
    }
    
    throw new InvalidOperationException("Max retries exceeded");
}
```

### B. Event Idempotency
```csharp
public async Task HandleAsync(ProductUpdatedEvent eventData)
{
    // Check if event was already processed
    var isProcessed = await _eventLogRepository.IsEventProcessedAsync(eventData.ProductId, eventData.UpdatedAt);
    if (isProcessed) return;
    
    // Process event
    await UpdateCartItemsAsync(eventData);
    
    // Mark as processed
    await _eventLogRepository.MarkAsProcessedAsync(eventData.ProductId, eventData.UpdatedAt);
}
```

This mixed approach gives you:
- **Performance** for critical operations
- **Scalability** for non-critical updates
- **Resilience** through loose coupling
- **Flexibility** to choose the right pattern for each scenario
