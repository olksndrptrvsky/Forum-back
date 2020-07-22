using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using System.Linq.Expressions;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMessageService messageService;
        private readonly UserManager<User> userManager;

        public ThemeService(IUnitOfWork unitOfWork, IMapper mapper, IMessageService messageService, UserManager<User> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.messageService = messageService;
            this.userManager = userManager;
        }

        public IEnumerable<ThemeListItemDTO> GetLatestThemes(int pagingNumber, int pagingSize)
        {
            var themes = unitOfWork.Themes.GetAll().OrderByDescending(x => x.DateTime).
                Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);

            return CreateThemeListItemDTOs(themes);
        }
        
        
        public IEnumerable<ThemeListItemDTO> GetPopularThemes(int pagingNumber, int pagingSize)
        {
            var themes = unitOfWork.Themes.GetAll();
             
            return CreateThemeListItemDTOs(themes).OrderByDescending(th => th.MessageCount)
                .Skip((pagingNumber - 1) * pagingSize).Take(pagingSize).ToList();
        }

        public ThemeDTO GetThemeById(int themeId)
        {
            var theme = unitOfWork.Themes.GetAll(th => th.Id == themeId).Include(th => th.Author).FirstOrDefault();
            if (theme == null) return null;

            var themeDTO = mapper.Map<ThemeDTO>(theme);
           
            themeDTO.Author.MessageCount = messageService.GetMessageCountForUser(themeDTO.Author.Id);
            themeDTO.Hashtags = unitOfWork.ThemeHashtags.GetAll(th => th.ThemeId == themeDTO.Id).
                   Join(unitOfWork.Hashtags.GetAll(), th => th.HashtagId, h => h.Id, (th, h) => h.Text);

            return themeDTO; 
        }


        public async Task<ThemeDTO> CreateAsync(ThemeDTO themeDTO, int authorId)
        {
            var themeToCreate = mapper.Map<Theme>(themeDTO);
            themeToCreate.AuthorId = authorId;
            themeToCreate.DateTime = DateTime.Now;
            var createdTheme = await unitOfWork.Themes.CreateAsync(themeToCreate);

            await unitOfWork.SaveAsync();

            foreach (var hashtagText in themeDTO.Hashtags) 
            {
                
                var hashtag = unitOfWork.Hashtags.GetAll(h => h.Text == hashtagText).FirstOrDefault();
                if (hashtag == null)
                {
                    hashtag = await unitOfWork.Hashtags.CreateAsync(new Hashtag { Text = hashtagText });
                    await unitOfWork.SaveAsync();
                }
                await unitOfWork.ThemeHashtags.CreateAsync(new ThemeHashtag 
                { 
                    ThemeId = createdTheme.Id,
                    HashtagId = hashtag.Id }
                );

            }

            await unitOfWork.SaveAsync();

            
            createdTheme = unitOfWork.Themes.GetAll(th => th.Id == createdTheme.Id).Include(th => th.Author).First();
            var createdthemeDTO = mapper.Map<ThemeDTO>(createdTheme);
            createdthemeDTO.Author.MessageCount = messageService.GetMessageCountForUser(createdTheme.AuthorId.Value);
            
            return createdthemeDTO;
        }
 

        public IEnumerable<ThemeListItemDTO> GetThemesWithoutModers(int pagingNumber, int pagingSize)
        {
            var themesWithoutModers = unitOfWork.Themes.GetAll().Include(t => t.ThemeModers)
                .Where(t => !t.ThemeModers.Any()).Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);

            return CreateThemeListItemDTOs(themesWithoutModers);

        }



        public int GetUnmoderatedThemeCount()
        {
            return unitOfWork.Themes.GetAll().Include(t => t.ThemeModers)
                .Where(t => !t.ThemeModers.Any()).Count();
        }


        private IEnumerable<ThemeListItemDTO> CreateThemeListItemDTOs(IQueryable<Theme> themes)
        {
            themes = themes.Include(t => t.Author);

            var themeDTOs = mapper.Map<IEnumerable<ThemeListItemDTO>>(themes);

            var messagesPerTheme = messageService.GetMessagesPerTheme();


            foreach (var themeDTO in themeDTOs)
            {
                themeDTO.Hashtags = unitOfWork.ThemeHashtags.GetAll(th => th.ThemeId == themeDTO.Id).
                   Join(unitOfWork.Hashtags.GetAll(), th => th.HashtagId, h => h.Id, (th, h) => h.Text);
                themeDTO.MessageCount = messagesPerTheme.Where(mpt => mpt.EntityId == themeDTO.Id).First().MessageCount;
            }
            return themeDTOs;
        }


        public async Task ReportThemeAsync(ReportDTO report)
        {
            var reportTheme = mapper.Map<ReportTheme>(report);

            reportTheme.Reporter = null;
            
            await unitOfWork.ReportThemes.CreateAsync(reportTheme);

            await unitOfWork.SaveAsync();
        }

        public async Task AddModerToThemeAsync(ThemeModerDTO themeModerDTO)
        {
            var moder = userManager.FindByNameAsync(themeModerDTO.ModerUsername);
            var themeModer = mapper.Map<ThemeModer>(themeModerDTO);

            if (await moder == null)
            {
                throw new ArgumentException($"There is no user with username {themeModerDTO.ModerUsername}");
            }
            themeModer.ModeratorId = moder.Id;

            await unitOfWork.ThemeModers.CreateAsync(themeModer);

            await unitOfWork.SaveAsync();
        }


        public bool UserCanDeleteTheme(int userId, int themeId)
       {
            return unitOfWork.ThemeModers.GetAll(tm => tm.ModeratorId == userId && tm.ThemeId == themeId).Any() ||
                unitOfWork.Themes.GetAll(t => t.Id == themeId && t.AuthorId == userId).Any();
       }


        public async Task DeleteThemeAsync(int id)
        {
            var theme = unitOfWork.Themes.GetByIdAsync(id);
            unitOfWork.Themes.Delete(id);
            await SetHashtagsToTheme(await theme, new List<string>());
            await unitOfWork.SaveAsync();
        }

        public IEnumerable<ThemeListItemDTO> SearchThemes(string search, int pagingNumber, int pagingSize)
        {
            Regex regexHashtag = new Regex(@"\[(\w*[#\-\.]*)*\]");

            MatchCollection matches = regexHashtag.Matches(search);
            var searchStrings = regexHashtag.Split(search).Where(str => !string.IsNullOrEmpty(str)).ToList();

            //get all search strings
            searchStrings.ForEach((string s) => s = s.Trim());

            //and hashtags from input
            var hashtags = new List<string>();
            foreach (Match match in matches)
            {
                hashtags.Add(match.Value.Trim(new char[] { '[', ']' }));
            }

            //creating query
            IQueryable<Theme> query = unitOfWork.Themes.GetAll().Include(t => t.ThemeHashtags).ThenInclude(th => th.Hashtag);            
            foreach (var hashtag in hashtags)
            {
                query = query.Where(t => t.ThemeHashtags.Any(th => th.Hashtag.Text == hashtag));
            }


            if (searchStrings.Count > 0)
            {
                query = query.Where(GenerateSearchExpression(searchStrings));

            }

            return CreateThemeListItemDTOs(query);
        }


        private Expression<Func<Theme, bool>> GenerateSearchExpression(IEnumerable<string> searchStrings)
        {
            var theme = Expression.Parameter(typeof(Theme), "theme");

            Expression result = Expression.Constant(false);

            Expression title = Expression.Property(theme, "Title");
            Expression text = Expression.Property(theme, "Text");

            foreach (var searchString in searchStrings)
            {
                result = Expression.Or(result, Expression.Call(title, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                    Expression.Constant(searchString)));
                result = Expression.Or(result, Expression.Call(text, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                        Expression.Constant(searchString)));

            }


            return Expression.Lambda<Func<Theme, bool>>(result, new ParameterExpression[] { theme });
        }

        public bool IsThemeExist(int themeId)
        {
            return unitOfWork.Themes.GetAll(t => t.Id == themeId).Any();
        }

        public bool UserIsAuthor(int themeId, int userId)
        {
            var theme = unitOfWork.Themes.GetAll(t => t.Id == themeId).FirstOrDefault();
            return theme?.AuthorId == userId;
        }


        public async Task UpdateAsync(int id, ThemeDTO updatedTheme)
        {
            var theme = await unitOfWork.Themes.GetByIdAsync(id);
            theme.Text = updatedTheme.Text;
            theme.Title = updatedTheme.Title;
            await SetHashtagsToTheme(theme, updatedTheme.Hashtags);
            unitOfWork.Themes.Update(theme);
            await unitOfWork.SaveAsync();
        }


        private async Task SetHashtagsToTheme(Theme theme, IEnumerable<string> newTags)
        {
            var oldTags = unitOfWork.ThemeHashtags.GetAll(th => th.ThemeId == theme.Id).Select(th => th.Hashtag.Text).ToList();
            var toAdd = newTags.Except(oldTags);
            var toDel = oldTags.Except(newTags);


            foreach(var tag in toAdd)
            {
                var hashtag = unitOfWork.Hashtags.GetAll(h => h.Text == tag).FirstOrDefault();
                if (hashtag == null)
                {
                    hashtag = await unitOfWork.Hashtags.CreateAsync(new Hashtag { Text = tag });
                    await unitOfWork.SaveAsync();
                }
                await unitOfWork.ThemeHashtags.CreateAsync(new ThemeHashtag
                {
                    ThemeId = theme.Id,
                    HashtagId = hashtag.Id
                }
                );
            }


            foreach (var tag in toDel)
            {
                var hashtag = unitOfWork.Hashtags.GetAll(h => h.Text == tag).FirstOrDefault();
                var themeHashtag = unitOfWork.ThemeHashtags.GetAll(th => th.ThemeId == theme.Id && th.HashtagId == hashtag.Id);

                unitOfWork.ThemeHashtags.Delete(theme.Id, hashtag.Id);
                await unitOfWork.SaveAsync();

                if (!unitOfWork.ThemeHashtags.GetAll(th => th.HashtagId == hashtag.Id).Any())
                {
                    unitOfWork.Hashtags.Delete(hashtag.Id);
                }
            }

        }


        public IEnumerable<EntityReportDTO<ThemeListItemDTO>> GetReportedThemesWithReports(int moderId)
        {
            IQueryable<int> themeIds = unitOfWork.ThemeModers.GetAll(tm => tm.ModeratorId == moderId)
                .Include(tm => tm.Theme).Select(tm => tm.Theme.Id);

            var themes = unitOfWork.Themes.GetAll(t => themeIds.Any(id => id == t.Id));


            var themeDTOs = CreateThemeListItemDTOs(themes);

            var result = new List<EntityReportDTO<ThemeListItemDTO>>();



            foreach (var theme in themeDTOs)
            {
                var uncheckedReports = unitOfWork.ReportThemes.GetAll(rt => rt.ThemeId == theme.Id && !rt.IsChecked)
                    .Include(rt => rt.Reporter).ToList();
                var reportDTOs = mapper.Map<IEnumerable<ReportDTO>>(uncheckedReports);
                
                result.Add(new EntityReportDTO<ThemeListItemDTO>() { Entity = theme, Reports = reportDTOs });
            }
            return result;
        }

        public bool IsModeratingThemeReport(int moderId, int reportId)
        {
            var themeId = unitOfWork.ReportThemes.GetByIdAsync(reportId).Result.ThemeId;
            return unitOfWork.ThemeModers.GetByIdAsync(themeId, moderId).Result != null;
        }


        public async Task<ReportDTO> CheckReportAsync(int reportId)
        {
            var report = await unitOfWork.ReportThemes.GetByIdAsync(reportId);

            report.IsChecked = true;

            await unitOfWork.SaveAsync();

            return mapper.Map<ReportDTO>(report);
        }

    }
}
