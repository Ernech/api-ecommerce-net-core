using ApiEcommerce.Models;
using ApiEcommerce.Models.DTO;
using Mapster;

namespace ApiEcommerce.Mapping
{
    public class UserMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserDTO>().TwoWays();
            config.NewConfig<User, CreateUserDTO>().TwoWays();
            config.NewConfig<User, UserLoginDTO>().TwoWays();
            config.NewConfig<User, UserLoginResponseDTO>().TwoWays();
            config.NewConfig<ApplicationUser, UserDataDTO>().TwoWays();
            config.NewConfig<ApplicationUser, UserDTO>().TwoWays();
        }
    }
}
