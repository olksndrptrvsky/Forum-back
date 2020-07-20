using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AutoMapper;
using System.Linq;
using DAL.Repositories;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<UserDTO> LogIn(string userName, string password, string key)
        {
            if (await IsValidUsernameAndPassword(userName, password))
            {
                return await GenerateToken(userName, key);
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> IsValidUsernameAndPassword(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            return await userManager.CheckPasswordAsync(user, password);
        }


        private async Task<UserDTO> GenerateToken(string userName, string key)
        {
            var user = await userManager.FindByNameAsync(userName);
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //not before (not valid before cetrain date and time)
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                //exparation
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(60)).ToUnixTimeSeconds().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            var userDTO = mapper.Map<UserDTO>(user);
            userDTO.Roles = roles;
            userDTO.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return userDTO;
            
        }

        public IdentityResult Register(string userName, string email, string password)
        {
            User user = new User() { UserName = userName, Email = email };
            var result = userManager.CreateAsync(user, password).Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "User");
            }
            return result;
        }

        
        public async Task<IEnumerable<string>> GetAllModers()
        {
            return (await userManager.GetUsersInRoleAsync("Moderator")).Select(u => u.UserName).ToList();
        }


        public async Task AddUserToModers(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null) throw new ArgumentException($"There is no user with name `{username}`");

            await userManager.AddToRoleAsync(user, "Moderator");
        }


    }
}
