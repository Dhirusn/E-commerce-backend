using Microsoft.EntityFrameworkCore;
using ProductService.Data.Models;

namespace ProductService.Services
{
    public class ProductsService
    {
        private readonly ProductDbContext _context;

        public ProductsService(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateAsync(Guid id, Product input)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Title = input.Title;
            product.Description = input.Description;
            product.Price = input.Price;
            product.ImageUrl = input.ImageUrl;
            product.Stock = input.Stock;
            product.BrandId = input.BrandId;
            // Update other properties as needed

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
