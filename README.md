# E-Commerce Backend System

A complete microservices-based e-commerce backend built with .NET, featuring an API Gateway, authentication, and four core services.

## 🏗️ System Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌────────────────────────┐
│   Client        │ ── │   API Gateway    │ ── │   Microservices        │
│   (Frontend)    │    │   (YARP)         │    │                        │
└─────────────────┘    └──────────────────┘    └────────────────────────┘
                              │                         │
                ┌─────────────┼─────────────┐   ┌───────┼───────┐
                │             │             │   │       │       │
        ┌───────▼───────┐ ┌───▼───┐ ┌───────▼─┐ ┌▼────┐┌▼────┐┌▼────┐┌▼────┐
        │   Auth0       │ │ Logging│ │ Config  │ │Product│Order│ Cart │Payment│
        │ Authentication│ │        │ │         │ │Service│Service│Service│Service│
        └───────────────┘ └────────┘ └─────────┘ └──────┘└─────┘└──────┘└──────┘
```

## 📦 Services Overview

### 🔐 API Gateway
- **Port**: 7260
- **Technology**: .NET 9 + YARP (Yet Another Reverse Proxy)
- **Features**:
  - Request routing and load balancing
  - JWT authentication with Auth0
  - Role-based authorization
  - Path transformation
  - Centralized logging

### 🛍️ Products Service
- **Port**: 7273
- **Endpoint**: `https://localhost:7273/api/products`
- **Key Features**:
  - Product catalog management
  - Inventory tracking
  - Product search and filtering
  - Category management
- **API Examples**:
  - `GET /api/products/getall` - Get all products
  - `GET /api/products/{id}` - Get product by ID
  - `POST /api/products` - Create new product (Admin)
  - `PUT /api/products/{id}` - Update product (Admin)

### 📦 Orders Service
- **Port**: 7274
- **Endpoint**: `https://localhost:7274/api/orders`
- **Security**: Admin-only access
- **Key Features**:
  - Order creation and processing
  - Order status tracking
  - Order history
  - Admin order management
- **API Examples**:
  - `GET /api/orders` - Get all orders (Admin)
  - `GET /api/orders/{id}` - Get order details
  - `POST /api/orders` - Create new order
  - `PUT /api/orders/{id}/status` - Update order status (Admin)

### 🛒 Cart Service
- **Port**: 7275
- **Endpoint**: `https://localhost:7275/api/cart`
- **Key Features**:
  - Shopping cart management
  - Add/remove items
  - Cart persistence
  - Price calculations
- **API Examples**:
  - `GET /api/cart/{userId}` - Get user cart
  - `POST /api/cart/{userId}/items` - Add item to cart
  - `DELETE /api/cart/{userId}/items/{itemId}` - Remove item from cart
  - `PUT /api/cart/{userId}/items/{itemId}` - Update item quantity

### 💳 Payments Service
- **Port**: 7276
- **Endpoint**: `https://localhost:7276/api/payments`
- **Key Features**:
  - Payment processing
  - Payment status tracking
  - Transaction history
  - Refund processing
- **API Examples**:
  - `POST /api/payments/process` - Process payment
  - `GET /api/payments/{transactionId}` - Get payment status
  - `POST /api/payments/{transactionId}/refund` - Process refund

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK or later
- Docker (optional, for containerized deployment)
- Git

### Installation & Setup

1. **Clone the Repository**
```bash
git clone https://github.com/yourusername/ecommerce-backend.git
cd ecommerce-backend
```

2. **Setup Auth Application**
   - Create an Jwt Secret
   - Create a new "Regular Web Application"

3. **Configure Environment Variables**

Create `appsettings.Development.json` in each service directory:

**API Gateway:**
```json
{
   "Jwt": {
    "Issuer": "https://localhost:5001/",
    "Audience": "product_api",
    "Secret": "ZKLCSDGJKAJSFKLDJSADFJLLJGDSFDJFGKLS",
    "AccessTokenExpirationMinutes": 1500,
    "RefreshTokenExpirationDays": 3000
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Microservices** (similar structure, adjust ports and database connections)

4. **Run the Services**

**Option 1: Run Individually**
```bash
# Terminal 1 - Products Service
cd product-service
dotnet run --urls="https://localhost:7273"

# Terminal 2 - Orders Service
cd order-service
dotnet run --urls="https://localhost:7274"

# Terminal 3 - Cart Service
cd cart-service
dotnet run --urls="https://localhost:7275"

# Terminal 4 - Payments Service
cd payment-service
dotnet run --urls="https://localhost:7276"

# Terminal 5 - API Gateway
cd api-gateway
dotnet run
```

**Option 2: Docker Compose**
```bash
docker-compose up --build
```

## 🔧 Configuration

### API Gateway Configuration

The gateway routes requests based on this configuration:

```json
{
  "ReverseProxy": {
    "Routes": {
      "products": {
        "ClusterId": "productCluster",
        "Match": { "Path": "/products/{**catch-all}" },
        "Transforms": [{ "PathPattern": "/api/products/{**catch-all}" }]
      },
      "orders": {
        "ClusterId": "orderCluster",
        "Match": { "Path": "/orders/{**catch-all}" },
        "AuthorizationPolicy": "AdminOnly",
        "Transforms": [{ "PathPattern": "/api/orders/{**catch-all}" }]
      },
      "cart": {
        "ClusterId": "cartCluster",
        "Match": { "Path": "/cart/{**catch-all}" },
        "Transforms": [{ "PathPattern": "/api/cart/{**catch-all}" }]
      },
      "payments": {
        "ClusterId": "paymentCluster",
        "Match": { "Path": "/payments/{**catch-all}" },
        "Transforms": [{ "PathPattern": "/api/payments/{**catch-all}" }]
      }
    },
    "Clusters": {
      "productCluster": { "Destinations": { "product1": { "Address": "https://localhost:7273" } } },
      "orderCluster": { "Destinations": { "order1": { "Address": "https://localhost:7274" } } },
      "cartCluster": { "Destinations": { "cart1": { "Address": "https://localhost:7275" } } },
      "paymentCluster": { "Destinations": { "pay1": { "Address": "https://localhost:7276" } } }
    }
  }
}
```

### Environment Variables

| `ConnectionStrings__Default` | Database connection string | `Server=localhost;Database=...` |

## 🔐 Authentication & Authorization

### JWT Token Structure

The system uses JWT tokens with custom claims:

```json
{
  "name": "John Doe",
  "https://myapp.com/roles": ["admin", "user"],
  "sub": "auth0|123456",
  "iss": "https://dev-zn7kooyuqtsoiajl.us.auth0.com/",
  "aud": "https://myapp.com/roles"
}
```

### Role-Based Access

| Service | Endpoint | Required Role |
|---------|----------|---------------|
| Products | Read operations | `user` |
| Products | Write operations | `admin` |
| Orders | All operations | `admin` |
| Cart | All operations | `user` |
| Payments | All operations | `user` |


## 📡 API Documentation

### Gateway Endpoints

All requests go through the API Gateway: `https://localhost:7260`

| Service | Gateway Path | Internal Path | Method | Auth |
|---------|--------------|---------------|--------|------|
| Products | `/products/getall` | `/api/products/getall` | GET | ✅ |
| Products | `/products/{id}` | `/api/products/{id}` | GET | ✅ |
| Products | `/products` | `/api/products` | POST | Admin |
| Orders | `/orders` | `/api/orders` | GET | Admin |
| Orders | `/orders` | `/api/orders` | POST | Admin |
| Cart | `/cart/{userId}` | `/api/cart/{userId}` | GET | ✅ |
| Cart | `/cart/{userId}/items` | `/api/cart/{userId}/items` | POST | ✅ |
| Payments | `/payments/process` | `/api/payments/process` | POST | ✅ |

### Example Requests

```bash
# Get all products
curl -H "Authorization: Bearer {token}" \
  https://localhost:7260/products/getall

# Create new order (Admin only)
curl -X POST -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{"userId": "123", "items": [...]}' \
  https://localhost:7260/orders

# Add item to cart
curl -X POST -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"productId": "456", "quantity": 2}' \
  https://localhost:7260/cart/123/items
```

## 🐳 Docker Deployment

### Docker Compose Setup

```yaml
version: '3.8'
services:
  api-gateway:
    build: ./api-gateway
    ports:
      - "7260:80"
    environment:
      - Auth0__Domain=${AUTH0_DOMAIN}
      - Auth0__ClientId=${AUTH0_CLIENT_ID}
      - Auth0__ClientSecret=${AUTH0_CLIENT_SECRET}
    depends_on:
      - product-service
      - order-service
      - cart-service
      - payment-service

  product-service:
    build: ./product-service
    environment:
      - ConnectionStrings__Default=${PRODUCT_DB_CONNECTION}
    ports:
      - "7273:80"

  order-service:
    build: ./order-service
    environment:
      - ConnectionStrings__Default=${ORDER_DB_CONNECTION}
    ports:
      - "7274:80"

  cart-service:
    build: ./cart-service
    environment:
      - ConnectionStrings__Default=${CART_DB_CONNECTION}
    ports:
      - "7275:80"

  payment-service:
    build: ./payment-service
    environment:
      - ConnectionStrings__Default=${PAYMENT_DB_CONNECTION}
    ports:
      - "7276:80"
```

### Environment File (.env)

```env
AUTH0_DOMAIN=your-auth0-domain
AUTH0_CLIENT_ID=your-client-id
AUTH0_CLIENT_SECRET=your-client-secret
PRODUCT_DB_CONNECTION=Server=db;Database=products;...
ORDER_DB_CONNECTION=Server=db;Database=orders;...
```

### Deployment Commands

```bash
# Build and start all services
docker-compose up --build

# Start in detached mode
docker-compose up -d

# View logs
docker-compose logs -f

# Scale services
docker-compose up --scale product-service=3
```

## 🗄️ Database Setup

Each service has its own database:

### Products Service
- **Database**: SQL Server/PostgreSQL
- **Tables**: Products, Categories, Inventory
- **Migrations**: `dotnet ef database update`

### Orders Service
- **Database**: SQL Server/PostgreSQL
- **Tables**: Orders, OrderItems, OrderHistory
- **Migrations**: `dotnet ef database update`

### Cart Service
- **Database**: Redis/SQL Server
- **Tables**: Carts, CartItems
- **Migrations**: `dotnet ef database update`

### Payments Service
- **Database**: SQL Server/PostgreSQL
- **Tables**: Payments, Transactions, Refunds
- **Migrations**: `dotnet ef database update`

## 🧪 Testing

### Unit Tests

```bash
# Run tests for all services
dotnet test

# Run tests for specific service
cd product-service
dotnet test

# With coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests

```bash
# Test API Gateway routing
curl -H "Authorization: Bearer {token}" https://localhost:7260/products/getall

# Test authentication
curl https://localhost:7260/products/getall  # Should return 401

# Test authorization
curl -H "Authorization: Bearer {user-token}" https://localhost:7260/orders  # Should return 403
```

### Load Testing

```bash
# Using Apache Bench
ab -n 1000 -c 10 -H "Authorization: Bearer {token}" https://localhost:7260/products/getall
```

## 📊 Monitoring & Logging

### Structured Logging

Each service uses structured logging with Serilog:

```json
{
  "Timestamp": "2023-01-01T12:00:00Z",
  "Level": "Information",
  "Message": "Product retrieved",
  "Service": "ProductService",
  "ProductId": "123",
  "Duration": 45
}
```

### Health Checks

All services include health check endpoints:

```bash
# Check service health
curl https://localhost:7273/health
curl https://localhost:7260/health  # Gateway health
```

## 🔒 Security Features

- JWT token validation with Auth0
- Role-based authorization
- HTTPS enforcement
- CORS configuration
- Input validation
- SQL injection protection
- XSS protection headers

## 🚀 Production Deployment

### Cloud Deployment (Azure Example)

```bash
# Deploy to Azure App Service
az webapp up --name ecommerce-gateway --resource-group myResourceGroup
az webapp up --name product-service --resource-group myResourceGroup
# ... repeat for other services
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-gateway
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: gateway
        image: myregistry/api-gateway:latest
        env:
        - name: Auth0__Domain
          valueFrom:
            secretKeyRef:
              name: auth0-secrets
              key: domain
```

## 📁 Project Structure

```
backend/
├── api-gateway/
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── product-service/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
├── order-service/
│   └── ... (similar structure)
├── cart-service/
│   └── ... (similar structure)
├── payment-service/
│   └── ... (similar structure)
├── docker-compose.yml
└── README.md
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Commit changes: `git commit -am 'Add new feature'`
4. Push to branch: `git push origin feature/new-feature`
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/ecommerce-backend/issues)
- **Email**: support@yourapp.com
- **Documentation**: [Full API Docs](https://docs.yourapp.com)

## 🙏 Acknowledgments

- [YARP](https://microsoft.github.io/reverse-proxy/) - Reverse proxy library
- [Auth0](https://auth0.com) - Authentication service
- [.NET 9](https://dotnet.microsoft.com) - Development framework

---

**Note**: Remember to replace placeholder values (like Auth0 credentials, database connections) with your actual configuration values before deployment.