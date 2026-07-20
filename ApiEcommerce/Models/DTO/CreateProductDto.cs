namespace ApiEcommerce.Models.DTO
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? Image { get; set; }


        public string SKU { get; set; } = string.Empty;


        public int Stock { get; set; }

        public int CategoryId { get; set; }
    }
}
