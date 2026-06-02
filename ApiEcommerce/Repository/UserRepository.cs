using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository
{
    public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
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

        public Task<UserLoginResponseDTO> Login(UserLoginDTO userLoginDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Register(UserRegisterDTO userRegisterDTO)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDTO.Password);
            var user = new User() 
            { 
                Name = userRegisterDTO.Name,
                Username = userRegisterDTO.Username,
                Password = encryptedPassword,
                Role = userRegisterDTO.Role,

            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
