using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Skip((pagingNumber - 1) * pagingSize).Take(pagingSize).Include(x => x.Author).ToList();

            var messagesPerTheme = messageService.GetMessagesPerTheme().ToList();

            var themeDTOs = mapper.Map<IEnumerable<ThemeListItemDTO>>(themes);
            foreach (var themeDTO in themeDTOs)
            {
                themeDTO.MessageCount = messagesPerTheme.Where(mpt => mpt.EntityId == themeDTO.Id).First().MessageCount;
            }
            return themeDTOs;
        }
        
        public IEnumerable<ThemeListItemDTO> GetPopularThemes(int pagingNumber, int pagingSize)
        {
            var themes = unitOfWork.Themes.GetAll().Include(th => th.Author);
               
            var messagesPerTheme = messageService.GetMessagesPerTheme();

            var themeDTOs = mapper.Map<IEnumerable<ThemeListItemDTO>>(themes);
            foreach (var themeDTO in themeDTOs)
            {
                themeDTO.MessageCount = messagesPerTheme.Where(mpt => mpt.EntityId == themeDTO.Id).First().MessageCount;
            }
            return themeDTOs.OrderByDescending(th => th.MessageCount).Skip((pagingNumber - 1) * pagingSize).Take(pagingSize).ToList();
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
            return themeDTO; 
        }


        public async Task<ThemeDTO> CreateAsync(ThemeDTO themeDTO, int authorId)
        {
            var themeToCreate = mapper.Map<Theme>(themeDTO);
            themeToCreate.AuthorId = authorId;
            themeToCreate.DateTime = DateTime.Now;
            var createdTheme = await unitOfWork.Themes.CreateAsync(themeToCreate);
            await unitOfWork.SaveAsync();
            createdTheme = unitOfWork.Themes.GetAll(th => th.Id == createdTheme.Id).Include(th => th.Author).First();
            var createdthemeDTO = mapper.Map<ThemeDTO>(createdTheme);
            createdthemeDTO.Author.MessageCount = messageService.GetMessageCountForUser(createdTheme.AuthorId.Value);
                
            return createdthemeDTO;
        }

        public IEnumerable<ThemeListItemDTO> GetThemesByHashtag(int pagingNumber, int pagingSize)
        {
            throw new NotImplementedException();
        }
    }
}
