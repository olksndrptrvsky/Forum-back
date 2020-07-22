using System.ComponentModel.DataAnnotations;


namespace PL.ViewModels
{
    public class CreateMessageVM
    {
        [Required, MaxLength(5000)]
        public string Text { get; set; }
        public int? ReplyMessageId { get; set; }
        [Required]
        public int ThemeId { get; set; }
    }
}
