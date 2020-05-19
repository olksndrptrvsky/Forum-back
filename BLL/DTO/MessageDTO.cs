using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public AuthorDTO Author { get; set; }
        public int? ReplyMessageId { get; set; }
    }
}
