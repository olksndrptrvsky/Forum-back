using BLL.DTO;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMessageService
    {
        IEnumerable<MessagesPerEntity> GetMessagesPerTheme();
        IEnumerable<MessagesPerEntity> GetMessagesPerUser();

        IEnumerable<MessageDTO> GetMessagesInTheme(int themeId, int pagingNumber, int pagingSize);

        int GetMessageCountForUser(int userId);

        void ReportMessage(ReportDTO report);

        bool UserCanDeleteMessage(int userId, int messageId);


        void DeleteMessage(int id);


        IEnumerable<MessageDTO> GetRepliesForMessage(int messageId, int pagingNumber, int pagingSize);

        Task<MessageDTO> CreateAsync(MessageDTO messageDTO, int authorId);

        Task<MessageDTO> GetMessageAsync(int messageId);

        Task UpdateAsync(int id, MessageDTO messageDTO);

        bool IsMessageExist(int id);

        int GetPagesCountForTheme(int id, int pageSize);

    }
}
