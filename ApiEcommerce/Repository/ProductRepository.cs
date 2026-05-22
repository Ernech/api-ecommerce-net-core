using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository
{
    public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;


        public bool BuyProduct(string productName, int quantity)
        {
            if (String.IsNullOrWhiteSpace(productName) || quantity <= 0)
            {
                return false;
            }
            var product = _dbContext.Products.FirstOrDefault(p => p.Name.ToLower().Trim()==productName.ToLower().Trim());
            if (product == null || product.Stock<quantity)
            {
                return false;
            }
            product.Stock -= quantity;
            _dbContext.Products.Update(product);
            return Save();
                
        }

        public bool CreateProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            product.CreationDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;
            _dbContext.Products.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            _dbContext.Products.Remove(product);
            return Save();
        }

        public Product? GetProduct(int productId)
        {
            if (productId <= 0)
            {
                return null;
            }
            return _dbContext.Products.Include(p=>p.Category).FirstOrDefault(p => p.ProductId == productId);
        }

        public ICollection<Product> GetProducts()
        {
            return [.. _dbContext.Products.Include(p => p.Category).OrderBy(p=>p.Name)];
        }

        public ICollection<Product> GetProductsByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                return [];
            }
            return [.. _dbContext.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).OrderBy(p=>p.Name)];
        }

        public bool ProductExists(int productId)
        {
            if (productId == 0)
            {
                return false;
            }
            return _dbContext.Products.Any(p=>p.ProductId == productId);
        }

        public bool ProductExists(string productName)
        {
            if (String.IsNullOrWhiteSpace(productName))
            {
                return false;
            }
            return _dbContext.Products.Any(p => p.Name.ToLower().Trim() == productName.ToLower().Trim());
        }

        public bool Save()
        {
            return _dbContext.SaveChanges() >= 0;
        }

        public ICollection<Product> SearchProduct(string productName)
        {
            IQueryable<Product> query = _dbContext.Products;
            if (!String.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(p=>p.Name.ToLower().Trim() == productName.ToLower().Trim());
            }
            return query.OrderBy(p => p.Name).ToList();
        }

        public bool UpdateProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            _dbContext.Products.Update(product);
            return Save();
        }
    }
}
