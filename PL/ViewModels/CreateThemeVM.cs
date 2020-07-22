using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PL.ViewModels
{
    public class CreateThemeVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public IEnumerable<string> Hashtags { get; set; }
    }
}
