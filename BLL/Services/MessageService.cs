using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<User> userManager;

        public MessageService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public IEnumerable<Message> GetMessagesInTheme(int themeId, int pagingNumber, int pagingSize)
        {
            var res = unitOfWork.Messages.GetAll(mes => mes.ThemeId == themeId && mes.ReplyMessageId == null).Include(mes => mes.Author).
                Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);
            
            return res.ToList();
        }

        public IEnumerable<MessagesPerEntity> GetMessagesPerTheme()
        {
            return unitOfWork.Themes.GetAll().Include(th => th.Messages).
                Select(th => new MessagesPerEntity { EntityId = th.Id, MessageCount = th.Messages.Count() });
        }

        public IEnumerable<MessagesPerEntity> GetMessagesPerUser()
        {
            return userManager.Users.Include(u => u.Messages).
                Select(u => new MessagesPerEntity { EntityId = u.Id, MessageCount = u.Messages.Count() });
        }

        public int GetMessageCountForUser(int userId)
        {
            return unitOfWork.Messages.GetAll(mes => mes.AuthorId == userId).Count();
        }

        public int GetMessageCountInTheme(int themeId)
        {
            return unitOfWork.Messages.GetAll(mes => mes.ThemeId == themeId).Count();
        }

    }
}
