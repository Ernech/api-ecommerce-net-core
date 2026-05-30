namespace ApiEcommerce.Models.DTO
{
    public class UserLoginResponseDTO
    {
        public UserRegisterDTO? User { get; set; }

        public string? Token { get; set; }

        public string? Message { get; set; }
    }
}
