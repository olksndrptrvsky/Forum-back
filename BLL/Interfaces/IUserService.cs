using BLL.DTO;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> LogInAsync(string userName, string password, string key);
        IdentityResult Register(string userName, string email, string password);
        Task<IEnumerable<string>> GetAllModersAsync();

        Task AddUserToModersAsync(string username);

    }
}
