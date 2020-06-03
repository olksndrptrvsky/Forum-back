using BLL.DTO;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IMessageService
    {
        IEnumerable<MessagesPerEntity> GetMessagesPerTheme();
        IEnumerable<MessagesPerEntity> GetMessagesPerUser();

        IEnumerable<Message> GetMessagesInTheme(int themeId, int pagingNumber, int pagingSize);

        int GetMessageCountForUser(int userId);

        
    }
}
