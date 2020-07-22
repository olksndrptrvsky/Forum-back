using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels
{
    public class UserLoginVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
