using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class ThemeModer
    {
        public int ThemeId { get; set; }
        public virtual Theme Theme { get; set; }
        public int ModeratorId { get; set; }
        public virtual User Moderator { get; set; }
    }
}
