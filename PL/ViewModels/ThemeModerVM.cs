using System.ComponentModel.DataAnnotations;


namespace PL.ViewModels
{
    public class ThemeModerVM
    {
        [Required]
        public int ThemeId { get; set; }
        [Required]
        public string ModerUsername { get; set; }
    }
}
