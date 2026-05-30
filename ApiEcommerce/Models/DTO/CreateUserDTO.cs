using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.DTO
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "El campo name es requerido")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El campo username es requerido")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "El campo password es requerido")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "El campo role es requerido")]
        public required string Role { get; set; }
    }
}
