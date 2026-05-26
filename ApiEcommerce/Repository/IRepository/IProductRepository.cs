using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();

        ICollection<Product> GetProductsByCategory(int categoryId);

        ICollection<Product> SearchProducts(string searchTerm);

        Product? GetProduct(int productId);

        bool BuyProduct(string productName, int quantity);

        bool ProductExists(int productId);

        bool ProductExists(string productName);

        bool CreateProduct(Product product);

        bool UpdateProduct(Product product);

        bool DeleteProduct(Product product);

        bool Save();
    }
}
