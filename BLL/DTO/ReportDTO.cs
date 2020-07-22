using System;


namespace BLL.DTO
{
    public class ReportDTO
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string Text { get; set; }
        public AuthorDTO Reporter { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
