using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository
{
    public class CategoryRepository(ApplicationDbContext db) : ICategoryRepository
    {
        private readonly ApplicationDbContext _db = db;

        public bool CategoryExists(int id)
        {
            return _db.Categories.Any(c => c.Id==id);
        }

        public bool CategoryExists(string name)
        {
            return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool CreateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db.Categories.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _db.Categories.Remove(category);
            return Save();
        }

        public Category? GetCategory(int id)
        {
            return _db.Categories.FirstOrDefault(c => c.Id == id) ;
        }

        public ICollection<Category> getCatgories()
        {
            return [.. _db.Categories.OrderBy(c => c.Name)];
        }

        public bool Save()
        {
            return _db.SaveChanges()>=0;
        }

        public bool UpdateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db.Categories.Update(category);
            return Save();
        }
    }
}
