using BLL.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> LogIn(string userName, string password, string key);
        IdentityResult Register(string userName, string email, string password);
        Task<IEnumerable<string>> GetAllModers();

        
    }
}
