using System;

namespace DAL.Entities
{
    public class ReportTheme
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Text { get; set; }
        public bool IsChecked { get; set; } = false;
        public int ThemeId { get; set; }
        public virtual Theme Theme { get; set; }
        public int ReporterId { get; set; }
        public virtual User Reporter { get; set; }
    }
}
