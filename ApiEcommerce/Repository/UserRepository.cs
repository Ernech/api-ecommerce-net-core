using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiEcommerce.Repository
{
    public class UserRepository(ApplicationDbContext dbContext,IConfiguration configuration) : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private string? secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        public User? GetUser(int id)
        {
            return _dbContext.Users.FirstOrDefault(u=>u.Id==id);
        }

        public ICollection<User> GetUsers()
        {
            return _dbContext.Users.OrderBy(u => u.Username).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            return !_dbContext.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
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
            var user = await _dbContext.Users.FirstOrDefaultAsync<User>(u=>u.Username.ToLower().Trim()==userLoginDTO.Username.ToLower().Trim());
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
            if (!BCrypt.Net.BCrypt.Verify(userLoginDTO.Password,user.Password))
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
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("id",user.Id.ToString()),
                    new Claim("username",user.Username),
                    new Claim(ClaimTypes.Role,user.Role??string.Empty)

                ]),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handlerToken.CreateToken(tokenDescriptor);
            return new UserLoginResponseDTO() 
            {
                Message="Login exitoso",
                Token = handlerToken.WriteToken(token),
                User = new UserRegisterDTO 
                {
                    Name = user.Name,
                    Password=user.Password?? string.Empty,
                    Username = user.Username,
                    Role = user.Role ??string.Empty
                }
            };

        }

        public async Task<User> Register(CreateUserDTO createUserDTO)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDTO.Password);
            var user = new User() 
            { 
                Name = createUserDTO.Name,
                Username = createUserDTO.Username,
                Password = encryptedPassword,
                Role = createUserDTO.Role,

            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
