using ApiEcommerce.Constants;
using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="ADMIN")]
    //[EnableCors(PolicyName = PolicyNames.AllowSpecificOrigin)]
    public class CategoriesController(ICategoryRepository categoryRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var catedories = _categoryRepository.getCatgories();
            var categoriesDTO = new List<CategoryDTO>();
            foreach (var category in catedories) 
            {
                categoriesDTO.Add(_mapper.Map<CategoryDTO>(category));
            }
            return Ok(categoriesDTO);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult GetCategory(int id)
        {
            var category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
               return NotFound($"La categoría con el id ${id} no existe");
            }
            var categoryDTO = _mapper.Map<CategoryDTO>(category);
            
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
            var category = _mapper.Map<Category>(createCategoryDTO);
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
            var category = _mapper.Map<Category>(updateCategoryDTO);
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
