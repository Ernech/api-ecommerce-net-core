using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.DTO
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage ="El nombre es obligatorio")]
        [MinLength(3,ErrorMessage ="El nombre no puede tener menos de 3 caractéres")]
        [MaxLength(50,ErrorMessage ="El nombre no puede tener más de 50 caractéres")]
        public string Name { get; set; } = string.Empty;
    }
}
