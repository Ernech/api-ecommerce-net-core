using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;

using Mapster;

namespace ApiEcommerce.Mapping
{
    public class ProductMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDTO>()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
                .TwoWays();

            config.NewConfig<Product, CreateProductDTO>().TwoWays();
            config.NewConfig<Product, UpdateProductDTO>().TwoWays();
        }
    }
}
