using System;
using System.Collections.Generic;


namespace DAL.Entities
{
    public class Theme
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public int? AuthorId { get; set; }
        public virtual User Author { get; set; }
        public virtual IEnumerable<Message> Messages { get; set; }
        public virtual IEnumerable<ThemeModer> ThemeModers { get; set; }
        public virtual IEnumerable<ThemeHashtag> ThemeHashtags { get; set; }
    }
}
