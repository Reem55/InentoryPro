
namespace InventoryAPI.Application.DTOs;

public record ProductDto(
    int Id, string Name, string Description,
    decimal Price, int Quantity, string Category,
    bool IsActive, string SupplierName, DateTime CreatedAt
);
public record CreateProductDto(
    string Name, string Description,
    decimal Price, int Quantity, string Category, int SupplierId
);
public record UpdateProductDto(
    string Name, string Description,
    decimal Price, int Quantity, string Category, bool IsActive
);
public record SupplierDto(int Id, string Name, string Email, string Phone, int ProductCount);
public record CreateSupplierDto(string Name, string Email, string Phone);
public record RegisterDto(string Username, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(
    string Token, string Username,
    string Email, string Role, DateTime ExpiresAt
);
public record ApiResponse<T>(bool Success, string Message, T? Data);