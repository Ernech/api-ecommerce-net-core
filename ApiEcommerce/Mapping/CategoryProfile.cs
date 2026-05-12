using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using AutoMapper;

namespace ApiEcommerce.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category,CreateCategoryDTO>().ReverseMap();
        }
    }
}
