using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string AccessToken { get; set; }
    }
}
