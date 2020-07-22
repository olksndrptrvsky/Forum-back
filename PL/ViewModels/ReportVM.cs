using System.ComponentModel.DataAnnotations;


namespace PL.ViewModels
{
    public class ReportVM
    {
        [Required]
        public int EntityId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
