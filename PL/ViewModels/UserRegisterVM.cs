using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels
{
    public class UserRegisterVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
