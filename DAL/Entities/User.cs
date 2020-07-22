using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace DAL.Entities
{
    public class User : IdentityUser<int>
    {
        public virtual IEnumerable<Message> Messages { get; set; }
        public virtual IEnumerable<Theme> Themes { get; set; }
    }
}
