using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IThemeService
    {
        IEnumerable<ThemeListItemDTO> GetPopularThemes(int pagingNumber, int pagingSize);
        IEnumerable<ThemeListItemDTO> GetLatestThemes(int pagingNumber, int pagingSize);

        ThemeDTO GetThemeById(int id, int pagingSize);

        Task<ThemeDTO> CreateAsync(ThemeDTO themeDTO, int authorId);




        IEnumerable<ThemeListItemDTO> GetThemesByHashtag(int pagingNumber, int pagingSize);
    }
}
