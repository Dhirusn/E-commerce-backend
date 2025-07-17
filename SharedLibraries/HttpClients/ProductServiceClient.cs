using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace SharedLibraries.HttpClients
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductServiceClient> _logger;
        private readonly ServiceEndpoints _serviceEndpoints;

        public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger, IOptions<ServiceEndpoints> serviceEndpoints)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serviceEndpoints = serviceEndpoints.Value;
            _httpClient.BaseAddress = new Uri(_serviceEndpoints.ProductService);
        }

        public async Task<ProductClientDto?> GetProductAsync(Guid productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/GetById/{productId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResult<ProductClientDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId}", productId);
                return null;
            }
        }

        public async Task<List<ProductClientDto>> GetProductsAsync(List<Guid> productIds)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/products/GetByIds", productIds);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResult<List<ProductClientDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Data ?? new List<ProductClientDto>();
                }
                return new List<ProductClientDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return new List<ProductClientDto>();
            }
        }

        public async Task<bool> CheckInventoryAsync(Guid productId, int quantity)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/{productId}/inventory/check?quantity={quantity}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking inventory for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> ReserveInventoryAsync(Guid productId, int quantity, Guid orderId)
        {
            try
            {
                var request = new ReserveInventoryRequest
                {
                    ProductId = productId,
                    Quantity = quantity,
                    OrderId = orderId
                };

                var response = await _httpClient.PostAsJsonAsync("/api/products/inventory/reserve", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving inventory for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<bool> ReleaseInventoryAsync(Guid productId, int quantity, Guid orderId)
        {
            try
            {
                var request = new ReserveInventoryRequest
                {
                    ProductId = productId,
                    Quantity = quantity,
                    OrderId = orderId
                };

                var response = await _httpClient.PostAsJsonAsync("/api/products/inventory/release", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing inventory for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<ProductInventoryDto?> GetInventoryAsync(Guid productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/{productId}/inventory");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResult<ProductInventoryDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory for product {ProductId}", productId);
                return null;
            }
        }
    }

    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public class ServiceEndpoints
    {
        public string ProductService { get; set; } = "https://localhost:7001";
        public string CartService { get; set; } = "https://localhost:7002";
        public string OrderService { get; set; } = "https://localhost:7003";
        public string PaymentService { get; set; } = "https://localhost:7004";
        public string AuthService { get; set; } = "https://localhost:7005";
    }
}
