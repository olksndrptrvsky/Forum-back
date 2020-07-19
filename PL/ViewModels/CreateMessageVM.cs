using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
