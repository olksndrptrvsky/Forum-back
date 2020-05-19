using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class ThemeListItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string AuthorUsername { get; set; }
        public int MessageCount { get; set; }
    }
}
