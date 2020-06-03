using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class ReportMessage
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Text { get; set; }
        public bool IsChecked { get; set; } = false;
        public int MessageId { get; set; }
        public virtual Message Message { get; set; }
        public int? ReporterId { get; set; }
        public virtual User Reporter { get; set; }
    }
}
