using ApiEcommerce.Constants;
using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.V2

{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    //[EnableCors(PolicyName = PolicyNames.AllowSpecificOrigin)]
    public class CategoriesController(ICategoryRepository categoryRepository) : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

      
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategoriesOrderedById()
        {
            var catedories = _categoryRepository.getCatgories().OrderBy(cat=>cat.Id);
            var categoriesDTO = catedories.Adapt<List<CategoryDTO>>();
            return Ok(categoriesDTO);
        }


        [HttpGet("{id:int}", Name = "GetCategory")]
        //[ResponseCache(Duration =10)]
        [ResponseCache(CacheProfileName =CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult GetCategory(int id)
        {
            Console.WriteLine($"Categoría con el ID: {id} a las {DateTime.Now}");
            var category = _categoryRepository.GetCategory(id);
            Console.WriteLine($"Respuesta con el ID: {id}");
            if (category == null)
            {
               return NotFound($"La categoría con el id ${id} no existe");
            }
            var categoryDTO = category.Adapt<CategoryDTO>();
            
            return Ok(categoryDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            if (createCategoryDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.CategoryExists(createCategoryDTO.Name))
            {
                ModelState.AddModelError("CustomError","La Categoría ya existe");
                return BadRequest(ModelState);
            }
            var category = createCategoryDTO.Adapt<Category>();
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError",$"Algo salió mal al guardar el registro {category.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategory",new { id=category.Id},category);
        }

        [HttpPatch("{id:int}",Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDTO updateCategoryDTO)
        {
            if (updateCategoryDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"La categoría con el id {id} no existe");
            }
            if (_categoryRepository.CategoryExists(updateCategoryDTO.Name))
            {
                ModelState.AddModelError("CustomError", "La Categoría ya existe");
                return BadRequest(ModelState);
            }
            var category = updateCategoryDTO.Adapt<Category>();
            category.Id = id;
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al actualizar el registro {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            var category=_categoryRepository.GetCategory(id);
            if (category==null)
            {
                return NotFound($"La categoría con el id {id} no existe");
            }
            
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
