using BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMessageService
    {
        IEnumerable<MessagesPerEntity> GetMessagesPerTheme();
        IEnumerable<MessagesPerEntity> GetMessagesPerUser();

        IEnumerable<MessageDTO> GetMessagesInTheme(int themeId, int pagingNumber, int pagingSize);

        int GetMessageCountForUser(int userId);

        Task ReportMessageAsync(ReportDTO report);

        bool UserCanDeleteMessage(int userId, int messageId);


        Task DeleteMessageAsync(int id);


        IEnumerable<MessageDTO> GetRepliesForMessage(int messageId, int pagingNumber, int pagingSize);

        Task<MessageDTO> CreateAsync(MessageDTO messageDTO, int authorId);

        MessageDTO GetMessage(int messageId);

        Task UpdateAsync(int id, MessageDTO messageDTO);

        bool IsMessageExist(int id);

        int GetPagesCountForTheme(int id, int pageSize);

        IEnumerable<EntityReportDTO<MessageDTO>> GetReportedMessagesWithReports(int moderId);

        bool IsModeratingMessageReport(int moderId, int reportId);

        Task<ReportDTO> CheckReportAsync(int reportId);

    }
}
