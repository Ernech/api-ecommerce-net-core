using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using Mapster;

namespace ApiEcommerce.Mapping
{
    public class CategoryMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryDTO>().TwoWays();
            config.NewConfig<Category, CreateCategoryDTO>().TwoWays();
        }
    }
}
