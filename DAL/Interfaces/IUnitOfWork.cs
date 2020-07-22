using System;
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
        IRepository<ThemeModer> ThemeModers { get; }
        IRepository<ReportTheme> ReportThemes { get; }
        IRepository<ReportMessage> ReportMessages { get; }
        Task<int> SaveAsync();
    }
}
