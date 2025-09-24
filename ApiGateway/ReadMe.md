# Shop API Gateway – README  
> **Stack**: .NET 9, YARP (Yet Another Reverse Proxy), Auth0  
> **Purpose**: Single ingress for the Shop micro-services (Auth, Product, Cart, Order, Payment)

---

## 1. What this gateway gives you

| Feature | Benefit | When you’ll use it |
|---|---|---|
| **Single HTTPS entry-point** | `https://api.myshop.com` hides all internal ports | Public traffic, single TLS cert |
| **Auth0 JWT validation** | Rejects requests without valid JWT | Protect all downstream routes |
| **Claim-based routing** | Route admins to special versions | A/B or tiered APIs |
| **Rate limiting** | 100 req/min per IP | Stop brute-force / scraping |
| **Circuit breaker / retry** | 3× retries → breaker open | Survive pod restarts |
| **Hot-reload config** | Edit `appsettings.json` → zero restart | Dev loop & CI/CD |
| **gRPC & REST** | Same gateway handles both | Future-proof high-perf endpoints |

---

## 2. Quick start (local Docker)

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)  
- Docker Desktop  
- Auth0 account (free)

### 2.1 Clone & run
```bash
git clone <repo>
cd Shop
docker compose up --build