using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();


        User? GetUser(int id);

        bool IsUniqueUser(string username);

        Task<UserLoginResponseDTO> Login(UserLoginDTO userLoginDTO);

        Task<User> Register(UserRegisterDTO userRegisterDTO);
    }
}
