using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserRepository userRepository, IMapper mapper) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUsers()
        { 
            var users =_userRepository.GetUsers();
            var usersDTO = _mapper.Map<List<UserDTO>>(users);
            return Ok(usersDTO);
        
        }

        [HttpGet("{userId:int}", Name ="GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUser(int userId)
        {
            var user = _userRepository.GetUser(userId);
            if (user == null)
            {
                return NotFound($"El usuario con el id {userId} no fue encontrado");
                
            }
            var userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }
        [AllowAnonymous]
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (String.IsNullOrWhiteSpace(createUserDTO.Username))
            {
                return BadRequest("Username es requerido");
            }
            if (!_userRepository.IsUniqueUser(createUserDTO.Username))
            {
                return BadRequest("EL usuario ya existe");
            }
            var result = await _userRepository.Register(createUserDTO);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario");
            }
            return CreatedAtRoute("GetUser", new { userId = result.Id});
        
        }
        [AllowAnonymous]
        [HttpPost("Login",Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDTO userLoginDTO)
        {
            if (userLoginDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _userRepository.Login(userLoginDTO);
            if (result == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }
            return Ok(result);

        }


    }
}
