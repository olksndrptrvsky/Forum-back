using System;

namespace DAL.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }

        public int ThemeId { get; set; }
        public virtual Theme Theme { get; set; }
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public int? ReplyMessageId { get; set; }
        public virtual Message ReplyMessage { get; set; }

    }
}
