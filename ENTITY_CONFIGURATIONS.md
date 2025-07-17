# Entity Framework Configurations

This document outlines all the Entity Framework configurations created for each microservice.

## **CartService Configurations**

### **Files Created:**
- `CartService/Data/Configurations/BaseConfiguration.cs`
- `CartService/Data/Configurations/CartConfiguration.cs`
- `CartService/Data/Configurations/CartItemConfiguration.cs`

### **Entities Configured:**
1. **Cart**
   - Primary Key: Guid Id
   - Properties: UserId, UserEmail, Status, TotalAmount, DiscountAmount, etc.
   - Relationships: One-to-many with CartItems
   - Indexes: UserId, UserEmail, Status, CreatedAt, ExpiresAt

2. **CartItem**
   - Primary Key: Guid Id
   - Properties: ProductId, ProductName, UnitPrice, Quantity, TotalPrice, etc.
   - Relationships: Many-to-one with Cart
   - Indexes: CartId, ProductId, ProductSku, IsAvailable
   - Unique Constraint: CartId + ProductId + ProductAttributes (prevents duplicates)

## **PaymentService Configurations**

### **Files Created:**
- `PaymentService/Data/Configurations/BaseConfiguration.cs`
- `PaymentService/Data/Configurations/PaymentConfiguration.cs`
- `PaymentService/Data/Configurations/PaymentTransactionConfiguration.cs`
- `PaymentService/Data/Configurations/RefundConfiguration.cs`

### **Entities Configured:**
1. **Payment**
   - Primary Key: Guid Id
   - Properties: OrderId, UserId, Amount, Currency, PaymentMethod, Status, etc.
   - Relationships: One-to-many with PaymentTransactions and Refunds
   - Indexes: OrderId, UserId, Status, PaymentIntentId, TransactionId, CreatedAt

2. **PaymentTransaction**
   - Primary Key: Guid Id
   - Properties: TransactionId, Amount, Currency, Type, Status, etc.
   - Relationships: Many-to-one with Payment
   - Indexes: PaymentId, TransactionId (unique), Type, Status, CreatedAt

3. **Refund**
   - Primary Key: Guid Id
   - Properties: Amount, Status, RefundId, GatewayResponse, etc.
   - Relationships: Many-to-one with Payment
   - Indexes: PaymentId, RefundId (unique), Status, CreatedAt

## **ProductService Configurations**

### **Files Created:**
- `ProductService/Data/Configurations/ProductReviewConfiguration.cs`

### **Entities Configured:**
1. **ProductReview**
   - Primary Key: Guid Id
   - Properties: ProductId, UserId, Rating, Comment
   - Relationships: Many-to-one with Product
   - Indexes: ProductId, UserId, Rating, CreatedAt
   - Unique Constraint: ProductId + UserId (prevents duplicate reviews)
   - Check Constraint: Rating between 1 and 5

## **OrderService Configurations**

### **Files Created:**
- `OrderService/Data/Configurations/AddressConfiguration.cs`

### **Entities Configured:**
1. **ShippingAddress** (Owned Entity)
   - Properties: FirstName, LastName, AddressLine1, AddressLine2, City, State, PostalCode, Country, Phone
   - Column names prefixed with "ShippingAddress_"
   - Owned by Order entity

2. **BillingAddress** (Owned Entity)
   - Properties: FirstName, LastName, AddressLine1, AddressLine2, City, State, PostalCode, Country, Phone
   - Column names prefixed with "BillingAddress_"
   - Owned by Order entity

## **Common Features in All Configurations**

### **BaseEntity Configuration**
All services include a `BaseConfiguration.cs` with extensions for:
- **Primary Keys**: Guid with NEWSEQUENTIALID() or Int with Identity
- **Audit Fields**: CreatedAt, UpdatedAt with default values
- **Soft Delete**: IsDeleted with query filters
- **Timestamps**: Automatic UTC timestamps

### **Naming Conventions**
- **Tables**: PascalCase entity names (e.g., "Carts", "PaymentTransactions")
- **Columns**: Default property names unless explicitly configured
- **Indexes**: Prefixed with "IX_" followed by table and column names
- **Constraints**: Prefixed with "CK_" for check constraints

### **Data Types**
- **Decimal**: All monetary values use `decimal(18,2)`
- **Strings**: Appropriate max lengths for different field types
- **Enums**: Converted to integers for database storage
- **JSON**: Stored as nvarchar(max) for metadata fields

## **DbContext Updates**

All DbContext files have been updated to use:
```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);
```

This automatically applies all IEntityTypeConfiguration implementations from the assembly.

## **Key Benefits**

1. **Separation of Concerns**: Each entity has its own configuration file
2. **Maintainability**: Easy to modify individual entity configurations
3. **Consistency**: Standardized patterns across all services
4. **Performance**: Proper indexes and constraints defined
5. **Data Integrity**: Foreign keys, unique constraints, and check constraints
6. **Extensibility**: Easy to add new configurations or modify existing ones

## **Usage**

When creating new migrations, these configurations will be automatically applied:

```bash
# For CartService
dotnet ef migrations add AddEntityConfigurations --project CartService

# For PaymentService  
dotnet ef migrations add AddEntityConfigurations --project PaymentService

# For ProductService
dotnet ef migrations add AddProductReviewConfiguration --project ProductService

# For OrderService
dotnet ef migrations add AddAddressConfigurations --project OrderService
```

The configurations ensure consistent database schema generation and proper relationships between entities.
