using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using AutoMapper;

namespace ApiEcommerce.Mapping
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
        }
    }
}
