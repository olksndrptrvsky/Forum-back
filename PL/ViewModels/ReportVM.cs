using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
