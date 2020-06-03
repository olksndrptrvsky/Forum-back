using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMessageService messageService;

        public ThemeService(IUnitOfWork unitOfWork, IMapper mapper, IMessageService messageService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.messageService = messageService;
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

        public ThemeDTO GetThemeById(int themeId, int pagingSize)
        {
            var messages = messageService.GetMessagesInTheme(themeId, 1, pagingSize);
            var theme = unitOfWork.Themes.GetAll(th => th.Id == themeId).Include(th => th.Author).First();
            var themeDTO = mapper.Map<ThemeDTO>(theme);
            themeDTO.Messages = mapper.Map<IEnumerable<MessageDTO>>(messages);

            var messagesPerUser = messageService.GetMessagesPerUser();

            foreach (var msg in themeDTO.Messages)
            {
                msg.Author.MessageCount = messagesPerUser.Where(mpu => mpu.EntityId == msg.Author.Id).First().MessageCount;
            }

            themeDTO.Author.MessageCount = messagesPerUser.Where(mpu => mpu.EntityId == theme.AuthorId).First().MessageCount;
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

            //get created theme with author
            createdTheme = unitOfWork.Themes.GetAll(th => th.Id == createdTheme.Id).Include(th => th.Author).First();
            var createdthemeDTO = mapper.Map<ThemeDTO>(createdTheme);
            createdthemeDTO.Author.MessageCount = messageService.GetMessageCountForUser(createdTheme.AuthorId.Value);
            
            return createdthemeDTO;
        }




        
        public IEnumerable<ThemeListItemDTO> GetThemesByHashtag(string hashtagText, int pagingNumber, int pagingSize)
        {
            var hashtag = unitOfWork.Hashtags.GetAll(h => h.Text == hashtagText).FirstOrDefault();
            if (hashtag == null) return null;

            var themes = unitOfWork.ThemeHashtags.GetAll(th => th.HashtagId == hashtag.Id).
                Join(unitOfWork.Themes.GetAll(), th => th.ThemeId, theme => theme.Id, (th, theme) => theme);
            
           
            return CreateThemeListItemDTOs(themes).Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);
        }


        public IEnumerable<ThemeListItemDTO> GetThemesWithoutModers(int pagingNumber, int pagingSize)
        {
            var themesWithoutModers = unitOfWork.Themes.GetAll().Include(t => t.ThemeModers)
                .Where(t => !t.ThemeModers.Any()).Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);

            return CreateThemeListItemDTOs(themesWithoutModers);

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


        public IEnumerable<ThemeListItemDTO> SearchByTitle(string query, int pagingNumber, int pagingSize)
        {
            var themes = unitOfWork.Themes.GetAll(t => t.Title.StartsWith(query))
                .Concat(unitOfWork.Themes.GetAll(t => t.Title.Contains(query))).Distinct();

            return CreateThemeListItemDTOs(themes);
        }


        public async void ReportTheme(ReportDTO report)
        {
            var reportTheme = mapper.Map<ReportTheme>(report);

            return await unitOfWork.;
        }

    }
}
