using InventoryAPI.Application.DTOs;
using InventoryAPI.Domain.Entities;
using InventoryAPI.Domain.Interfaces;

namespace InventoryAPI.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<ProductDto>> SearchAsync(string keyword);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        // ✅ DTO Mapping using LINQ Select
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
    {
        var products = await _repo.GetByCategoryAsync(category);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string keyword)
    {
        var products = await _repo.SearchAsync(keyword);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        // Check duplicate
        if (await _repo.ExistsAsync(dto.Name))
            throw new InvalidOperationException($"Product '{dto.Name}' already exists.");

        // ✅ DTO → Entity mapping
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Category = dto.Category,
            SupplierId = dto.SupplierId
        };

        var created = await _repo.CreateAsync(product);
        return MapToDto(created);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null) return null;

        // ✅ Update entity from DTO
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.Category = dto.Category;
        product.IsActive = dto.IsActive;

        var updated = await _repo.UpdateAsync(product);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
        => await _repo.DeleteAsync(id);

    // ✅ DTO Mapping method
    private static ProductDto MapToDto(Product p) => new(
        Id: p.Id,
        Name: p.Name,
        Description: p.Description,
        Price: p.Price,
        Quantity: p.Quantity,
        Category: p.Category,
        IsActive: p.IsActive,
        SupplierName: p.Supplier?.Name ?? "Unknown",
        CreatedAt: p.CreatedAt
    );
}
