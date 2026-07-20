using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models.DTO
{
    public class ProductDTO
    {

        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

       
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } 

        public string? ImageUrlLocal { get; set; } 
        
        public string SKU { get; set; } = string.Empty; 

       
        public int Stock { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; } = null;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

    }
}
