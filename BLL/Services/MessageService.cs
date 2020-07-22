using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public MessageService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public IEnumerable<MessageDTO> GetMessagesInTheme(int themeId, int pagingNumber, int pagingSize)
        {
            var messages = unitOfWork.Messages.GetAll(mes => mes.ThemeId == themeId && mes.ReplyMessageId == null).Include(mes => mes.Author).
                Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);

            return CreateMessageDTOs(messages);
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

        private IEnumerable<MessagesPerEntity> GetRepliesPerMessage()
        {
            return unitOfWork.Messages.GetAll(m => m.ReplyMessageId != null).GroupBy(m => m.ReplyMessageId).
                Select(gr => new MessagesPerEntity { EntityId = gr.Key.Value, MessageCount = gr.Count() });
        }



        public int GetMessageCountForUser(int userId)
        {
            return unitOfWork.Messages.GetAll(mes => mes.AuthorId == userId).Count();
        }

        public int GetMessageCountInTheme(int themeId)
        {
            return unitOfWork.Messages.GetAll(mes => mes.ThemeId == themeId).Count();
        }

        public async Task ReportMessageAsync(ReportDTO report)
        {
            var reportMessage = mapper.Map<ReportMessage>(report);
            reportMessage.Reporter = null;
            await unitOfWork.ReportMessages.CreateAsync(reportMessage);

            await unitOfWork.SaveAsync();
        }


        public bool UserCanDeleteMessage(int userId, int messageId)
        {
            var message = unitOfWork.Messages.GetAll(m => m.Id == messageId).FirstOrDefault();

            var isModer = unitOfWork.ThemeModers.GetAll(tm => tm.ThemeId == message.ThemeId && tm.ModeratorId == userId).Any();

            return message?.AuthorId == userId || isModer;
        }


        public async Task DeleteMessageAsync(int id)
        {
            //delete replies
            foreach(var replyMessageId in unitOfWork.Messages.GetAll(m => m.ReplyMessageId == id).Select(m => m.Id))
            {
                unitOfWork.Messages.Delete(replyMessageId);
            }
            unitOfWork.Messages.Delete(id);
            await unitOfWork.SaveAsync();
        }

        public IEnumerable<MessageDTO> GetRepliesForMessage(int messageId, int pagingNumber, int pagingSize)
        {
            var replies = unitOfWork.Messages.GetAll(m => m.ReplyMessageId == messageId).
                Skip((pagingNumber - 1) * pagingSize).Take(pagingSize);
            return CreateMessageDTOs(replies);
        }


        private IEnumerable<MessageDTO> CreateMessageDTOs(IQueryable<Message> messages)
        {
            messages = messages.Include(m => m.Author);

            var messageDTOs = mapper.Map<IEnumerable<MessageDTO>>(messages);

            var messagesPerUser = GetMessagesPerUser();


            foreach(var messageDTO in messageDTOs)
            {
                messageDTO.Author.MessageCount = messagesPerUser.First(x => x.EntityId == messageDTO.Author.Id).MessageCount;
            }

            var repliesPerMessage = GetRepliesPerMessage();
            foreach (var msg in messageDTOs)
            {
                msg.HasReplies = repliesPerMessage.FirstOrDefault(rpm => rpm.EntityId == msg.Id) != null;
            }

            return messageDTOs;
        }



        public async Task<MessageDTO> CreateAsync(MessageDTO messageDTO, int authorId)
        {
            var messageToCreate = mapper.Map<Message>(messageDTO);
            messageToCreate.AuthorId = authorId;
            messageToCreate.DateTime = DateTime.Now;
            var createdMessage = await unitOfWork.Messages.CreateAsync(messageToCreate);

            await unitOfWork.SaveAsync();

            //get created message with author
            var getCreatedMessage = unitOfWork.Messages.GetAll(m => m.Id == createdMessage.Id);
            return CreateMessageDTOs(getCreatedMessage).First();
        }


        public MessageDTO GetMessage(int messageId)
        {
            var message = unitOfWork.Messages.GetAll(m => m.Id == messageId);
            return CreateMessageDTOs(message).FirstOrDefault();
        }


        public async Task UpdateAsync(int id, MessageDTO messageDTO)
        {
            var message = await unitOfWork.Messages.GetByIdAsync(id);
            message.Text = messageDTO.Text;
            unitOfWork.Messages.Update(message);
            await unitOfWork.SaveAsync();
        }


        public bool IsMessageExist(int id)
        {
            return unitOfWork.Messages.GetAll(m => m.Id == id).Any();
        }        


        public int GetPagesCountForTheme(int id, int pageSize)
        {
            return (unitOfWork.Messages.GetAll(m => m.ThemeId == id).Count() + pageSize - 1) / pageSize;
        }


        public IEnumerable<EntityReportDTO<MessageDTO>> GetReportedMessagesWithReports(int moderId)
        {
            IQueryable<int> moderatedThemeIds = unitOfWork.ThemeModers.GetAll(tm => tm.ModeratorId == moderId)
               .Include(tm => tm.Theme).Select(tm => tm.Theme.Id);

            var result = unitOfWork.ReportMessages.GetAll()
                .Include(rm => rm.Message).ThenInclude(m => m.Author).Include(rm => rm.Reporter)
                .Where(rm => moderatedThemeIds.Any(id => rm.Message.ThemeId == id) && !rm.IsChecked).ToList()
                .GroupBy(rm => rm.Message.Id).Select(gr => new EntityReportDTO<MessageDTO>
                {
                    Entity = mapper.Map<MessageDTO>(gr.ToList()[0].Message),
                    Reports = mapper.Map<IEnumerable<ReportDTO>>(gr.ToList())
                }).ToList();

            return result;
        }

        public bool IsModeratingMessageReport(int moderId, int reportId)
        {
            var themeId = unitOfWork.ReportMessages.GetAll(rm => rm.Id == reportId)
                .Include(rm => rm.Message).First().Message.ThemeId;
            return unitOfWork.ThemeModers.GetByIdAsync(themeId, moderId).Result != null;
        }


        public async Task<ReportDTO> CheckReportAsync(int reportId)
        {
            var report = await unitOfWork.ReportMessages.GetByIdAsync(reportId);

            report.IsChecked = true;

            var task = unitOfWork.SaveAsync();
            var reportDTO = mapper.Map<ReportDTO>(report);
            await task;
            return reportDTO;
        }


    }

}
