using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;


namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Theme> Themes { get; }
        IRepository<Hashtag> Hashtags { get; }
        IRepository<ThemeHashtag> ThemeHashtags { get; }
        IRepository<Message> Messages { get; }
        Task<int> SaveAsync();
    }
}
