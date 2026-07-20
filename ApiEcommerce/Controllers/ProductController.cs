using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [ApiVersionNeutral]
    public class ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository,IMapper mapper) : ControllerBase
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);
            
            return Ok(productsDTO);
        }
        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProduct(int id)
        {
            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound($"EL producto con el id {id} no existe");
            }
            var productDTO = _mapper.Map<ProductDTO>(product);

            return Ok(productDTO);
        }

        [HttpPost(Name = "CreateProduct")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromForm] CreateProductDTO createProductDTO)
        {
            if (createProductDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_productRepository.ProductExists(createProductDTO.Name))
            {
                ModelState.AddModelError("CustomError", "El producto ya existe");
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(createProductDTO.CategoryId))
            {
                ModelState.AddModelError("CustomError", "La categoria no existe");
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(createProductDTO);
            //Agregando imagen
            if (createProductDTO.Image != null)
            {
                UploadProductImage(createProductDTO, product);
            }
            else 
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }
            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al guardar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            var createdProduct = _productRepository.GetProduct(product.ProductId);
            var productDTO = _mapper.Map<ProductDTO>(createdProduct);
            return CreatedAtRoute("GetProduct", new { id = product.ProductId }, productDTO);
        }


        [HttpGet("category/{categoryId:int}", Name = "GetProductsByCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            var categoryExists = _categoryRepository.CategoryExists(categoryId);
            if (!categoryExists)
            {
                return NotFound($"La categoría con el id {categoryId} no existe");
            }
            var products = _productRepository.GetProductsByCategory(categoryId);
            if (products.Count == 0)
            {
                return NotFound($"No se encontraro productos pertenecientes a la categoría {categoryId}");
            }
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);

            return Ok(productsDTO);
        }

        [HttpGet("term/{searchTerm}", Name = "SearchProductsByName")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProductByName(string searchTerm)
        {
            
            var products = _productRepository.SearchProducts(searchTerm);
            if (products.Count==0)
            {
                return NotFound($"No se encontraron productos con el nombre '{searchTerm}'");
            }
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);

            return Ok(productsDTO);
        }

        [HttpPatch("buy/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(string name, int quantity)
        {
            if (String.IsNullOrWhiteSpace(name) || quantity<=0)
            {
                return BadRequest("El nombre del producto o la cantidad no son válidos");
            }
            var foundProduct = _productRepository.ProductExists(name);
            if (!foundProduct)
            {
                return NotFound($"El producto con el nombre {name} no existe");
            }
            if (!_productRepository.BuyProduct(name,quantity))
            {
                ModelState.AddModelError("CustomError",$"No se pudo comprar el producto {name} o la cantidad solicitada es mayor al stock disponible");
                return BadRequest(ModelState);
            }
            var units = quantity == 1 ? "unidad" : "unidades";
            return Ok($"Se compró la cantidad {quantity} {units} del producto '{name}'");
        }

        [HttpPut("{productId:int}",Name = "ModifyProduct")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ModifyProduct(int productId, [FromForm] UpdateProductDTO updateProductDTO)
        {
            if (updateProductDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", $"El producto con id {productId} no existe");
                return NotFound(ModelState);
            }
            if (!_categoryRepository.CategoryExists(updateProductDTO.CategoryId))
            {
                ModelState.AddModelError("CustomError", "La categoria no existe");
                return NotFound(ModelState);
            }
            var product = _mapper.Map<Product>(updateProductDTO);
            product.ProductId = productId;
            if (updateProductDTO.Image != null)
            {
                UploadProductImage(updateProductDTO, product);
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }
            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        private void UploadProductImage(dynamic productDTO, Product product)
        {
            string fileName = product.ProductId + Guid.NewGuid().ToString() + Path.GetExtension(productDTO.Image.FileName);
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwrot", "ProductsImages");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            var filePath = Path.Combine(imageFolder, fileName);
            FileInfo file = new(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            using var fileStream = new FileStream(filePath, FileMode.Create);
            productDTO.Image.CopyTo(fileStream);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.ImageUrl = $"{baseUrl}/ProductsImages/{fileName}";
            product.ImageUrlLocal = filePath;
        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            { 
                return BadRequest(ModelState);
            }
            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound($"EL producto con el id {id} no existe");
            }
            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {product.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
