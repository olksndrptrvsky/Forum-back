using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class Theme
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public int? AuthorId { get; set; }
        public User Author { get; set; }
        public virtual IEnumerable<Message> Messages { get; set; }
    }
}
