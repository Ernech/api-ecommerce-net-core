namespace ApiEcommerce.Models.DTO
{
    public class UserRegisterDTO
    {

        public string? ID { get; set; }

        public string? Name { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        public required  string Role { get; set; }

    }
}
