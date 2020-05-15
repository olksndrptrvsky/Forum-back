using DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.EF
{
    public class ForumContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<ReportMessage> ReportMessages { get; set; }
        public DbSet<ReportTheme> ReportThemes { get; set; }
        public DbSet<ThemeHashtag> ThemeHashtags { get; set; }


        public ForumContext(DbContextOptions options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ThemeHashtag>().HasKey(th => new { th.ThemeId, th.HashtagId });


        }
    }
}
