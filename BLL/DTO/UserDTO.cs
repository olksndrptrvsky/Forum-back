using System.Collections.Generic;


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
