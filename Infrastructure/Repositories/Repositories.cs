using Microsoft.EntityFrameworkCore;
using InventoryAPI.Domain.Entities;
using InventoryAPI.Domain.Interfaces;
using InventoryAPI.Infrastructure.Data;

namespace InventoryAPI.Infrastructure.Repositories;

// ✅ Generic Repository Implementation
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null) return false;
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ✅ Product Repository
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        => await _context.Products
            .Include(p => p.Supplier)
            .Where(p => p.Category == category && p.IsActive)
            .ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        => await _context.Products
            .Include(p => p.Supplier)
            .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
            .ToListAsync();

    public async Task<bool> ExistsAsync(string name)
        => await _context.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower());

    public new async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products
            .Include(p => p.Supplier)
            .ToListAsync();

    public new async Task<Product?> GetByIdAsync(int id)
        => await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);
}

// ✅ Supplier Repository
public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context) { }

    public async Task<Supplier?> GetWithProductsAsync(int id)
        => await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
}

// ✅ User Repository
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
