using System;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Entities;
using DAL.EF;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ForumContext _db;

        private IRepository<Theme> _themes;
        private IRepository<Hashtag> _hashtags;
        private IRepository<ThemeHashtag> _themeHashtag;
        private IRepository<Message> _messages;
        private IRepository<ReportMessage> _reportMessages;
        private IRepository<ReportTheme> _reportThemes;
        private IRepository<ThemeModer> _themeModers;
        


        public IRepository<Theme> Themes => _themes ?? new Repository<Theme>(_db);
        public IRepository<Hashtag> Hashtags => _hashtags ?? new Repository<Hashtag>(_db);
        public IRepository<ThemeHashtag> ThemeHashtags => _themeHashtag ?? new Repository<ThemeHashtag>(_db);
        public IRepository<Message> Messages => _messages ?? new Repository<Message>(_db);
        public IRepository<ReportMessage> ReportMessages => _reportMessages ?? new Repository<ReportMessage>(_db);
        public IRepository<ReportTheme> ReportThemes => _reportThemes ?? new Repository<ReportTheme>(_db);
        public IRepository<ThemeModer> ThemeModers => _themeModers ?? new Repository<ThemeModer>(_db);


        public UnitOfWork(ForumContext productContext)
        {
            _db = productContext;
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
