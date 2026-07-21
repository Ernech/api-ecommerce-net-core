using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiEcommerce.Repository
{
    public class UserRepository(ApplicationDbContext dbContext,IConfiguration configuration, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager) : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly string? secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;



        public ApplicationUser? GetUser(string id)
        {
            return _dbContext.ApplicationUsers.FirstOrDefault(u=>u.Id==id);
        }

        public ICollection<ApplicationUser> GetUsers()
        {
            return [.. _dbContext.ApplicationUsers.OrderBy(u => u.UserName)];
        }

        public bool IsUniqueUser(string username)
        {
            return !_dbContext.ApplicationUsers.Any(u => u.UserName!=null && u.UserName.ToLower().Trim() == username.ToLower().Trim());
        }

        public async Task<UserLoginResponseDTO> Login(UserLoginDTO userLoginDTO)
        {
            if (string.IsNullOrWhiteSpace(userLoginDTO.Username))
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "El username es requerido"
                };
            }
            var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u=> u.UserName!=null && u.UserName.ToLower().Trim()==userLoginDTO.Username.ToLower().Trim());
            if (user == null)
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Usuario no encontrado"
                };
            }
            if (user ==null)
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Usuario no encontrado"
                };
            }
            if (userLoginDTO.Password == null) 
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Password Requerido"
                };
            }
            bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDTO.Password);
            if (!isValid)
            {
                return new UserLoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Las credenciales son incorrectas"
                };
            }
            var handlerToken = new JwtSecurityTokenHandler();
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("Secret key no configurada");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("id",user.Id.ToString()),
                    new Claim("username",user.UserName??string.Empty),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault()??string.Empty)

                ]),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handlerToken.CreateToken(tokenDescriptor);
            return new UserLoginResponseDTO() 
            {
                Message="Login exitoso",
                Token = handlerToken.WriteToken(token),
                User = user.Adapt<UserDataDTO>()
            };

        }

        public async Task<UserDataDTO> Register(CreateUserDTO createUserDTO)
        {
            if (string.IsNullOrWhiteSpace(createUserDTO.Username))
            {
                throw new ArgumentNullException("El username es requerido");
            }
            
            
            if (createUserDTO.Password == null)
            {
                throw new ArgumentNullException("El password es requerido");
            }
            var user = new ApplicationUser()
            {
                UserName = createUserDTO.Username,
                Email = createUserDTO.Username,
                NormalizedEmail = createUserDTO.Username.ToUpper(),
                Name =createUserDTO.Name
            };
            var result = await _userManager.CreateAsync(user,createUserDTO.Password);
            if (result.Succeeded)
            {
                var userRole = createUserDTO.Role ?? "User";
                var roleExists = await _roleManager.RoleExistsAsync(userRole);
                if (!roleExists)
                {
                    var identityRole = new IdentityRole(userRole);
                    await _roleManager.CreateAsync(identityRole);
                }
                await _userManager.AddToRoleAsync(user,userRole);
                var createdUser = _dbContext.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDTO.Username);
                return createdUser.Adapt<UserDataDTO>();
            }
            var errors = string.Join(",",result.Errors.Select(e=>e.Description));
            throw new ApplicationException($"No se pudo realizar el registro {errors}");
        }
    }
}
